using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Auth;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.PermissionFoldar.Query;

namespace ZndAlhadedStore.PermissionFoldar.Handler
{
    public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, Result<List<PermissionDto>>>
    {
        private readonly AppDbContext _dbContext;
        public GetPermissionsQueryHandler(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result<List<PermissionDto>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _dbContext.Permissions
                .Select(p => new PermissionDto(p.Id, p.Name, p.Description))
                .ToListAsync(cancellationToken);

            return Result<List<PermissionDto>>.Success(permissions);
        }
    }

    public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, Result<List<PermissionDto>>>
    {
        private readonly AppDbContext _dbContext;
        public GetRolePermissionsQueryHandler(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result<List<PermissionDto>>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
        {
            var roleExists = await _dbContext.Roles.AnyAsync(r => r.Id == request.RoleId, cancellationToken);
            if (!roleExists)
                return Result<List<PermissionDto>>.Failure("الدور غير موجود.");

            var permissions = await _dbContext.RolePermissions
                .Where(rp => rp.RoleId == request.RoleId)
                .Join(_dbContext.Permissions, rp => rp.PermissionId, p => p.Id,
                    (rp, p) => new PermissionDto(p.Id, p.Name, p.Description))
                .ToListAsync(cancellationToken);

            return Result<List<PermissionDto>>.Success(permissions);
        }
    }
}
