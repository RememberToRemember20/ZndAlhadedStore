using AuthKit.Api.Contracts;
using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Features.AccessControl.AssignRole.Command;
using AuthKit.Features.AccessControl.RemoveRoleFromUser.Command;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Api.Controllers
{

    [Route("api/users/{userId}/roles")]
    [RequirePermission(AuthKitPermissions.Users.ManageRoles)]
    public class UserRolesController : AuthKitControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Assign(string userId, AssignRoleRequest request) =>
            FromResult(await Sender.Send(new AssignRoleCommand(userId, request.RoleName)));

        [HttpDelete("{roleName}")]
        public async Task<IActionResult> Remove(string userId, string roleName) =>
            FromResult(await Sender.Send(new RemoveRoleFromUserCommand(userId, roleName)));
    }   
}
