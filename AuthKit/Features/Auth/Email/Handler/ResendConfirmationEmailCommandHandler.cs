using AuthKit.Common;
using AuthKit.Features.Auth.Email.Command;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Email.Handler
{
    public class ResendConfirmationEmailCommandHandler<TUser, TKey> : IRequestHandler<ResendConfirmationEmailCommand, Result>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IAuthEmailSender<TUser> _emailSender;
        private readonly ILogger<ResendConfirmationEmailCommandHandler<TUser, TKey>> _logger;
        private readonly IUserAccessGuard<TUser>? _accessGuard;

        public ResendConfirmationEmailCommandHandler(
            UserManager<TUser> userManager,
            IAuthEmailSender<TUser> emailSender,
            ILogger<ResendConfirmationEmailCommandHandler<TUser, TKey>> logger,
            IUserAccessGuard<TUser>? accessGuard = null)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _accessGuard = accessGuard;
        }

        public async Task<Result> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null
                || await _userManager.IsEmailConfirmedAsync(user)
                || (_accessGuard is not null && !_accessGuard.CanSignIn(user)))
                return Result.Success();

            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _emailSender.SendEmailConfirmationAsync(user, UrlTokenEncoder.Encode(token), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "فشل إعادة إرسال رسالة تأكيد البريد.");
            }

            return Result.Success();
        }
    }
}
