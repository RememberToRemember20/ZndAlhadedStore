using MediatR;
using Microsoft.AspNetCore.Identity;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.Identity;
using ZndAlhadedStore.UserCommandQueryHandler.Role.Coomand;

namespace ZndAlhadedStore.UserCommandQueryHandler.Role.Handler
{
    public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AssignRoleCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.TargetUserId);
            if (user is null)
                return Result.Failure("المستخدم غير موجود.");

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
