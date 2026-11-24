using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthKit.Services.Implementations
{
    internal class HttpCurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _accessor;
        public HttpCurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

        private ClaimsPrincipal? Principal => _accessor.HttpContext?.User;

        public string? UserId => Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub);
        public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;
        public bool IsSystem => false;
        public bool HasPermission(string permission) => Principal?.HasClaim(AuthKitClaims.Permission, permission) == true;
    }
}

