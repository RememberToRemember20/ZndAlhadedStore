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

                entity.Property(rt => rt.TokenHash).HasMaxLength(128);
                entity.Property(rt => rt.ReplacedByTokenHash).HasMaxLength(128);
                entity.HasIndex(rt => rt.TokenHash).IsUnique();

                entity.HasOne<TUser>().WithMany().HasForeignKey(rt => rt.UserId).OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");
                entity.Property(a => a.EventType).HasMaxLength(64).IsRequired();
                entity.Property(a => a.FailureCode).HasMaxLength(64);
                entity.Property(a => a.ActorUserId).HasMaxLength(128);
                entity.Property(a => a.Subject).HasMaxLength(256);
                entity.Property(a => a.TargetId).HasMaxLength(128);
                entity.Property(a => a.IpAddress).HasMaxLength(45);   // أطول عنوان IPv6
                entity.Property(a => a.UserAgent).HasMaxLength(256);
                entity.Property(a => a.Details).HasMaxLength(512);

                entity.HasIndex(a => a.OccurredAtUtc);
                entity.HasIndex(a => new { a.ActorUserId, a.OccurredAtUtc });
                entity.HasIndex(a => new { a.EventType, a.OccurredAtUtc });
            });

            return builder;
        }
    }
}
