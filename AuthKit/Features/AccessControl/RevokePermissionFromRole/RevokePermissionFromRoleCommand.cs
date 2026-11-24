using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;

namespace AuthKit.Features.AccessControl.RevokePermissionFromRole
{
    public record RevokePermissionFromRoleCommand(string RoleId, int PermissionId) : IRequest<Result>, IAuditableRequest, IPermissionProtectedRequest
    {
        public string RequiredPermission => AuthKitPermissions.Roles.Manage;
    
        public AuditDescriptor Describe() => new(AuditEvents.RolePermissionRevoke, TargetId: RoleId, Details: $"permission_id={PermissionId}");
    }

}
