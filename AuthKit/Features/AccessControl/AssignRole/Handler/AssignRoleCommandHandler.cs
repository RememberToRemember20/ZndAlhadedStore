using MediatR;
using Microsoft.AspNetCore.Identity;
using AuthKit.Common;
using AuthKit.Features.AccessControl.AssignRole.Command;
using AuthKit.Services.Interfaces;

namespace AuthKit.Features.AccessControl.AssignRole.Handler
{
    internal class AssignRoleCommandHandler<TUser, TRole, TKey> : IRequestHandler<AssignRoleCommand, Result>
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly RoleManager<TRole> _roleManager;
        private readonly IPrivilegeGuard<TKey> _privilegeGuard;
        public AssignRoleCommandHandler(UserManager<TUser> userManager, RoleManager<TRole> roleManager, IPrivilegeGuard<TKey> privilegeGuard)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _privilegeGuard = privilegeGuard;
        }

        public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.TargetUserId);
            if (user is null) return Result.Failure(AuthErrorCodes.UserNotFound);

            var role = await _roleManager.FindByNameAsync(request.RoleName);
            if (role is null) return Result.Failure(AuthErrorCodes.InvalidRole);

            var allowed = await _privilegeGuard.RequireCallerDominatesRoleAsync(role.Id, cancellationToken);
            if (!allowed.IsSuccess) return allowed;

            if (await _userManager.IsInRoleAsync(user, request.RoleName))
                return Result.Failure(AuthErrorCodes.UserAlreadyInRole);

            var result = await _userManager.AddToRoleAsync(user, request.RoleName);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(string.Join(" | ", result.Errors.Select(e => e.Description)));
        }
    }
}

