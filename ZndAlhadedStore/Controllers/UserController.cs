using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Features.AccessControl.AssignRole.Command;
using AuthKit.Features.AccessControl.RemoveRoleFromUser.Command;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.Controllers
{
    public record AssignRoleRequest(string RoleName);
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ISender _sender;
        public UserController(ISender sender) => _sender = sender;

     //   [RequirePermission(Permissions.Users.ManageRoles)]
        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> AssignRole(string userId, AssignRoleRequest request)
        {
            var result = await _sender.Send(new AssignRoleCommand(userId, request.RoleName));
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }
        [RequirePermission(Permissions.Users.ManageRoles)]
        [HttpDelete("{userId}/roles/{roleName}")]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            var result = await _sender.Send(new RemoveRoleFromUserCommand(userId, roleName));
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }
        [RequirePermission(Permissions.Users.ViewAll)]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _sender.Send(new GetUsersQuery());
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
    }
}
