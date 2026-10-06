using MediatR;
using Microsoft.AspNetCore.Identity;
using AuthKit.Common;
using AuthKit.Features.AccessControl.AssignRole.Command;

namespace AuthKit.Features.AccessControl.AssignRole.Handler
{
    public class AssignRoleCommandHandler<TUser, TRole, TKey> : IRequestHandler<AssignRoleCommand, Result>
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    {
        private readonly UserManager<TUser> _userManager;
        private readonly RoleManager<TRole> _roleManager;

        public AssignRoleCommandHandler(UserManager<TUser> userManager, RoleManager<TRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.TargetUserId);
            if (user is null) return Result.Failure("المستخدم غير موجود.");

            if (!await _roleManager.RoleExistsAsync(request.RoleName))
                return Result.Failure("الدور غير موجود.");

            if (await _userManager.IsInRoleAsync(user, request.RoleName))
                return Result.Failure("المستخدم لديه هذا الدور بالفعل.");

            var result = await _userManager.AddToRoleAsync(user, request.RoleName);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(string.Join(" | ", result.Errors.Select(e => e.Description)));
        }
    }
}
