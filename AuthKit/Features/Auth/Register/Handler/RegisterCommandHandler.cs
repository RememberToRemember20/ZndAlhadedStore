using AuthKit.Common;
using AuthKit.Features.Auth.Register.Command;
using AuthKit.Models;
using AuthKit.Services.Interfaces;
using AuthKit.Settings;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;



namespace AuthKit.Features.Auth.Register.Handler
{


    public class RegisterCommandHandler<TUser, TKey, TContext> : IRequestHandler<RegisterCommand<TUser, TKey>, Result<RegisterResult>>
     where TUser : IdentityUser<TKey>
     where TKey : IEquatable<TKey>
     where TContext : DbContext
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IAuthTokenIssuer<TUser, TKey> _tokenIssuer;
        private readonly TContext _dbContext;
        private readonly AuthKitOptions _options;
        private readonly ILogger<RegisterCommandHandler<TUser, TKey, TContext>> _logger;
        private readonly IAuthEmailSender<TUser>? _emailSender;

        public RegisterCommandHandler(
            UserManager<TUser> userManager,
            IAuthTokenIssuer<TUser, TKey> tokenIssuer,
            TContext dbContext,
            IOptions<AuthKitOptions> options,
            ILogger<RegisterCommandHandler<TUser, TKey, TContext>> logger,
            IAuthEmailSender<TUser>? emailSender = null)
        {
            _userManager = userManager;
            _tokenIssuer = tokenIssuer;
            _dbContext = dbContext;
            _options = options.Value;
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<Result<RegisterResult>> Handle(RegisterCommand<TUser, TKey> request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.User.Email!);
            if (existingUser is not null)
                return Result<RegisterResult>.Failure("البريد الإلكتروني مستخدم بالفعل.");

            AuthResult? authResult = null;

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var createResult = await _userManager.CreateAsync(request.User, request.Password);
                if (!createResult.Succeeded)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Result<RegisterResult>.Failure(string.Join(" | ", createResult.Errors.Select(e => e.Description)));
                }

                if (!_options.RequireConfirmedEmail)
                    authResult = await _tokenIssuer.IssueTokensAsync(request.User);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "فشلت عملية إكمال التسجيل، تم التراجع الكامل عن إنشاء المستخدم.");
                return Result<RegisterResult>.Failure("حدث خطأ أثناء إكمال عملية التسجيل. حاول مرة أخرى.");
            }

            if (_options.SendConfirmationEmailOnRegister)
                await TrySendConfirmationEmailAsync(request.User, cancellationToken);

            return Result<RegisterResult>.Success(new RegisterResult(authResult, _options.RequireConfirmedEmail));
        }

        private async Task TrySendConfirmationEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            if (_emailSender is null)
            {
                _logger.LogWarning("تأكيد البريد مفعّل لكن لا يوجد IAuthEmailSender مسجّل.");
                return;
            }

            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _emailSender.SendEmailConfirmationAsync(user, UrlTokenEncoder.Encode(token), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "فشل إرسال رسالة تأكيد البريد بعد التسجيل.");
            }
        }
    }
}
