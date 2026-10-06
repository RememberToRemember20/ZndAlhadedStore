using AuthKit.Common;
using AuthKit.Features.AccessControl.GetPermissions.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.GetRolePermissions.Quey
{
    public record GetRolePermissionsQuery(string RoleId) : IRequest<Result<List<PermissionDto>>>;
}
