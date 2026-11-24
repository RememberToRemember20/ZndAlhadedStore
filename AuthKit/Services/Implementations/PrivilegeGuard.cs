using AuthKit.Common;
using AuthKit.Entities;
using AuthKit.Services.Interfaces;
using AuthKit.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Implementations
{
    internal class PrivilegeGuard<TUser, TKey, TContext> : IPrivilegeGuard<TKey>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    where TContext : DbContext
    {
        private readonly ICurrentUser _currentUser;
        private readonly UserManager<TUser> _userManager;
        private readonly IUserPermissionResolver<TKey> _permissions;
        private readonly TContext _db;
        private readonly AuthKitOptions _options;

        public PrivilegeGuard(
            ICurrentUser currentUser, UserManager<TUser> userManager,
            IUserPermissionResolver<TKey> permissions, TContext db, IOptions<AuthKitOptions> options)
        {
            _currentUser = currentUser;
            _userManager = userManager;
            _permissions = permissions;
            _db = db;
            _options = options.Value;
        }

        public async Task<Result> RequireCallerHoldsAsync(IReadOnlyCollection<string> permissionNames, CancellationToken cancellationToken)
        {
            if (_currentUser.IsSystem)
                return Result.Success();

            if (_currentUser.UserId is null)
                return Result.Failure(AuthErrorCodes.Forbidden);

            var caller = await _userManager.FindByIdAsync(_currentUser.UserId);
            if (caller is null)
                return Result.Failure( AuthErrorCodes.AccountUnavailable);

            // الصلاحيات من قاعدة البيانات لحظة الطلب، مش من توكن ممكن يكون قديم
            var held = (await _permissions.GetUserPermissionsAsync(caller.Id, cancellationToken)).ToHashSet(StringComparer.Ordinal);

            return permissionNames.All(held.Contains)
                ? Result.Success()
                : Result.Failure( AuthErrorCodes.PrivilegeEscalation);
        }

        public async Task<Result> RequireCallerDominatesRoleAsync(TKey roleId, CancellationToken cancellationToken)
        {
            var rolePermissions = await _permissions.GetRolePermissionsAsync([roleId], cancellationToken);
            return await RequireCallerHoldsAsync(rolePermissions, cancellationToken);
        }

        public Result EnsureRoleIsNotProtected(string roleName) =>
            _options.ProtectedRoleNames.Contains(roleName, StringComparer.OrdinalIgnoreCase)
                ? Result.Failure(AuthErrorCodes.ProtectedRole)
                : Result.Success();

        public async Task<Result> EnsureNotRemovingLastHolderAsync(TKey userId, TKey roleId, CancellationToken cancellationToken)
        {
            if (_options.CriticalPermissions.Count == 0)
                return Result.Success();

            var rolePermissions = await _permissions.GetRolePermissionsAsync([roleId], cancellationToken);

            foreach (var permissionName in rolePermissions.Intersect(_options.CriticalPermissions))
            {
                var anotherHolder = await _db.Set<IdentityUserRole<TKey>>()
                    .Where(ur => !(ur.UserId.Equals(userId) && ur.RoleId.Equals(roleId)))
                    .Join(_db.Set<RolePermission<TKey>>(), ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => rp.PermissionId)
                    .Join(_db.Set<Permission>().Where(p => p.Name == permissionName), id => id, p => p.Id, (id, p) => p.Id)
                    .AnyAsync(cancellationToken);

                if (!anotherHolder)
                    return Result.Failure( AuthErrorCodes.LastAdministrator);
            }

            return Result.Success();
        }

        public async Task<Result> EnsureNotOrphaningPermissionAsync(TKey roleId, string permissionName, CancellationToken cancellationToken)
        {
            if (!_options.CriticalPermissions.Contains(permissionName))
                return Result.Success();

            var anotherHolder = await _db.Set<IdentityUserRole<TKey>>()
                .Where(ur => !ur.RoleId.Equals(roleId))
                .Join(_db.Set<RolePermission<TKey>>(), ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => rp.PermissionId)
                .Join(_db.Set<Permission>().Where(p => p.Name == permissionName), id => id, p => p.Id, (id, p) => p.Id)
                .AnyAsync(cancellationToken);

            return anotherHolder
                ? Result.Success()
                : Result.Failure( AuthErrorCodes.LastAdministrator);
        }
    }
}

