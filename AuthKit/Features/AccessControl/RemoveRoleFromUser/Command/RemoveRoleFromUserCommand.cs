using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.RemoveRoleFromUser.Command
{
    public record RemoveRoleFromUserCommand(string TargetUserId, string RoleName) : IRequest<Result>, IAuditableRequest, IPermissionProtectedRequest
    {
        public string RequiredPermission => AuthKitPermissions.Users.ManageRoles;
    
        public AuditDescriptor Describe() => new(AuditEvents.UserRoleRemove, TargetId: TargetUserId, Details: $"role={RoleName}");
    }
}
