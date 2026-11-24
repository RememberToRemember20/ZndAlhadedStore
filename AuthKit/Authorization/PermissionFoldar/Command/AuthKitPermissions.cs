using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Authorization.PermissionFoldar.Command
{
    public static class AuthKitPermissions
    {
        public static class Roles { public const string Manage = "Roles.Manage"; }

        public static class Users
        {
            public const string ManageRoles = "Users.ManageRoles";
            public const string Invite = "Users.Invite";
        }

        public static class AuditLogs { public const string View = "AuditLogs.View"; }

        public static IEnumerable<string> GetAll() => typeof(AuthKitPermissions).GetNestedTypes()
            .SelectMany(t => t.GetFields())
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!);
    }
}
