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
        private readonly IUserAccessGuard<TUser>? _accessGuard;

        public PrivilegeGuard(
            ICurrentUser currentUser, UserManager<TUser> userManager,
            IUserPermissionResolver<TKey> permissions, TContext db, IOptions<AuthKitOptions> options, IUserAccessGuard<TUser> accessGuard)
        {
            _currentUser = currentUser;
            _userManager = userManager;
            _permissions = permissions;
            _db = db;
            _options = options.Value;
            _accessGuard = accessGuard;
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

        private async Task<bool> HasActiveHolderAsync(
      IQueryable<IdentityUserRole<TKey>> candidates, string permissionName, CancellationToken ct)
        {
            var holderIds = await candidates
                .Join(_db.Set<RolePermission<TKey>>(), ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => new { ur.UserId, rp.PermissionId })
                .Join(_db.Set<Permission>().Where(p => p.Name == permissionName), x => x.PermissionId, p => p.Id, (x, p) => x.UserId)
                .Distinct()
                .ToListAsync(ct);

            if (_accessGuard is null)
                return holderIds.Count > 0;

            foreach (var holderId in holderIds)
            {
                var holder = await _userManager.FindByIdAsync(holderId.ToString()!);
                if (holder is not null && _accessGuard.CanSignIn(holder))
                    return true;
            }

            return false;
        }

        public async Task<Result> EnsureNotRemovingLastHolderAsync(TKey userId, TKey roleId, CancellationToken ct)
        {
            var rolePermissions = await _permissions.GetRolePermissionsAsync([roleId], ct);

            foreach (var permissionName in rolePermissions.Intersect(_options.CriticalPermissions))
            {
                var candidates = _db.Set<IdentityUserRole<TKey>>()
                    .Where(ur => !(ur.UserId.Equals(userId) && ur.RoleId.Equals(roleId)));

                if (!await HasActiveHolderAsync(candidates, permissionName, ct))
                    return Result.Failure(AuthErrorCodes.LastAdministrator);
            }

            return Result.Success();
        }

        public async Task<Result> EnsureNotOrphaningPermissionAsync(TKey roleId, string permissionName, CancellationToken ct)
        {
            if (!_options.CriticalPermissions.Contains(permissionName))
                return Result.Success();

            var candidates = _db.Set<IdentityUserRole<TKey>>().Where(ur => !ur.RoleId.Equals(roleId));

            return await HasActiveHolderAsync(candidates, permissionName, ct)
                ? Result.Success()
                : Result.Failure(AuthErrorCodes.LastAdministrator);
        }

        public async Task<Result> EnsureNotDisablingLastHolderAsync(TKey userId, CancellationToken ct)
        {
            var held = await _permissions.GetUserPermissionsAsync(userId, ct);

            foreach (var permissionName in held.Intersect(_options.CriticalPermissions))
            {
                var candidates = _db.Set<IdentityUserRole<TKey>>().Where(ur => !ur.UserId.Equals(userId));

                if (!await HasActiveHolderAsync(candidates, permissionName, ct))
                    return Result.Failure(AuthErrorCodes.LastAdministrator);
            }

            return Result.Success();
        }
    }
}

