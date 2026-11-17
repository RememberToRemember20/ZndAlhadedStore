using AuthKit.Entities;
using AuthKit.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Implementations
{
    public class UserPermissionResolver<TKey, TContext> : IUserPermissionResolver<TKey>
     where TKey : IEquatable<TKey>
     where TContext : DbContext
    {
        private readonly TContext _db;
        public UserPermissionResolver(TContext db) => _db = db;

        public async Task<IReadOnlyCollection<string>> GetUserPermissionsAsync(TKey userId, CancellationToken cancellationToken = default) =>
            await _db.Set<IdentityUserRole<TKey>>()
                .Where(ur => ur.UserId.Equals(userId))
                .Join(_db.Set<RolePermission<TKey>>(), ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => rp.PermissionId)
                .Join(_db.Set<Permission>(), id => id, p => p.Id, (id, p) => p.Name)
                .Distinct()
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyCollection<string>> GetRolePermissionsAsync(IReadOnlyCollection<TKey> roleIds, CancellationToken cancellationToken = default)
        {
            var ids = roleIds.ToList();
            return await _db.Set<RolePermission<TKey>>()
                .Where(rp => ids.Contains(rp.RoleId))
                .Join(_db.Set<Permission>(), rp => rp.PermissionId, p => p.Id, (rp, p) => p.Name)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}
