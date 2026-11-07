using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;

namespace AuthKit.Features.AccessControl.AssignPermissionToRole.Command
{
    public record AssignPermissionToRoleCommand(string RoleId, int PermissionId) : IRequest<Result>, IAuditableRequest
    {
        public AuditDescriptor Describe() => new(AuditEvents.RolePermissionAssign, TargetId: RoleId, Details: $"permission_id={PermissionId}");
    }
}
