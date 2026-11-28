using AuthKit.Authorization.PermissionFoldar.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Settings
{
    public class AuthKitOptions
    {
       
        public bool SendConfirmationEmailOnRegister { get; set; }
        public TimeSpan EmailCooldown { get; set; } = TimeSpan.FromSeconds(60);
        public int ExpiredTokenRetentionDays { get; set; } = 30;

        /// الفاصل بين عمليات التنظيف. TimeSpan.Zero يعطّل التنظيف.
        public TimeSpan TokenCleanupInterval { get; set; } = TimeSpan.FromHours(6);
        public bool RequireConfirmedEmail { get; set; }
        public IList<string> CriticalPermissions { get; set; } = new List<string>
         {
             AuthKitPermissions.Roles.Manage,
             AuthKitPermissions.Users.ManageRoles
         };

        public IList<string> ProtectedRoleNames { get; set; } = new List<string>();
    }
}
