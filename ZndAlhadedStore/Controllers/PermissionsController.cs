using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.PermissionFoldar.Command;
using ZndAlhadedStore.PermissionFoldar.Query;

namespace ZndAlhadedStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly ISender _sender;
        public PermissionsController(ISender sender) => _sender = sender;

        [RequirePermission(Permissions.Roles.Manage)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _sender.Send(new GetPermissionsQuery());
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
    }
}
