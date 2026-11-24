using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Settings
{
    internal static class JwtSettingsValidator
    {
        public static IReadOnlyList<string> GetErrors(JwtSettings s)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(s.Secret))
                errors.Add("Jwt:Secret is missing.");
            else
            {
                if (Encoding.UTF8.GetByteCount(s.Secret) < 32)
                    errors.Add("Jwt:Secret must be at least 32 bytes (HS256 needs a 256-bit key).");
                if (s.Secret.Distinct().Count() < 10)
                    errors.Add("Jwt:Secret looks low-entropy; generate it from a cryptographic random source.");
            }

            if (string.IsNullOrWhiteSpace(s.Issuer)) errors.Add("Jwt:Issuer is missing.");
            if (string.IsNullOrWhiteSpace(s.Audience)) errors.Add("Jwt:Audience is missing.");
            if (s.AccessTokenExpirationMinutes is < 1 or > 1440) errors.Add("Jwt:AccessTokenExpirationMinutes must be 1-1440.");
            if (s.RefreshTokenExpirationDays is < 1 or > 365) errors.Add("Jwt:RefreshTokenExpirationDays must be 1-365.");

            return errors;
        }
    }
}

