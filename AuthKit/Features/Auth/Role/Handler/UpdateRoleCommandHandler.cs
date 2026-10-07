using AuthKit.Common;
using AuthKit.Features.Auth.Role.Command;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Handler
{
    public class UpdateRoleCommandHandler<TRole, TKey> : IRequestHandler<UpdateRoleCommand<TRole, TKey>, Result>
      where TRole : IdentityRole<TKey>
      where TKey : IEquatable<TKey>
    {
        private readonly RoleManager<TRole> _roleManager;
        public UpdateRoleCommandHandler(RoleManager<TRole> roleManager) => _roleManager = roleManager;

        public async Task<Result> Handle(UpdateRoleCommand<TRole, TKey> request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId.ToString()!);
            if (role is null) return Result.Failure("الدور غير موجود.");

            if (string.Equals(role.Name, request.NewName, StringComparison.Ordinal))
                return Result.Success();

            if (await _roleManager.RoleExistsAsync(request.NewName))
                return Result.Failure("يوجد دور بنفس هذا الاسم بالفعل.");

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
