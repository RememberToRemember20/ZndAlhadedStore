using AuthKit.Common;
using AuthKit.Features.AccessControl.RemoveRoleFromUser.Command;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.RemoveRoleFromUser.Handler
{
    public class RemoveRoleFromUserCommandHandler<TUser, TKey> : IRequestHandler<RemoveRoleFromUserCommand, Result>
     where TUser : IdentityUser<TKey>
     where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        public RemoveRoleFromUserCommandHandler(UserManager<TUser> userManager) => _userManager = userManager;

        public async Task<Result> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.TargetUserId);
            if (user is null) return Result.Failure("المستخدم غير موجود.");

            if (!await _userManager.IsInRoleAsync(user, request.RoleName))
                return Result.Success();   // Idempotent

            var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(string.Join(" | ", result.Errors.Select(e => e.Description)));
        }
    }
}
