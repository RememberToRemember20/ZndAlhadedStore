using AuthKit.Common;
using AuthKit.Features.AccessControl.GetRoles.Query;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace AuthKit.Features.AccessControl.GetRoles.Handler
{
    public class GetRolesQueryHandler<TRole, TKey> : IRequestHandler<GetRolesQuery, Result<List<RoleDto>>>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    {
        private readonly RoleManager<TRole> _roleManager;
        public GetRolesQueryHandler(RoleManager<TRole> roleManager) => _roleManager = roleManager;

        public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles.ToListAsync(cancellationToken);
            var dtos = roles.Select(r => new RoleDto(r.Id.ToString()!, r.Name!)).ToList();

            return Result<List<RoleDto>>.Success(dtos);
        }
    }
}
