using AuthKit.Common;
using AuthKit.Features.AccessControl.RemoveRoleFromUser.Command;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.RemoveRoleFromUser.Handler
{
    internal class RemoveRoleFromUserCommandHandler<TUser, TRole, TKey> : IRequestHandler<RemoveRoleFromUserCommand, Result>
     where TUser : IdentityUser<TKey>
         where TRole : IdentityRole<TKey>
     where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IPrivilegeGuard<TKey> _privilegeGuard;
        private readonly RoleManager<TRole> _roleManager;
        public RemoveRoleFromUserCommandHandler(UserManager<TUser> userManager, IPrivilegeGuard<TKey> privilegeGuard, RoleManager<TRole> roleManager)
        {
            _userManager = userManager;
            _privilegeGuard = privilegeGuard;
            _roleManager = roleManager;
        }

        public async Task<Result> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.TargetUserId);
            if (user is null) return Result.Failure(AuthErrorCodes.UserNotFound);

            var role = await _roleManager.FindByNameAsync(request.RoleName);
            if (role is null) return Result.Success();

            // الفحص قبل الرد "العملية متكررة"، عشان غير المصرّح له ما يقدرش يستكشف
            var allowed = await _privilegeGuard.RequireCallerDominatesRoleAsync(role.Id, cancellationToken);
            if (!allowed.IsSuccess) return allowed;

            if (!await _userManager.IsInRoleAsync(user, role.Name!))
                return Result.Success();

            var safe = await _privilegeGuard.EnsureNotRemovingLastHolderAsync(user.Id, role.Id, cancellationToken);
            if (!safe.IsSuccess) return safe;

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(string.Join(" | ", result.Errors.Select(e => e.Description)));
        }
    }
}

