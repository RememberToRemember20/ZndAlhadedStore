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

        /// تغيير رمز HTTP لكود معين (يغلب الجدول الافتراضي)
        public IDictionary<string, int> StatusCodeOverrides { get; } = new Dictionary<string, int>();
    }
}
