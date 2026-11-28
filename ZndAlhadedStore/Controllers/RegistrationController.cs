using AuthKit.Api.Controllers;
using AuthKit.Common;
using AuthKit.Features.Auth.Register.Command;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.Controllers
{
    [Route("api/auth/register")]
    public class RegistrationController : AuthKitControllerBase
    {
        public record RegisterRequest(string Email, string Password, string FullName);

        [HttpPost]
        [EnableRateLimiting(AuthKitRateLimitPolicies.Sensitive)]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var user = new ApplicationUser { UserName = request.Email, Email = request.Email, FullName = request.FullName, IsActive = true };
            return Untrusted() ?? FromRegister(await Sender.Send(new RegisterCommand<ApplicationUser, string>(user, request.Password)));
        }
    }
}
