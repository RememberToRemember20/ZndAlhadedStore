using AuthKit.Common;
using MediatR;

namespace AuthKit.Features.AccessControl.GetRoles.Query
{
    public record RoleDto(string Id, string Name);
    public record GetRolesQuery : IRequest<Result<List<RoleDto>>>;
}
