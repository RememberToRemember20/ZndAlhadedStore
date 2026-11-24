using AuthKit.Common;
using AuthKit.Entities;
using AuthKit.Features.AccessControl.AssignPermissionToRole.Command;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.RevokePermissionFromRole
{
    internal class RevokePermissionFromRoleCommandHandler<TRole, TKey, TContext> : IRequestHandler<RevokePermissionFromRoleCommand, Result>
     where TRole : IdentityRole<TKey>
     where TKey : IEquatable<TKey>
     where TContext : DbContext
    {
        private readonly RoleManager<TRole> _roleManager;
        private readonly IPrivilegeGuard<TKey> _privilegeGuard;
        private readonly TContext _dbContext;

        public RevokePermissionFromRoleCommandHandler(RoleManager<TRole> roleManager, TContext dbContext, IPrivilegeGuard<TKey> privilegeGuard)
        {
            _roleManager = roleManager;
            _dbContext = dbContext;
            _privilegeGuard = privilegeGuard;
        }

        public async Task<Result> Handle(RevokePermissionFromRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role is null) return Result.Success();

            var permission = await _dbContext.Set<Permission>().SingleOrDefaultAsync(p => p.Id == request.PermissionId, cancellationToken);
            if (permission is null) return Result.Success();

            var allowed = await _privilegeGuard.RequireCallerHoldsAsync([permission.Name], cancellationToken);
            if (!allowed.IsSuccess) return allowed;

            var safe = await _privilegeGuard.EnsureNotOrphaningPermissionAsync(role.Id, permission.Name, cancellationToken);
            if (!safe.IsSuccess) return safe;

            var link = await _dbContext.Set<RolePermission<TKey>>()
                .FirstOrDefaultAsync(rp => rp.RoleId.Equals(role.Id) && rp.PermissionId == request.PermissionId, cancellationToken);
            if (link is null) return Result.Success();

            _dbContext.Set<RolePermission<TKey>>().Remove(link);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}

