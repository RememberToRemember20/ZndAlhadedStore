using AuthKit.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthKit.Services.Implementations
{
    public class HttpAuditContext : IAuditContext
    {
        private readonly IHttpContextAccessor _accessor;
        private string? _resolvedActor;

        public HttpAuditContext(IHttpContextAccessor accessor) => _accessor = accessor;

        public string? ActorUserId =>
            _accessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? _resolvedActor;

        public string? IpAddress => _accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        public string? UserAgent => _accessor.HttpContext?.Request.Headers.UserAgent.ToString();

        public void SetActor(string userId) => _resolvedActor = userId;
    }
}
