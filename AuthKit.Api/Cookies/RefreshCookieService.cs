using AuthKit.Api.Options;
using AuthKit.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Api.Cookies
{
    internal sealed class RefreshCookieService
    {
        public const string DeliveryHeader = "X-AuthKit-Token-Delivery";

        private readonly AuthKitApiOptions _options;
        private readonly AuthKitCorsOptions _cors;
        private readonly JwtSettings _jwt;
        private readonly TimeProvider _time;

        public RefreshCookieService(IOptions<AuthKitApiOptions> options, IOptions<AuthKitCorsOptions> cors,
            IOptions<JwtSettings> jwt, TimeProvider time)
        {
            _options = options.Value; _cors = cors.Value; _jwt = jwt.Value; _time = time;
        }

        public bool CookieRequested(HttpContext ctx) => _options.RefreshTokenDelivery switch
        {
            AkRefreshDelivery.Cookie => true,
            AkRefreshDelivery.Both => ctx.Request.Headers[DeliveryHeader] == "cookie",
            _ => false
        };

        /// الهيدر المخصص يفرض Preflight على أي موقع تاني، والأصل لازم يكون موثوقاً (Defense in depth فوق SameSite)
        public bool IsTrustedRequest(HttpContext ctx)
        {
            if (ctx.Request.Headers[DeliveryHeader] != "cookie") return false;

            var origin = ctx.Request.Headers.Origin.ToString();
            if (origin.Length == 0) return false;

            var own = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
            return string.Equals(origin, own, StringComparison.OrdinalIgnoreCase)
                || _cors.AllowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
        }

        public string? Read(HttpContext ctx) =>
            ctx.Request.Cookies.TryGetValue(_options.RefreshCookieName, out var value) && !string.IsNullOrEmpty(value) ? value : null;

        public void Write(HttpContext ctx, string refreshToken) =>
            ctx.Response.Cookies.Append(_options.RefreshCookieName, refreshToken,
                Build(_time.GetUtcNow().AddDays(_jwt.RefreshTokenExpirationDays)));

        public void Clear(HttpContext ctx) => ctx.Response.Cookies.Delete(_options.RefreshCookieName, Build(null));

        private CookieOptions Build(DateTimeOffset? expires) => new()
        {
            HttpOnly = true,
            Secure = true,
            Path = "/",
            IsEssential = true,
            SameSite = _options.RefreshCookieSameSite,
            Expires = expires
        };
    }
}
