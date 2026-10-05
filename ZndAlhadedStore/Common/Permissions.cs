namespace ZndAlhadedStore.Common
{
    public static class Permissions
    {
        public static class Users
        {
            public const string Create = "Users.Create";
            public const string ViewAll = "Users.ViewAll";
            public const string ManageRoles = "Users.ManageRoles";
        }
        public static class Roles
        {
            public const string Manage = "Roles.Manage";
        }
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
