using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Features.AccessControl.AssignPermissionToRole.Command;
using AuthKit.Features.AccessControl.GetRolePermissions.Quey;
using AuthKit.Features.AccessControl.GetRoles.Query;
using AuthKit.Features.AccessControl.RevokePermissionFromRole;
using AuthKit.Features.Auth.Role.Command;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.Controllers
{
    public record CreateRoleRequest(string Name, string? Description);
    public record UpdateRoleRequest(string NewName);
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
        [RequirePermission(Permissions.Roles.Manage)]
        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateRole(string roleId, UpdateRoleRequest request)
        {
            var result = await _sender.Send(new UpdateRoleCommand<ApplicationRole, string>(roleId, request.NewName));
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        [RequirePermission(Permissions.Roles.Manage)]
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var result = await _sender.Send(new DeleteRoleCommand<string>(roleId));
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }
        [RequirePermission(Permissions.Roles.Manage)]
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleRequest request)
        {
            var role = new ApplicationRole { Name = request.Name, Description = request.Description };
            var result = await _sender.Send(new CreateRoleCommand<ApplicationRole, string>(role));
            return result.IsSuccess ? Ok(new { RoleId = result.Value }) : BadRequest(result.Error);
        }

    }
}
