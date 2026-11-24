using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;

namespace AuthKit.Features.AccessControl.AssignPermissionToRole.Command
{
    public record AssignPermissionToRoleCommand(string RoleId, int PermissionId) : IRequest<Result>, IAuditableRequest, IPermissionProtectedRequest
    {
        public string RequiredPermission => AuthKitPermissions.Roles.Manage;    
        public AuditDescriptor Describe() => new(AuditEvents.RolePermissionAssign, TargetId: RoleId, Details: $"permission_id={PermissionId}");
    }
}
