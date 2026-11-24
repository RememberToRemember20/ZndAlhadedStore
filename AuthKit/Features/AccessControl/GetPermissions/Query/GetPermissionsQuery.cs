using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.GetPermissions.Query
{
    public record PermissionDto(int Id, string Name, string? Description);
    public record GetPermissionsQuery : IRequest<Result<List<PermissionDto>>>, IPermissionProtectedRequest
    {
        public string RequiredPermission => AuthKitPermissions.Roles.Manage;
    }
}
