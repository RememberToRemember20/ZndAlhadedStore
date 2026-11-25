using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Features.AccessControl.Audit.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Api.Controllers
{

    [Route("api/audit-logs")]
    [RequirePermission(AuthKitPermissions.AuditLogs.View)]
    public class AuditLogsController : AuthKitControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAuditLogsQuery query) =>
            FromResult(await Sender.Send(query));
    }
}
