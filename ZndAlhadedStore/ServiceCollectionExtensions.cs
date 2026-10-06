using Auth.Middlewares;
using AuthKit.Extensions;
using AuthKit.Services.Implementations;
using AuthKit.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.ClaimsProvider;
using ZndAlhadedStore.Identity;
using ZndAlhadedStore.Implment;
using ZndAlhadedStore.Interfaces;
using ZndAlhadedStore.UserAccess;


namespace ZndAlhadedStore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ZndAlHaded")));

            return services;
        }
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
             .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<ICustomClaimsProvider<ApplicationUser>, AppCustomClaimsProvider>();
            services.AddSingleton<IUserAccessGuard<ApplicationUser>, AppUserAccessGuard>();

            services.AddAuthKitCore<ApplicationUser, ApplicationRole, string, AppDbContext>();   // دلوقتي بيسجل كل حاجة

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(AuthKit.Common.Behaviors.ValidationBehavior<,>));
            });

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }
    }
}
