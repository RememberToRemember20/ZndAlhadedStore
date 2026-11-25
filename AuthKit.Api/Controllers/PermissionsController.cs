using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Features.AccessControl.GetPermissions.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Api.Controllers
{
    [Route("api/permissions")]
    [RequirePermission(AuthKitPermissions.Roles.Manage)]
    public class PermissionsController : AuthKitControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get() =>
            FromResult(await Sender.Send(new GetPermissionsQuery()));
    }
}
