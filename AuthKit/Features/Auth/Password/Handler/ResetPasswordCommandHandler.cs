using AuthKit.Common;
using AuthKit.Features.Auth.Password.Command;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Password.Handler
{
    public class ResetPasswordCommandHandler<TUser, TKey, TContext> : IRequestHandler<ResetPasswordCommand, Result>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    where TContext : DbContext
    {
        private const string InvalidLinkMessage = "رابط الاسترجاع غير صالح أو منتهي الصلاحية.";

        private readonly UserManager<TUser> _userManager;
        private readonly IAuthTokenIssuer<TUser, TKey> _tokenIssuer;
        private readonly TContext _dbContext;
        private readonly ILogger<ResetPasswordCommandHandler<TUser, TKey, TContext>> _logger;

        public ResetPasswordCommandHandler(
            UserManager<TUser> userManager,
            IAuthTokenIssuer<TUser, TKey> tokenIssuer,
            TContext dbContext,
            ILogger<ResetPasswordCommandHandler<TUser, TKey, TContext>> logger)
        {
            _userManager = userManager;
            _tokenIssuer = tokenIssuer;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null) return Result.Failure(InvalidLinkMessage);

          //  string token;
            if (!UrlTokenEncoder.TryDecode(request.Token, out var token))
                return Result.Failure(InvalidLinkMessage);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.InvalidToken))
                        ? Result.Failure(InvalidLinkMessage)
                        : Result.Failure(string.Join(" | ", result.Errors.Select(e => e.Description)));
                }

                await _userManager.SetLockoutEndDateAsync(user, null);
                await _userManager.ResetAccessFailedCountAsync(user);
                await _tokenIssuer.RevokeAllActiveTokensAsync(user.Id, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "فشلت عملية إعادة تعيين كلمة المرور، تم التراجع الكامل.");
                return Result.Failure("حدث خطأ أثناء إعادة تعيين كلمة المرور. حاول مرة أخرى.");
            }
        }
    }
}
