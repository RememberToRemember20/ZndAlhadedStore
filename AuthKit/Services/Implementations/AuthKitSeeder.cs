using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Entities;
using AuthKit.Seeder;
using AuthKit.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Implementations
{
    internal sealed class AuthKitSeeder<TUser, TRole, TKey, TContext> : IAuthKitSeeder
      where TUser : IdentityUser<TKey>
      where TRole : IdentityRole<TKey>, new()
      where TKey : IEquatable<TKey>
      where TContext : DbContext
    {
        private readonly AuthKitSeedBuilder<TUser, TRole, TKey> _seed;
        private readonly TContext _db;
        private readonly RoleManager<TRole> _roles;
        private readonly UserManager<TUser> _users;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthKitSeeder<TUser, TRole, TKey, TContext>> _logger;

        public AuthKitSeeder(
            AuthKitSeedBuilder<TUser, TRole, TKey> seed, TContext db, RoleManager<TRole> roles,
            UserManager<TUser> users, IConfiguration configuration,
            ILogger<AuthKitSeeder<TUser, TRole, TKey, TContext>> logger)
        {
            _seed = seed;
            _db = db;
            _roles = roles;
            _users = users;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken)
        {
            var catalog = AuthKitPermissions.GetAll().Concat(_seed.PermissionNames).ToHashSet(StringComparer.Ordinal);
            Validate(catalog);

            var ids = await SyncPermissionsAsync(catalog, cancellationToken);

            foreach (var role in _seed.Roles)
                await SyncRoleAsync(role, catalog, ids, cancellationToken);

            await SeedAdminAsync(cancellationToken);
        }

        private void Validate(HashSet<string> catalog)
        {
            var errors = new List<string>();

            foreach (var duplicate in _seed.Roles.GroupBy(r => r.Name, StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1))
                errors.Add($"Role '{duplicate.Key}' is defined more than once.");

            foreach (var role in _seed.Roles)
                foreach (var permission in role.Permissions.Where(p => !catalog.Contains(p)))
                    errors.Add($"Role '{role.Name}' references unknown permission '{permission}'.");

            if (errors.Count > 0)
                throw new InvalidOperationException("Invalid AuthKit seed definition: " + string.Join(" ", errors));
        }

        private async Task<Dictionary<string, int>> SyncPermissionsAsync(HashSet<string> catalog, CancellationToken ct)
        {
            var existing = await _db.Set<Permission>().ToListAsync(ct);
            var known = existing.Select(p => p.Name).ToHashSet(StringComparer.Ordinal);

            var missing = catalog.Where(n => !known.Contains(n)).Select(n => new Permission { Name = n }).ToList();
            if (missing.Count > 0)
            {
                _db.Set<Permission>().AddRange(missing);
                await _db.SaveChangesAsync(ct);
                existing.AddRange(missing);
            }

            foreach (var obsolete in existing.Where(p => !catalog.Contains(p.Name)))
                _logger.LogWarning(
                    "Permission '{Name}' exists in the database but not in the seed catalog. Remove it manually if it is obsolete.",
                    obsolete.Name);

            return existing.Where(p => catalog.Contains(p.Name)).ToDictionary(p => p.Name, p => p.Id, StringComparer.Ordinal);
        }

        private async Task SyncRoleAsync(RoleSeed seed, HashSet<string> catalog, Dictionary<string, int> ids, CancellationToken ct)
        {
            var role = await _roles.FindByNameAsync(seed.Name);
            var created = false;

            if (role is null)
            {
                role = _seed.RoleFactory(seed.Name, seed.Description);
                var result = await _roles.CreateAsync(role);
                if (!result.Succeeded)
                    throw new InvalidOperationException(
                        $"Could not create role '{seed.Name}': {string.Join(",", result.Errors.Select(e => e.Code))}");
                created = true;
            }

            if (!created && !seed.AllPermissions)
                return;   // دور موجود: الأدمن بيتحكم فيه من الـ API

            var wanted = (seed.AllPermissions ? catalog : seed.Permissions.ToHashSet()).Select(n => ids[n]).ToList();
            var linked = await _db.Set<RolePermission<TKey>>()
                .Where(rp => rp.RoleId.Equals(role.Id))
                .Select(rp => rp.PermissionId)
                .ToListAsync(ct);

            var toAdd = wanted.Except(linked).ToList();
            if (toAdd.Count == 0) return;

            _db.Set<RolePermission<TKey>>().AddRange(
                toAdd.Select(id => new RolePermission<TKey> { RoleId = role.Id, PermissionId = id }));
            await _db.SaveChangesAsync(ct);
        }

        private async Task SeedAdminAsync(CancellationToken ct)
        {
            if (_seed.AdminFactory is null || _seed.AdminRoleName is null)
                return;

            var email = _configuration[_seed.AdminEmailKey];
            var password = _configuration[_seed.AdminPasswordKey];

            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogInformation("No initial admin email is configured; skipping.");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("An initial admin email is configured but the password is missing; skipping.");
                return;
            }

            if (await _users.FindByEmailAsync(email) is not null)
                return;

            var user = _seed.AdminFactory(email);
            user.UserName ??= email;
            user.Email ??= email;
            user.EmailConfirmed = true;

            // المستخدم ودوره مع بعض، وإلا يفضل حساب بدون دور وإعادة التشغيل ما بتصلحوش
            await using var transaction = await _db.Database.BeginTransactionAsync(ct);

            var created = await _users.CreateAsync(user, password);
            if (!created.Succeeded)
                throw new InvalidOperationException(
                    "Could not create the initial admin: " + string.Join(",", created.Errors.Select(e => e.Code)));

            var added = await _users.AddToRoleAsync(user, _seed.AdminRoleName);
            if (!added.Succeeded)
                throw new InvalidOperationException(
                    "Could not assign the admin role: " + string.Join(",", added.Errors.Select(e => e.Code)));

            await transaction.CommitAsync(ct);
            _logger.LogInformation("The initial admin was created.");
        }
    }
}
