using AuthKit.Authorization.PermissionFoldar.Command;

namespace ZndAlhadedStore.Common
{
    public static class Permissions
    {
        public static class Users
        {
            public const string Create = "Users.Create";
            public const string ViewAll = "Users.ViewAll";
            public const string ManageRoles = "Users.ManageRoles";
            public const string SetActive = "Users.SetActive";
            public const string Invite = "Users.Invite";

        }
        public static class Staff
        {
            public const string View = "Staff.View";            // يرى فريقه فقط
            public const string AllTeams = "Staff.AllTeams";    // يرى ويضيف في كل الفرق
        }

        public static class Roles { public const string Manage = AuthKitPermissions.Roles.Manage; }
       public static class ProductsManagement
        {
            public const string Create = "Products.Create";
            public const string ViewAll = "Products.ViewAll";
            public const string Update = "Products.Update";
            public const string Delete = "Products.Delete";
        }
        public static class ProductsEmployee
        {
            public const string ViewAll = "Products.ViewAll";

        }
        public static class AuditLogs { public const string View = AuthKitPermissions.AuditLogs.View; }
        public static IEnumerable<string> GetAll()
        {
            return typeof(Permissions)
                .GetNestedTypes()
                .SelectMany(t => t.GetFields())
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                .Select(f => (string)f.GetRawConstantValue()!);
        }
    }
}
