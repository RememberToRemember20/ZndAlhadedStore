using AuthKit.Common;
using AuthKit.Entities;
using AuthKit.Features.AccessControl.GetPermissions.Query;
using AuthKit.Features.AccessControl.GetRolePermissions.Quey;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.GetRolePermissions.Handler
{
    public class GetRolePermissionsQueryHandler<TRole, TKey, TContext>
     : IRequestHandler<GetRolePermissionsQuery, Result<List<PermissionDto>>>
     where TRole : IdentityRole<TKey>
     where TKey : IEquatable<TKey>
     where TContext : DbContext
    {
        private readonly RoleManager<TRole> _roleManager;
        private readonly TContext _dbContext;

        public GetRolePermissionsQueryHandler(RoleManager<TRole> roleManager, TContext dbContext)
        {
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task<Result<List<PermissionDto>>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role is null) return Result<List<PermissionDto>>.Failure("الدور غير موجود.");

            var permissions = await _dbContext.Set<RolePermission<TKey>>()
                .Where(rp => rp.RoleId.Equals(role.Id))
                .Join(_dbContext.Set<Permission>(), rp => rp.PermissionId, p => p.Id,
                    (rp, p) => new PermissionDto(p.Id, p.Name, p.Description))
                .ToListAsync(cancellationToken);

            return Result<List<PermissionDto>>.Success(permissions);
        }
    }
}
