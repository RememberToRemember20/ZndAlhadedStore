using AuthKit.Common;
using AuthKit.Features.Auth.Role.Command;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Handler
{
    internal class UpdateRoleCommandHandler<TRole, TKey> : IRequestHandler<UpdateRoleCommand, Result>
      where TRole : IdentityRole<TKey>
      where TKey : IEquatable<TKey>
    {
        private readonly RoleManager<TRole> _roleManager;
        private readonly IPrivilegeGuard<TKey> _privilegeGuard;
        public UpdateRoleCommandHandler(RoleManager<TRole> roleManager, IPrivilegeGuard<TKey> privilegeGuard)
        {
            _roleManager = roleManager;
            _privilegeGuard = privilegeGuard;
        }

        public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId.ToString()!);
            if (role is null) return Result.Failure(AuthErrorCodes.InvalidRole);
            var protectedCheck = _privilegeGuard.EnsureRoleIsNotProtected(role.Name!);
            if (!protectedCheck.IsSuccess) return protectedCheck;

            var allowed = await _privilegeGuard.RequireCallerDominatesRoleAsync(role.Id, cancellationToken);
            if (!allowed.IsSuccess) return allowed;

            if (string.Equals(role.Name, request.NewName, StringComparison.Ordinal))
                return Result.Success();

            if (await _roleManager.RoleExistsAsync(request.NewName))
                return Result.Failure(AuthErrorCodes.RoleNameTaken);

            var setNameResult = await _roleManager.SetRoleNameAsync(role, request.NewName);
            if (!setNameResult.Succeeded)
                return Result.Failure(string.Join(" | ", setNameResult.Errors.Select(e => e.Description)));

            var updateResult = await _roleManager.UpdateAsync(role);
            return updateResult.Succeeded
                ? Result.Success()
                : Result.Failure(string.Join(" | ", updateResult.Errors.Select(e => e.Description)));
        }
    }
}

