using AuthKit.Common;
using AuthKit.Entities;
using AuthKit.Features.AccessControl.AssignPermissionToRole.Command;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.RevokePermissionFromRole
{
    public class RevokePermissionFromRoleCommandHandler<TRole, TKey, TContext> : IRequestHandler<RevokePermissionFromRoleCommand, Result>
     where TRole : IdentityRole<TKey>
     where TKey : IEquatable<TKey>
     where TContext : DbContext
    {
        private readonly RoleManager<TRole> _roleManager;
        private readonly TContext _dbContext;

        public RevokePermissionFromRoleCommandHandler(RoleManager<TRole> roleManager, TContext dbContext)
        {
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(RevokePermissionFromRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role is null) return Result.Success();   // Idempotent، زي Logout بالظبط

            var link = await _dbContext.Set<RolePermission<TKey>>()
                .FirstOrDefaultAsync(rp => rp.RoleId.Equals(role.Id) && rp.PermissionId == request.PermissionId, cancellationToken);

            if (link is null) return Result.Success();

            _dbContext.Set<RolePermission<TKey>>().Remove(link);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
