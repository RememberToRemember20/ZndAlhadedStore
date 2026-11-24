using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;

namespace AuthKit.Features.AccessControl.AssignRole.Command
{
    public record AssignRoleCommand(string TargetUserId, string RoleName) : IRequest<Result>, IAuditableRequest, IPermissionProtectedRequest
    {
        public string RequiredPermission => AuthKitPermissions.Users.ManageRoles;
        public AuditDescriptor Describe() => new(AuditEvents.UserRoleAssign, TargetId: TargetUserId, Details: $"role={RoleName}");
    }
}
