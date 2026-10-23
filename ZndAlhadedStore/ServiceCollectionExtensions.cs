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
using ZndAlhadedStore.Email;
using ZndAlhadedStore.EmailSender;
using ZndAlhadedStore.Identity;
using ZndAlhadedStore.Implment;
using ZndAlhadedStore.Interfaces;
using ZndAlhadedStore.Services;
using ZndAlhadedStore.Settings;
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
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            options.TokenLifespan = TimeSpan.FromMinutes(30));

            return services;
        }
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<ICustomClaimsProvider<ApplicationUser>, AppCustomClaimsProvider>();
            services.AddSingleton<IUserAccessGuard<ApplicationUser>, AppUserAccessGuard>();

            services.AddAuthKitCore<ApplicationUser, ApplicationRole, string, AppDbContext>();   

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(AuthKit.Common.Behaviors.ValidationBehavior<,>));
            });

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            services.AddAuthKitPasswordRecovery<ApplicationUser, string, AppDbContext>();
            services.AddAuthKitEmailConfirmation<ApplicationUser, string>();
            return services;
        }
        public static IServiceCollection AddEmailSending(
    this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            if (!string.IsNullOrWhiteSpace(configuration["Smtp:Host"]))
            {
                services.AddOptions<SmtpSettings>()
                    .Bind(configuration.GetSection("Smtp"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

                services.AddOptions<FrontendSettings>()
                    .Bind(configuration.GetSection("Frontend"))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

                services.AddSingleton<EmailQueue>();
                services.AddSingleton<IEmailQueue>(sp => sp.GetRequiredService<EmailQueue>());
                services.AddHostedService<EmailBackgroundService>();
                services.AddSingleton<IAuthEmailSender<ApplicationUser>, SmtpAuthEmailSender>();
            }
            else if (environment.IsDevelopment())
            {
                services.AddSingleton<IAuthEmailSender<ApplicationUser>, DevLoggingEmailSender>();
            }
            else
            {
                throw new InvalidOperationException(
                    "إعدادات البريد (Smtp) غير موجودة. لا يُسمح بتشغيل بيئة غير التطوير بدون مرسل بريد حقيقي.");
            }

            return services;
        }
    }
}
