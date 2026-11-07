using AuthKit.Common;
using AuthKit.Features.AccessControl.GetRoles.Query;
using AuthKit.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace AuthKit.Features.AccessControl.GetRoles.Handler
{
    public class GetRolesQueryHandler<TRole, TKey> : IRequestHandler<GetRolesQuery, Result<PagedResult<RoleDto>>>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    {
        private readonly RoleManager<TRole> _roleManager;
        public GetRolesQueryHandler(RoleManager<TRole> roleManager) => _roleManager = roleManager;

        public async Task<Result<PagedResult<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var query = _roleManager.Roles.OrderBy(r => r.Name);

            var total = await query.CountAsync(cancellationToken);
            var roles = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var items = roles.Select(r => new RoleDto(r.Id.ToString()!, r.Name!)).ToList();

            return Result<PagedResult<RoleDto>>.Success(new PagedResult<RoleDto>(items, request.Page, request.PageSize, total));
        }
    }
}
