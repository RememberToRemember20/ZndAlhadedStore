using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;

namespace AuthKit.Features.AccessControl.RevokePermissionFromRole
{
    public record RevokePermissionFromRoleCommand(string RoleId, int PermissionId) : IRequest<Result>, IAuditableRequest
    {
        public AuditDescriptor Describe() => new(AuditEvents.RolePermissionRevoke, TargetId: RoleId, Details: $"permission_id={PermissionId}");
    }

}
