using AuthKit.Common;
using AuthKit.Features.Auth.Password.Command;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Password.Handler
{
    public class ForgotPasswordCommandHandler<TUser, TKey> : IRequestHandler<ForgotPasswordCommand, Result>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IAuthEmailSender<TUser> _emailSender;
        private readonly IUserAccessGuard<TUser>? _accessGuard;
        private readonly ILogger<ForgotPasswordCommandHandler<TUser, TKey>> _logger;

        public ForgotPasswordCommandHandler(
            UserManager<TUser> userManager,
            IAuthEmailSender<TUser> emailSender,
            ILogger<ForgotPasswordCommandHandler<TUser, TKey>> logger,
            IUserAccessGuard<TUser>? accessGuard = null)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _accessGuard = accessGuard;
        }

        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null || (_accessGuard is not null && !_accessGuard.CanSignIn(user)))
                return Result.Success();

            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = UrlTokenEncoder.Encode(token);
                await _emailSender.SendPasswordResetAsync(user, encodedToken, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "فشل إرسال رسالة استرجاع كلمة المرور.");
            }

            return Result.Success();
        }
    }
}
