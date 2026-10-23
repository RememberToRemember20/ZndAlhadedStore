using AuthKit.Common;
using AuthKit.Features.Auth.Email.Command;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Email.Handler
{
    public class ConfirmEmailCommandHandler<TUser, TKey> : IRequestHandler<ConfirmEmailCommand, Result>
     where TUser : IdentityUser<TKey>
     where TKey : IEquatable<TKey>
    {
        private const string InvalidLinkMessage = "رابط التأكيد غير صالح أو منتهي الصلاحية.";
        private readonly UserManager<TUser> _userManager;

        public ConfirmEmailCommandHandler(UserManager<TUser> userManager) => _userManager = userManager;

        public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null || !UrlTokenEncoder.TryDecode(request.Token, out var token))
                return Result.Failure(InvalidLinkMessage);

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded ? Result.Success() : Result.Failure(InvalidLinkMessage);
        }
    }
}
