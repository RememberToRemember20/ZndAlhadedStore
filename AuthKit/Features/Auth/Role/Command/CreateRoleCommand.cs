using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Command
{
    internal record CreateRoleCommand(string Name) : IRequest<Result<string>>, IAuditableRequest, IPermissionProtectedRequest
    {
        public string RequiredPermission => AuthKitPermissions.Roles.Manage;
        public AuditDescriptor Describe() => new(AuditEvents.RoleCreate, Details: $"name={Name}");
    }
}
