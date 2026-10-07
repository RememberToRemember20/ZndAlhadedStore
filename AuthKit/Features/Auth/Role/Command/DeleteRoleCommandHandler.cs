using AuthKit.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Command
{
    public class DeleteRoleCommandHandler<TUser, TRole, TKey> : IRequestHandler<DeleteRoleCommand<TKey>, Result>
     where TUser : IdentityUser<TKey>
     where TRole : IdentityRole<TKey>
     where TKey : IEquatable<TKey>
    {
        private readonly RoleManager<TRole> _roleManager;
        private readonly UserManager<TUser> _userManager;

        public DeleteRoleCommandHandler(RoleManager<TRole> roleManager, UserManager<TUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<Result> Handle(DeleteRoleCommand<TKey> request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId.ToString()!);
            if (role is null) return Result.Success();   // Idempotent

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            if (usersInRole.Count > 0)
                return Result.Failure($"لا يمكن حذف الدور لأنه مرتبط بـ {usersInRole.Count} مستخدم. أزل الدور منهم أولاً.");

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(string.Join(" | ", result.Errors.Select(e => e.Description)));
        }
    }
}
