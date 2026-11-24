using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Common;
using AuthKit.Features.AccessControl.GetPermissions.Query;
using AuthKit.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.GetRolePermissions.Quey
{
    public record GetRolePermissionsQuery(string RoleId) : IRequest<Result<List<PermissionDto>>>, IPermissionProtectedRequest
    {
        public string RequiredPermission => AuthKitPermissions.Roles.Manage;
    }
}
