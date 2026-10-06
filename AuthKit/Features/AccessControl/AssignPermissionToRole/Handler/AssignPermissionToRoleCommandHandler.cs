using AuthKit.Common;
using AuthKit.Entities;
using AuthKit.Features.AccessControl.AssignPermissionToRole.Command;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace AuthKit.Features.AccessControl.AssignPermissionToRole.Handler
{
    public class AssignPermissionToRoleCommandHandler<TRole, TKey, TContext> : IRequestHandler<AssignPermissionToRoleCommand, Result>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TContext : DbContext
    {
        private readonly RoleManager<TRole> _roleManager;
        private readonly TContext _dbContext;

        public AssignPermissionToRoleCommandHandler(RoleManager<TRole> roleManager, TContext dbContext)
        {
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task<Result> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId);
            if (role is null) return Result.Failure("الدور غير موجود.");

            var permissionExists = await _dbContext.Set<Permission>().AnyAsync(p => p.Id == request.PermissionId, cancellationToken);
            if (!permissionExists) return Result.Failure("الصلاحية غير موجودة.");

            var alreadyLinked = await _dbContext.Set<RolePermission<TKey>>()
                .AnyAsync(rp => rp.RoleId.Equals(role.Id) && rp.PermissionId == request.PermissionId, cancellationToken);
            if (alreadyLinked) return Result.Failure("الصلاحية مرتبطة بالفعل بهذا الدور.");

            _dbContext.Set<RolePermission<TKey>>().Add(new RolePermission<TKey> { RoleId = role.Id, PermissionId = request.PermissionId });
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
