using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZndAlhadedStore.Auth.Command;
using ZndAlhadedStore.UserCommandQueryHandler.Command;

namespace ZndAlhadedStore.Controllers
{
    public record LogoutRequest(string RefreshToken);
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenCommand command)
        {
            var result = await _sender.Send(command);
            return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutRequest request)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _sender.Send(new LogoutCommand(userId, request.RefreshToken));
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _sender.Send(new LogoutAllCommand(userId));
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

    }
}
