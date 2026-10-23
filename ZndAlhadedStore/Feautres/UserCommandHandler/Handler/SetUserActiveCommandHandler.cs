using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using ZndAlhadedStore.Feautres.UserCommandHandler.Command;
using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.Feautres.UserCommandHandler.Handler
{
    public class SetUserActiveCommandHandler : IRequestHandler<SetUserActiveCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthTokenIssuer<ApplicationUser, string> _tokenIssuer;

        public SetUserActiveCommandHandler(
            UserManager<ApplicationUser> userManager, IAuthTokenIssuer<ApplicationUser, string> tokenIssuer)
        {
            _userManager = userManager;
            _tokenIssuer = tokenIssuer;
        }

        public async Task<Result> Handle(SetUserActiveCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsActive && request.CurrentUserId == request.TargetUserId)
                return Result.Failure("لا يمكنك تعطيل حسابك بنفسك.");

            var user = await _userManager.FindByIdAsync(request.TargetUserId);
            if (user is null) return Result.Failure("المستخدم غير موجود.");

            if (user.IsActive == request.IsActive) return Result.Success();

            user.IsActive = request.IsActive;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Result.Failure(string.Join(" | ", result.Errors.Select(e => e.Description)));

            if (!request.IsActive)
                await _tokenIssuer.RevokeAllActiveTokensAsync(user.Id, cancellationToken);

            return Result.Success();
        }
    }
}
