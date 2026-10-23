using AuthKit.Entities;
using AuthKit.Extensions;
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
         builder.ApplyAuthKit<ApplicationUser, ApplicationRole, string>();
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
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission<string>> RolePermissions => Set<RolePermission<string>>();
        public DbSet<RefreshToken<string>> RefreshTokens => Set<RefreshToken<string>>();
        public DbSet<ManagerProfile> ManagerProfiles { get; set; }
        public DbSet<EmployeeProfile> EmployeeProfiles { get; set; }
    }
}
