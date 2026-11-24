using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Command
{
    internal class DeleteRoleCommandHandler<TUser, TRole, TKey> : IRequestHandler<DeleteRoleCommand, Result>
     where TUser : IdentityUser<TKey>
     where TRole : IdentityRole<TKey>
     where TKey : IEquatable<TKey>
    {
        private readonly RoleManager<TRole> _roleManager;
        private readonly UserManager<TUser> _userManager;
        private readonly IPrivilegeGuard<TKey> _privilegeGuard;

        public DeleteRoleCommandHandler(RoleManager<TRole> roleManager, UserManager<TUser> userManager, IPrivilegeGuard<TKey> privilegeGuard)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _privilegeGuard = privilegeGuard;
        }

        public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId.ToString()!);
            if (role is null) return Result.Success();   // Idempotent
            var protectedCheck = _privilegeGuard.EnsureRoleIsNotProtected(role.Name!);
            if (!protectedCheck.IsSuccess) return protectedCheck;

            var allowed = await _privilegeGuard.RequireCallerDominatesRoleAsync(role.Id, cancellationToken);
            if (!allowed.IsSuccess) return allowed;
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            if (usersInRole.Count > 0)
                return Result.Failure(AuthErrorCodes.RoleInUse);

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(string.Join(" | ", result.Errors.Select(e => e.Description)));
        }
    }
}

