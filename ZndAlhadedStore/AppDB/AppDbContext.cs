using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZndAlhadedStore.Entity;
using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.AppDB
{
    public class AppDbContext: IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

                entity.HasOne(rp => rp.Role)
                      .WithMany(r => r.RolePermissions)
                      .HasForeignKey(rp => rp.RoleId);

                entity.HasOne(rp => rp.Permission)
                      .WithMany(p => p.RolePermissions)
                      .HasForeignKey(rp => rp.PermissionId);
            });
            builder.Entity<Permission>()
            .HasIndex(p => p.Name)
            .IsUnique();
            builder.Entity<RefreshToken>()
           .HasOne(rt => rt.User)
           .WithMany(u => u.RefreshTokens)
           .HasForeignKey(rt => rt.UserId)
           .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ManagerProfile>(entity =>
            {
                entity.HasKey(m => m.UserId);
                entity.HasOne(m => m.User)
                      .WithOne(u => u.ManagerProfile)
                      .HasForeignKey<ManagerProfile>(m => m.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<EmployeeProfile>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.HasOne(e => e.User)
                      .WithOne(u => u.EmployeeProfile)
                      .HasForeignKey<EmployeeProfile>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Manager)
                      .WithMany()
                      .HasForeignKey(e => e.ManagerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<QuoteItem> QuoteItems { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions {  get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ManagerProfile> ManagerProfiles { get; set; }
        public DbSet<EmployeeProfile> EmployeeProfiles { get; set; }
    }
}
