using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Features.AccessControl.Audit.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly ISender _sender;
        public AuditLogsController(ISender sender) => _sender = sender;

        [RequirePermission(Permissions.AuditLogs.View)]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetAuditLogsQuery query)
        {
            var result = await _sender.Send(query);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
    }
}
