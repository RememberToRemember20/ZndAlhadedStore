using AuthKit.Common;
using AuthKit.Features.Auth.Register.Command;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs.Auth;


namespace AuthKit.Features.Auth.Register.Handler
{
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class RegisterCommandHandler<TUser, TKey, TContext> : IRequestHandler<RegisterCommand<TUser, TKey>, Result<AuthResult>>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TContext : DbContext
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IAuthTokenIssuer<TUser, TKey> _tokenIssuer;
        private readonly TContext _dbContext;
        private readonly ILogger<RegisterCommandHandler<TUser, TKey, TContext>> _logger;

        public RegisterCommandHandler(
            UserManager<TUser> userManager,
            IAuthTokenIssuer<TUser, TKey> tokenIssuer,
            TContext dbContext,
            ILogger<RegisterCommandHandler<TUser, TKey, TContext>> logger)
        {
            _userManager = userManager;
            _tokenIssuer = tokenIssuer;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<AuthResult>> Handle(RegisterCommand<TUser, TKey> request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.User.Email!);
            if (existingUser is not null)
                return Result<AuthResult>.Failure("البريد الإلكتروني مستخدم بالفعل.");

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var createResult = await _userManager.CreateAsync(request.User, request.Password);
                if (!createResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
                    return Result<AuthResult>.Failure(errors);
                }

                var authResult = await _tokenIssuer.IssueTokensAsync(request.User);

                await transaction.CommitAsync(cancellationToken);
                return Result<AuthResult>.Success(authResult);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "فشلت عملية إكمال التسجيل، تم التراجع الكامل عن إنشاء المستخدم.");
                return Result<AuthResult>.Failure("حدث خطأ أثناء إكمال عملية التسجيل. حاول مرة أخرى.");
            }
        }
    }
}
