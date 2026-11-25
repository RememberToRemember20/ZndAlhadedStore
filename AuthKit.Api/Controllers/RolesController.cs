using AuthKit.Api.Contracts;
using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Features.AccessControl.AssignPermissionToRole.Command;
using AuthKit.Features.AccessControl.GetRolePermissions.Quey;
using AuthKit.Features.AccessControl.GetRoles.Query;
using AuthKit.Features.AccessControl.RevokePermissionFromRole;
using AuthKit.Features.Auth.Role.Command;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Api.Controllers
{
    [Route("api/roles")]
    [RequirePermission(AuthKitPermissions.Roles.Manage)]
    public class RolesController : AuthKitControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
            FromResult(await Sender.Send(new GetRolesQuery(page, pageSize)));

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleRequest request) =>
            FromResult(await Sender.Send(new CreateRoleCommand(request.Name)),
                id => StatusCode(StatusCodes.Status201Created, new CreatedResponse(id)));

        [HttpPut("{roleId}")]
        public async Task<IActionResult> Update(string roleId, UpdateRoleRequest request) =>
            FromResult(await Sender.Send(new UpdateRoleCommand(roleId, request.NewName)));

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> Delete(string roleId) =>
            FromResult(await Sender.Send(new DeleteRoleCommand(roleId)));

        [HttpGet("{roleId}/permissions")]
        public async Task<IActionResult> GetPermissions(string roleId) =>
            FromResult(await Sender.Send(new GetRolePermissionsQuery(roleId)));

        [HttpPost("{roleId}/permissions")]
        public async Task<IActionResult> AssignPermission(string roleId, AssignPermissionRequest request) =>
            FromResult(await Sender.Send(new AssignPermissionToRoleCommand(roleId, request.PermissionId)));

        [HttpDelete("{roleId}/permissions/{permissionId:int}")]
        public async Task<IActionResult> RevokePermission(string roleId, int permissionId) =>
            FromResult(await Sender.Send(new RevokePermissionFromRoleCommand(roleId, permissionId)));
    }
}
