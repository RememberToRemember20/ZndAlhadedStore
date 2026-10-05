using MediatR;
using Microsoft.EntityFrameworkCore;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.UserCommandQueryHandler.Role.Coomand;

namespace ZndAlhadedStore.UserCommandQueryHandler.Role.Handler
{
    public class RevokePermissionFromRoleCommandHandler : IRequestHandler<RevokePermissionFromRoleCommand, Result>
    {
        private readonly AppDbContext _dbContext;
        public RevokePermissionFromRoleCommandHandler(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result> Handle(RevokePermissionFromRoleCommand request, CancellationToken cancellationToken)
        {
            var link = await _dbContext.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == request.RoleId && rp.PermissionId == request.PermissionId, cancellationToken);

            if (link is null) return Result.Success();   // Idempotent، نفس فلسفة Logout

            _dbContext.RolePermissions.Remove(link);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
