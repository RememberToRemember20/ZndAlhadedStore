using AuthKit.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.Identity.Seeding
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<AppDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = services.GetRequiredService<IConfiguration>();

            await SeedPermissionsAsync(dbContext);
            await SeedAdminRoleAsync(roleManager, dbContext);
            await roleManager.CreateAsync(new ApplicationRole { Name = "User", Description = "مستخدم عادي" });
            await SeedAdminUserAsync(userManager, configuration);
        }

        private static async Task SeedPermissionsAsync(AppDbContext dbContext)
        {
            var existingNames = await dbContext.Permissions.Select(p => p.Name).ToListAsync();

            var newPermissions = Permissions.GetAll()
                .Except(existingNames)
                .Select(name => new Permission { Name = name })
                .ToList();

            if (newPermissions.Count > 0)
            {
                dbContext.Permissions.AddRange(newPermissions);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task SeedAdminRoleAsync(RoleManager<ApplicationRole> roleManager, AppDbContext dbContext)
        {
            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole is null)
            {
                adminRole = new ApplicationRole { Name = "Admin", Description = "صلاحيات كاملة على النظام" };
                await roleManager.CreateAsync(adminRole);
            }

            var allPermissionIds = await dbContext.Permissions.Select(p => p.Id).ToListAsync();
            var linkedPermissionIds = await dbContext.RolePermissions
                .Where(rp => rp.RoleId == adminRole.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var missingIds = allPermissionIds.Except(linkedPermissionIds);
            foreach (var permissionId in missingIds)
                dbContext.RolePermissions.Add(new RolePermission<string> { RoleId = adminRole.Id, PermissionId = permissionId });

            await dbContext.SaveChangesAsync();
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            var email = configuration["SeedAdmin:Email"];
            var password = configuration["SeedAdmin:Password"];

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return;

            if (await userManager.FindByEmailAsync(email) is not null)
                return;

            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = "System Administrator",
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
