using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Features.AccessControl.AssignRole.Command;
using AuthKit.Features.AccessControl.RemoveRoleFromUser.Command;
using AuthKit.Features.Auth.Email.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.Feautres.UserCommandHandler.Command;
using ZndAlhadedStore.Feautres.UserCommandHandler.Query;

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
       // [RequirePermission(Permissions.Users.ViewAll)]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _sender.Send(new GetUsersQuery());
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
        [EnableRateLimiting("auth-sensitive")]
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommand command)
        {
            var result = await _sender.Send(command);
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        [EnableRateLimiting("auth-sensitive")]
        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation(ResendConfirmationEmailCommand command)
        {
            await _sender.Send(command);
            return Ok(new { message = "إذا كان الحساب موجوداً ولم يُؤكَّد بعد، ستصلك رسالة جديدة." });
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _sender.Send(new GetCurrentUserQuery(userId));
            if (!result.IsSuccess) return Unauthorized();

            return Ok(new
            {
                result.Value!.Id,
                result.Value.Email,
                result.Value.FullName,
                result.Value.EmailConfirmed,
                Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value),
                Permissions = User.FindAll("permission").Select(c => c.Value)
            });
        }
        public record SetUserActiveRequest(bool IsActive);

        [RequirePermission(Permissions.Users.SetActive)]
        [HttpPut("{userId}/active")]
        public async Task<IActionResult> SetActive(string userId, SetUserActiveRequest request)
        {
            var currentUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            var result = await _sender.Send(new SetUserActiveCommand(currentUserId, userId, request.IsActive));
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }
    }
}
