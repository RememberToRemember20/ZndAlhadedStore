using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.Auth;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.UserCommandQueryHandler.Role.Query;

namespace ZndAlhadedStore.UserCommandQueryHandler.Role.Handler
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<List<RoleDto>>>
    {
        private readonly AppDbContext _dbContext;
        public GetRolesQueryHandler(AppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _dbContext.Roles
                .Select(r => new RoleDto(r.Id, r.Name!, r.Description))
                .ToListAsync(cancellationToken);

            return Result<List<RoleDto>>.Success(roles);
        }
    }
}
