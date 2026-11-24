using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Seeder
{
    public sealed record RoleSeed(string Name, string? Description, IReadOnlyList<string> Permissions, bool AllPermissions);

    public sealed class AuthKitSeedBuilder<TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        internal List<string> PermissionNames { get; } = new();
        internal List<RoleSeed> Roles { get; } = new();
        internal Func<string, TUser>? AdminFactory { get; private set; }
        internal string AdminEmailKey { get; private set; } = "SeedAdmin:Email";
        internal string AdminPasswordKey { get; private set; } = "SeedAdmin:Asd@6323774";
        internal string? AdminRoleName { get; private set; }

        /// لو تطبيقك أضاف حقولاً للدور (مثل Description)
        public Func<string, string?, TRole> RoleFactory { get; set; } = (name, _) => new TRole { Name = name };

        /// كتالوج صلاحيات تطبيقك (صلاحيات المكتبة بتتضاف تلقائياً)
        public AuthKitSeedBuilder<TUser, TRole, TKey> Permissions(IEnumerable<string> names)
        {
            PermissionNames.AddRange(names);
            return this;
        }

        /// دور بكل الصلاحيات دايماً، ومحمي من الحذف وإعادة التسمية
        public AuthKitSeedBuilder<TUser, TRole, TKey> AdminRole(string name, string? description = null)
        {
            Roles.Add(new RoleSeed(name, description, Array.Empty<string>(), AllPermissions: true));
            AdminRoleName = name;
            return this;
        }

        /// دور افتراضي بصلاحيات محددة (تتطبق عند إنشائه فقط)
        public AuthKitSeedBuilder<TUser, TRole, TKey> Role(string name, string? description, params string[] permissions)
        {
            Roles.Add(new RoleSeed(name, description, permissions, AllPermissions: false));
            return this;
        }

        /// المدير الأول. البريد وكلمة المرور من الإعدادات (User Secrets)، ويتخطى لو غير موجودين
        public AuthKitSeedBuilder<TUser, TRole, TKey> InitialAdmin(
            Func<string, TUser> factory, string emailKey = "SeedAdmin:Email", string passwordKey = "SeedAdmin:Asd@6323774")
        {
            AdminFactory = factory;
            AdminEmailKey = emailKey;
            AdminPasswordKey = passwordKey;
            return this;
        }
    }
}
