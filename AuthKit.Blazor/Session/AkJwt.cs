using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AuthKit.Blazor.Session
{
    internal static class AkJwt
    {
        private const string RoleLong = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        public static ClaimsPrincipal? ToPrincipal(string jwt)
        {
            var parts = jwt.Split('.');
            if (parts.Length != 3) return null;

            try
            {
                using var doc = JsonDocument.Parse(Decode(parts[1]));
                var claims = new List<Claim>();

                foreach (var property in doc.RootElement.EnumerateObject())
                {
                    var type = property.Name switch
                    {
                        "role" or RoleLong => ClaimTypes.Role,
                        "sub" => ClaimTypes.NameIdentifier,
                        "email" => ClaimTypes.Email,
                        _ => property.Name
                    };

                    if (property.Value.ValueKind == JsonValueKind.Array)
                        claims.AddRange(property.Value.EnumerateArray().Select(v => new Claim(type, v.ToString())));
                    else
                        claims.Add(new Claim(type, property.Value.ToString()));
                }

                return new ClaimsPrincipal(new ClaimsIdentity(claims, "AuthKit", ClaimTypes.Email, ClaimTypes.Role));
            }
            catch (Exception ex) when (ex is JsonException or FormatException) { return null; }
        }

        private static byte[] Decode(string value)
        {
            var s = value.Replace('-', '+').Replace('_', '/');
            return Convert.FromBase64String(s.PadRight(s.Length + (4 - s.Length % 4) % 4, '='));
        }
    }
}
