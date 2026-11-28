using AuthKit.Api.Contracts;
using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Common;
using AuthKit.Features.AccessControl.AssignPermissionToRole.Command;
using AuthKit.Features.AccessControl.AssignRole.Command;
using AuthKit.Features.AccessControl.Audit.Query;
using AuthKit.Features.AccessControl.GetPermissions.Query;
using AuthKit.Features.AccessControl.GetRolePermissions.Quey;
using AuthKit.Features.AccessControl.GetRoles.Query;
using AuthKit.Features.AccessControl.RemoveRoleFromUser.Command;
using AuthKit.Features.AccessControl.RevokePermissionFromRole;
using AuthKit.Features.Auth.Email.Command;
using AuthKit.Features.Auth.Login.Command;
using AuthKit.Features.Auth.Logout.Command;
using AuthKit.Features.Auth.LogoutAll.Command;
using AuthKit.Features.Auth.Password.Command;
using AuthKit.Features.Auth.RefreshToken.Command;
using AuthKit.Features.User.Query;
using AuthKit.Features.Auth.Role.Command;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthKit.Api.Controllers
{

    [Route("api/auth")]
    public class AuthController : AuthKitControllerBase
    {
        private string? CallerId => User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        [HttpPost("login")]
        [EnableRateLimiting(AuthKitRateLimitPolicies.Sensitive)]
        public async Task<IActionResult> Login(LoginCommand command) =>
       Untrusted() ?? FromTokens(await Sender.Send(command));
        [HttpPost("refresh-token")]
        [EnableRateLimiting(AuthKitRateLimitPolicies.Moderate)]
        public async Task<IActionResult> Refresh(RefreshRequest request)
        {
            if (Untrusted() is { } rejected) return rejected;

            var useCookie = Cookies.CookieRequested(HttpContext);
            var token = useCookie ? Cookies.Read(HttpContext) : request.RefreshToken;

            if (useCookie && token is null)
                return Failure(Result.Failure(AuthErrorCodes.InvalidToken));

            var result = await Sender.Send(new RefreshTokenCommand(request.AccessToken, token ?? string.Empty));

            if (useCookie && !result.IsSuccess && IsSessionEnding(result.ErrorCode))
                Cookies.Clear(HttpContext);   // كوكي ميت: امسحه عشان ما يكررش الفشل

            return FromTokens(result);
        }
        [Authorize, HttpPost("logout")]
        [EnableRateLimiting(AuthKitRateLimitPolicies.Moderate)]
        public async Task<IActionResult> Logout(LogoutRequest request)
        {
            if (Untrusted() is { } rejected) return rejected;
            if (CallerId is not { } id) return Unauthorized();

            var useCookie = Cookies.CookieRequested(HttpContext);
            var token = useCookie ? Cookies.Read(HttpContext) : request.RefreshToken;

            var result = string.IsNullOrEmpty(token) ? Result.Success() : await Sender.Send(new LogoutCommand(id, token));
            if (useCookie) Cookies.Clear(HttpContext);

            return FromResult(result);
        }

        [Authorize, HttpPost("logout-all")]
        [EnableRateLimiting(AuthKitRateLimitPolicies.Moderate)]
        public async Task<IActionResult> LogoutAll()
        {
            if (Untrusted() is { } rejected) return rejected;
            if (CallerId is not { } id) return Unauthorized();

            var result = await Sender.Send(new LogoutAllCommand(id));
            if (Cookies.CookieRequested(HttpContext)) Cookies.Clear(HttpContext);

            return FromResult(result);
        }
        [HttpPost("forgot-password")]
        [EnableRateLimiting(AuthKitRateLimitPolicies.Sensitive)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command) =>
            FromResult(await Sender.Send(command));

        [HttpPost("reset-password")]
        [EnableRateLimiting(AuthKitRateLimitPolicies.Sensitive)]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command) =>
            FromResult(await Sender.Send(command));
        [Authorize, HttpPost("change-password")]
        [EnableRateLimiting(AuthKitRateLimitPolicies.Sensitive)]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            if (Untrusted() is { } rejected) return rejected;
            if (CallerId is not { } id) return Unauthorized();

            return FromTokens(await Sender.Send(new ChangePasswordCommand(id, request.CurrentPassword, request.NewPassword)));
        }

        private static bool IsSessionEnding(string? code) => code is
            AuthErrorCodes.InvalidToken or AuthErrorCodes.TokenReuseDetected or
            AuthErrorCodes.SessionExpired or AuthErrorCodes.AccountUnavailable;
        [HttpPost("confirm-email")]
        [EnableRateLimiting(AuthKitRateLimitPolicies.Sensitive)]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommand command) =>
            FromResult(await Sender.Send(command));

        [HttpPost("resend-confirmation")]
        [EnableRateLimiting(AuthKitRateLimitPolicies.Sensitive)]
        public async Task<IActionResult> ResendConfirmation(ResendConfirmationEmailCommand command) =>
            FromResult(await Sender.Send(command));

        [Authorize, HttpGet("me")]
        public async Task<IActionResult> Me() =>
            FromResult(await Sender.Send(new GetCurrentUserQuery()));
    }
}
