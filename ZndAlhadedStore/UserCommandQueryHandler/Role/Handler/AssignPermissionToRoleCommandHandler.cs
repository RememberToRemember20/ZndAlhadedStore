using MediatR;
using Microsoft.EntityFrameworkCore;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.Identity;
using ZndAlhadedStore.UserCommandQueryHandler.Role.Coomand;

namespace ZndAlhadedStore.UserCommandQueryHandler.Role.Handler
{
    public class AssignPermissionToRoleCommandHandler : IRequestHandler<AssignPermissionToRoleCommand, Result>
    {
        private readonly AppDbContext _dbContext;
        public AssignPermissionToRoleCommandHandler(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            var roleExists = await _dbContext.Roles.AnyAsync(r => r.Id == request.RoleId, cancellationToken);
            if (!roleExists) return Result.Failure("الدور غير موجود.");

            var permissionExists = await _dbContext.Permissions.AnyAsync(p => p.Id == request.PermissionId, cancellationToken);
            if (!permissionExists) return Result.Failure("الصلاحية غير موجودة.");

            var alreadyLinked = await _dbContext.RolePermissions
                .AnyAsync(rp => rp.RoleId == request.RoleId && rp.PermissionId == request.PermissionId, cancellationToken);
            if (alreadyLinked) return Result.Failure("الصلاحية مرتبطة بالفعل بهذا الدور.");

            _dbContext.RolePermissions.Add(new RolePermission { RoleId = request.RoleId, PermissionId = request.PermissionId });
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
