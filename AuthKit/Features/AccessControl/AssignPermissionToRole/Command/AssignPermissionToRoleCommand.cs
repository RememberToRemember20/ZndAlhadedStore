using AuthKit.Common;
using MediatR;

namespace AuthKit.Features.AccessControl.AssignPermissionToRole.Command
{
    public record AssignPermissionToRoleCommand(string RoleId, int PermissionId) : IRequest<Result>;
    
}
