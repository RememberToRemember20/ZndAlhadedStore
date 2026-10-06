using AuthKit.Common;
using MediatR;

namespace AuthKit.Features.AccessControl.RevokePermissionFromRole
{
    public record RevokePermissionFromRoleCommand(string RoleId, int PermissionId) : IRequest<Result>;
    
}
