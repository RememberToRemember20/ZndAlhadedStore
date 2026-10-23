using AuthKit.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Extensions
{
    public static class AuthKitModelBuilderExtensions
    {
        public static ModelBuilder ApplyAuthKit<TUser, TRole, TKey>(this ModelBuilder builder)
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
        {
            builder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions");
                entity.HasIndex(p => p.Name).IsUnique();
            });

            builder.Entity<RolePermission<TKey>>(entity =>
            {
                entity.ToTable("RolePermissions");
                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

                entity.HasOne<TRole>().WithMany().HasForeignKey(rp => rp.RoleId);
                entity.HasOne<Permission>().WithMany().HasForeignKey(rp => rp.PermissionId);
            });

            builder.Entity<RefreshToken<TKey>>(entity =>
            {
                entity.ToTable("RefreshTokens");
                entity.HasOne<TUser>().WithMany().HasForeignKey(rt => rt.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            return builder;
        }
    }
}
