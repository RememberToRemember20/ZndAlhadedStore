using AuthKit.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Feautres.UserCommandHandler.Query;

namespace ZndAlhadedStore.Feautres.UserCommandHandler.Handler
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<List<UserListItemDto>>>
    {
        private readonly AppDbContext _dbContext;

        public GetUsersQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<UserListItemDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _dbContext.Users
                .Select(u => new UserListItemDto(
                    u.Id,
                    u.Email!,
                    u.FullName,
                    u.IsActive,
                    _dbContext.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_dbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!)
                        .ToList()
                ))
                .ToListAsync(cancellationToken);

            return Result<List<UserListItemDto>>.Success(users);
        }
    }
}
