using AuthKit.Common;
using AuthKit.Entities;
using AuthKit.Features.AccessControl.AssignPermissionToRole.Command;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security;


namespace AuthKit.Features.AccessControl.AssignPermissionToRole.Handler
{
    internal class AssignPermissionToRoleCommandHandler<TRole, TKey, TContext> : IRequestHandler<AssignPermissionToRoleCommand, Result>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TContext : DbContext
    {
        private readonly RoleManager<TRole> _roleManager;
        private readonly TContext _dbContext;
        private readonly IPrivilegeGuard<TKey> _privilegeGuard;

        public AssignPermissionToRoleCommandHandler(RoleManager<TRole> roleManager, TContext dbContext, IPrivilegeGuard<TKey> privilegeGuard)
        {
            _roleManager = roleManager;
            _dbContext = dbContext;
            _privilegeGuard = privilegeGuard;
        }

        public async Task<Result> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role is null) return Result.Failure(AuthErrorCodes.InvalidRole);

            var permission = await _dbContext.Set<Permission>().SingleOrDefaultAsync(p => p.Id == request.PermissionId, cancellationToken);
            if (permission is null) return Result.Failure(AuthErrorCodes.PermissionNotFound);
            var allowed = await _privilegeGuard.RequireCallerHoldsAsync([permission.Name], cancellationToken);
            if (!allowed.IsSuccess) return allowed;


            var alreadyLinked = await _dbContext.Set<RolePermission<TKey>>()
                .AnyAsync(rp => rp.RoleId.Equals(role.Id) && rp.PermissionId == request.PermissionId, cancellationToken);
            if (alreadyLinked) return Result.Failure(AuthErrorCodes.PermissionAlreadyAssigned);

            _dbContext.Set<RolePermission<TKey>>().Add(new RolePermission<TKey> { RoleId = role.Id, PermissionId = request.PermissionId });
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

