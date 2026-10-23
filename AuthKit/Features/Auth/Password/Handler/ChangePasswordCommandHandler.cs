using AuthKit.Common;
using AuthKit.Features.Auth.Password.Command;
using AuthKit.Models;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Password.Handler
{
    public class ChangePasswordCommandHandler<TUser, TKey, TContext> : IRequestHandler<ChangePasswordCommand, Result<AuthResult>>
     where TUser : IdentityUser<TKey>
     where TKey : IEquatable<TKey>
     where TContext : DbContext
    {
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;
        private readonly IAuthTokenIssuer<TUser, TKey> _tokenIssuer;
        private readonly TContext _dbContext;
        private readonly ILogger<ChangePasswordCommandHandler<TUser, TKey, TContext>> _logger;

        public ChangePasswordCommandHandler(
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            IAuthTokenIssuer<TUser, TKey> tokenIssuer,
            TContext dbContext,
            ILogger<ChangePasswordCommandHandler<TUser, TKey, TContext>> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenIssuer = tokenIssuer;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<AuthResult>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
                return Result<AuthResult>.Failure("الحساب غير متاح.");

            var check = await _signInManager.CheckPasswordSignInAsync(user, request.CurrentPassword, lockoutOnFailure: true);
            if (check.IsLockedOut)
                return Result<AuthResult>.Failure("تم قفل الحساب مؤقتاً بسبب محاولات فاشلة متكررة.");
            if (!check.Succeeded)
                return Result<AuthResult>.Failure("كلمة المرور الحالية غير صحيحة.");

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<AuthResult>.Failure(string.Join(" | ", result.Errors.Select(e => e.Description)));
                }

                await _tokenIssuer.RevokeAllActiveTokensAsync(user.Id, cancellationToken);
                var authResult = await _tokenIssuer.IssueTokensAsync(user);

                await transaction.CommitAsync(cancellationToken);
                return Result<AuthResult>.Success(authResult);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "فشل تغيير كلمة المرور، تم التراجع الكامل.");
                return Result<AuthResult>.Failure("حدث خطأ أثناء تغيير كلمة المرور. حاول مرة أخرى.");
            }
        }
    }
}
