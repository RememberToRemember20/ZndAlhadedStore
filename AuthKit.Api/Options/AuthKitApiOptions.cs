using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Api.Options
{
    public class AuthKitApiOptions
    {
        public bool EnableAuth { get; set; } = true;
        public bool EnableRoles { get; set; } = true;
        public bool EnablePermissions { get; set; } = true;
        public bool EnableUserRoles { get; set; } = true;
        public bool EnableAuditLogs { get; set; } = true;
        public AkRefreshDelivery RefreshTokenDelivery { get; set; } = AkRefreshDelivery.Body;
        public string RefreshCookieName { get; set; } = "__Host-authkit-rt";
        public SameSiteMode RefreshCookieSameSite { get; set; } = SameSiteMode.Strict;

        /// تغيير رمز HTTP لكود معين (يغلب الجدول الافتراضي)
        public IDictionary<string, int> StatusCodeOverrides { get; } = new Dictionary<string, int>();
    }
}
