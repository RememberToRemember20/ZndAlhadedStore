using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Settings
{
    public class AuthKitOptions
    {
       
        public bool SendConfirmationEmailOnRegister { get; set; }

        public int ExpiredTokenRetentionDays { get; set; } = 30;

        /// الفاصل بين عمليات التنظيف. TimeSpan.Zero يعطّل التنظيف.
        public TimeSpan TokenCleanupInterval { get; set; } = TimeSpan.FromHours(6);
        public bool RequireConfirmedEmail { get; set; }
    }
}
