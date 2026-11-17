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
            builder.Entity<StaffProfile>(entity =>
            {
                entity.HasKey(s => s.UserId);
                entity.Property(s => s.JobTitle).HasMaxLength(100);
                entity.Property(s => s.Department).HasMaxLength(100);

                entity.HasOne(s => s.User)
                      .WithOne(u => u.StaffProfile)
                      .HasForeignKey<StaffProfile>(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<StaffProfile>()
                      .WithMany()
                      .HasForeignKey(s => s.ReportsToId)
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
        public DbSet<StaffProfile> StaffProfiles => Set<StaffProfile>();
    }
}
