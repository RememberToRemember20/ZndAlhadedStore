using AuthKit.Common;
using AuthKit.Models;
using MediatR;

namespace AuthKit.Features.AccessControl.GetRoles.Query
{
    public record RoleDto(string Id, string Name);
    public record GetRolesQuery(int Page = 1, int PageSize = 20) : IRequest<Result<PagedResult<RoleDto>>>;

}
