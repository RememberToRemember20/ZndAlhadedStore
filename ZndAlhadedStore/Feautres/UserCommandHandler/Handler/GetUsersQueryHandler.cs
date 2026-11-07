using AuthKit.Common;
using AuthKit.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Feautres.UserCommandHandler.Query;

namespace ZndAlhadedStore.Feautres.UserCommandHandler.Handler
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PagedResult<UserListItemDto>>>
    {
        private readonly AppDbContext _dbContext;

        public GetUsersQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<PagedResult<UserListItemDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim();
                query = query.Where(u => u.Email!.Contains(term) || u.FullName.Contains(term));
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(u => u.Email).ThenBy(u => u.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UserListItemDto(
                    u.Id,
                    u.Email!,
                    u.FullName,
                    u.IsActive,
                    _dbContext.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_dbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!)
                        .ToList()))
                .ToListAsync(cancellationToken);

            return Result<PagedResult<UserListItemDto>>.Success(
                new PagedResult<UserListItemDto>(items, request.Page, request.PageSize, total));
        }
    }
}
