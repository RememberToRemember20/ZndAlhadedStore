using AuthKit.Common;
using AuthKit.Features.Auth.Role.Command;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Handler
{
    public class CreateRoleCommandHandler<TRole, TKey> : IRequestHandler<CreateRoleCommand<TRole, TKey>, Result<TKey>>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    {
        private readonly RoleManager<TRole> _roleManager;
        public CreateRoleCommandHandler(RoleManager<TRole> roleManager) => _roleManager = roleManager;

        public async Task<Result<TKey>> Handle(CreateRoleCommand<TRole, TKey> request, CancellationToken cancellationToken)
        {
            if (await _roleManager.RoleExistsAsync(request.Role.Name!))
                return Result<TKey>.Failure("يوجد دور بنفس هذا الاسم بالفعل.");

            var result = await _roleManager.CreateAsync(request.Role);
            return result.Succeeded
                ? Result<TKey>.Success(request.Role.Id)
                : Result<TKey>.Failure(string.Join(" | ", result.Errors.Select(e => e.Description)));
        }
    }
}
