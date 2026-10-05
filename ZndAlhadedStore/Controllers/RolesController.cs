using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.PermissionFoldar.Command;
using ZndAlhadedStore.PermissionFoldar.Query;
using ZndAlhadedStore.UserCommandQueryHandler.Role.Coomand;
using ZndAlhadedStore.UserCommandQueryHandler.Role.Query;

namespace ZndAlhadedStore.Controllers
{
    public record AssignPermissionRequest(int PermissionId);
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ISender _sender;
        public RolesController(ISender sender) => _sender = sender;

        [RequirePermission(Permissions.Roles.Manage)]
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _sender.Send(new GetRolesQuery());
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [RequirePermission(Permissions.Roles.Manage)]
        [HttpGet("{roleId}/permissions")]
        public async Task<IActionResult> GetRolePermissions(string roleId)
        {
            var result = await _sender.Send(new GetRolePermissionsQuery(roleId));
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [RequirePermission(Permissions.Roles.Manage)]
        [HttpPost("{roleId}/permissions")]
        public async Task<IActionResult> AssignPermission(string roleId, AssignPermissionRequest request)
        {
            var result = await _sender.Send(new AssignPermissionToRoleCommand(roleId, request.PermissionId));
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        [RequirePermission(Permissions.Roles.Manage)]
        [HttpDelete("{roleId}/permissions/{permissionId}")]
        public async Task<IActionResult> RevokePermission(string roleId, int permissionId)
        {
            var result = await _sender.Send(new RevokePermissionFromRoleCommand(roleId, permissionId));
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

    }
}
