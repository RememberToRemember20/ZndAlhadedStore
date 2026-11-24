using AuthKit.Common;
using AuthKit.Extensions;
using AuthKit.Features.Auth.Role.Command;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Handler
{
    internal sealed class CreateRoleCommandHandler<TRole, TKey> : IRequestHandler<CreateRoleCommand, Result<string>>
    where TRole : IdentityRole<TKey>, new()
    where TKey : IEquatable<TKey>
    {
        private readonly RoleManager<TRole> _roleManager;
        public CreateRoleCommandHandler(RoleManager<TRole> roleManager) => _roleManager = roleManager;

        public async Task<Result<string>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            if (await _roleManager.RoleExistsAsync(request.Name))
                return Result<string>.Failure(AuthErrorCodes.RoleNameTaken);

            var role = new TRole { Name = request.Name };
            var result = await _roleManager.CreateAsync(role);

            return result.Succeeded
                ? Result<string>.Success(role.Id.ToString()!)
                : result.ToFailure<string>();
        }
    }
}

