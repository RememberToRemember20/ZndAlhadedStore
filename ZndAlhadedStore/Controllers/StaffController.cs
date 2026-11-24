using AuthKit.Api.Controllers;
using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Features.Auth.InventUser.Command;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.Entity;
using ZndAlhadedStore.Feautres.Staff.Query;
using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.Controllers
{
    public record InviteStaffRequest(
    string Email, string FullName, string[] Roles,
    string? JobTitle, string? Department, DateOnly HireDate, string? ReportsToId);

    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : AuthKitControllerBase
    {  private readonly ISender _sender;
        public StaffController(ISender sender) => _sender = sender;

        [RequirePermission(Permissions.Users.Invite)]
        [EnableRateLimiting("auth-moderate")]
        [HttpPost]
        public async Task<IActionResult> Invite(InviteStaffRequest request)
        {
            var callerId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(callerId)) return Unauthorized();

            // من غير صلاحية "كل الفرق" يضيف تحت نفسه فقط
            var reportsTo = User.HasClaim("permission", Permissions.Staff.AllTeams)
                ? request.ReportsToId
                : callerId;

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                IsActive = true,
                EmailConfirmed = true,      // استلامه رابط الدعوة هو إثبات ملكية البريد
                CreatedAt = DateTime.UtcNow,
                StaffProfile = new StaffProfile
                {
                    JobTitle = request.JobTitle,
                    Department = request.Department,
                    HireDate = request.HireDate,
                    ReportsToId = reportsTo
                }
            };

            var result = await _sender.Send(
                new InviteUserCommand<ApplicationUser, string>(user, request.Roles ?? [], callerId));

            return FromResult(result);
        }

        [RequirePermission(Permissions.Staff.View)]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? role, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var callerId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(callerId)) return Unauthorized();

            var result = await _sender.Send(new GetStaffQuery(
                callerId, User.HasClaim("permission", Permissions.Staff.AllTeams), role, page, pageSize));

            return FromResult(result);  
        }
    }
}
