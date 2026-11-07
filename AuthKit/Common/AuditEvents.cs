using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Common
{
    public static class AuditEvents
    {
        public const string Register = "auth.register";
        public const string Login = "auth.login";
        public const string Refresh = "auth.refresh";
        public const string Logout = "auth.logout";
        public const string LogoutAll = "auth.logout_all";
        public const string PasswordForgot = "auth.password_forgot";
        public const string PasswordReset = "auth.password_reset";
        public const string PasswordChange = "auth.password_change";
        public const string EmailConfirm = "auth.email_confirm";
        public const string RoleCreate = "role.create";
        public const string RoleUpdate = "role.update";
        public const string RoleDelete = "role.delete";
        public const string RolePermissionAssign = "role.permission_assign";
        public const string RolePermissionRevoke = "role.permission_revoke";
        public const string UserRoleAssign = "user.role_assign";
        public const string UserRoleRemove = "user.role_remove";
    }
}
