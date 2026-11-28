using AuthKit.Common;
using AuthKit.Features.Auth.Email.Command;
using AuthKit.Services.Interfaces;
using AuthKit.Settings;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Email.Handler
{
    internal class ResendConfirmationEmailCommandHandler<TUser, TKey> : IRequestHandler<ResendConfirmationEmailCommand, Result>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IAuthEmailSender<TUser> _emailSender;
        private readonly ILogger<ResendConfirmationEmailCommandHandler<TUser, TKey>> _logger;
        private readonly IUserAccessGuard<TUser>? _accessGuard;
        private readonly IMemoryCache _cache;
        private readonly IOptions<AuthKitOptions> _options;

        public ResendConfirmationEmailCommandHandler(
            UserManager<TUser> userManager,
            IAuthEmailSender<TUser> emailSender,
            ILogger<ResendConfirmationEmailCommandHandler<TUser, TKey>> logger
            , IMemoryCache cache, IOptions<AuthKitOptions> options,
            IUserAccessGuard<TUser>? accessGuard = null)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _accessGuard = accessGuard;
            _cache = cache;
            _options = options;
        }

        public async Task<Result> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            var key = $"ak:forgot:{request.Email.Trim().ToUpperInvariant()}";   // "ak:resend:" في التاني
            if (_cache.TryGetValue(key, out _)) return Result.Success();
            _cache.Set(key, true, _options.Value.EmailCooldown);
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

