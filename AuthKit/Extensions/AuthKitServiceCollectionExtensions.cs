

using Auth.Middlewares;
using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Authorization.PermissionFoldar.Handler;
using AuthKit.Common;
using AuthKit.Common.Behaviors;
using AuthKit.Features.AccessControl.AssignPermissionToRole.Command;
using AuthKit.Features.AccessControl.AssignPermissionToRole.Handler;
using AuthKit.Features.AccessControl.AssignRole.Command;
using AuthKit.Features.AccessControl.AssignRole.Handler;
using AuthKit.Features.AccessControl.Audit.Handler;
using AuthKit.Features.AccessControl.Audit.Query;
using AuthKit.Features.AccessControl.GetPermissions.Handler;
using AuthKit.Features.AccessControl.GetPermissions.Query;
using AuthKit.Features.AccessControl.GetRolePermissions.Handler;
using AuthKit.Features.AccessControl.GetRolePermissions.Quey;
using AuthKit.Features.AccessControl.GetRoles.Handler;
using AuthKit.Features.AccessControl.GetRoles.Query;
using AuthKit.Features.AccessControl.RemoveRoleFromUser.Command;
using AuthKit.Features.AccessControl.RemoveRoleFromUser.Handler;
using AuthKit.Features.AccessControl.RevokePermissionFromRole;
using AuthKit.Features.Auth.Email.Command;
using AuthKit.Features.Auth.Email.Handler;
using AuthKit.Features.Auth.InventUser.Command;
using AuthKit.Features.Auth.InventUser.Handler;
using AuthKit.Features.Auth.Login.Command;
using AuthKit.Features.Auth.Login.Handler;
using AuthKit.Features.Auth.Logout.Command;
using AuthKit.Features.Auth.Logout.Handler;
using AuthKit.Features.Auth.LogoutAll.Command;
using AuthKit.Features.Auth.LogoutAll.Handler;
using AuthKit.Features.Auth.Password.Command;
using AuthKit.Features.Auth.Password.Handler;
using AuthKit.Features.Auth.RefreshToken.Command;
using AuthKit.Features.Auth.RefreshToken.Handler;
using AuthKit.Features.Auth.Register.Command;
using AuthKit.Features.Auth.Register.Handler;
using AuthKit.Features.Auth.Role.Command;
using AuthKit.Features.Auth.Role.Handler;
using AuthKit.Features.User.Handler;
using AuthKit.Features.User.Query;
using AuthKit.Models;
using AuthKit.Seeder;
using AuthKit.Services.Implementations;
using AuthKit.Services.Interfaces;
using AuthKit.Settings;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

namespace AuthKit.Extensions
{
    public static class AuthKitServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services, IConfiguration configuration, string sectionName = "Jwt")
        {
            var jwtSettings = configuration.GetSection(sectionName).Get<JwtSettings>();
                var errors = JwtSettingsValidator.GetErrors(jwtSettings);
            if (errors.Count > 0)
                throw new InvalidOperationException("Invalid Jwt configuration: " + string.Join(" ", errors));

            services.Configure<JwtSettings>(configuration.GetSection(sectionName));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }

        public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services;
        }
        public static IServiceCollection AddAuthKitCore<TUser, TRole, TKey, TContext>(this IServiceCollection services)
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>,new()

    where TKey : IEquatable<TKey>
    where TContext : IdentityDbContext<TUser, TRole, TKey>
        {
            services.AddSingleton<ITokenService<TUser, TKey>, TokenService<TUser, TKey>>();
            services.AddScoped<IAuthTokenIssuer<TUser, TKey>, AuthTokenIssuer<TUser, TRole, TKey, TContext>>();

           // services.AddScoped<IRequestHandler<RegisterCommand<TUser, TKey>, Result<AuthResult>>, RegisterCommandHandler<TUser, TKey,TContext>>();
            services.AddScoped<IValidator<RegisterCommand<TUser, TKey>>, RegisterCommandValidator<TUser, TKey>>();

            services.AddScoped<IRequestHandler<LoginCommand, Result<AuthResult>>, LoginCommandHandler<TUser, TKey>>();
            services.AddScoped<IValidator<LoginCommand>, LoginCommandValidator>();

            services.AddScoped<IRequestHandler<RefreshTokenCommand, Result<AuthResult>>, RefreshTokenCommandHandler<TUser, TKey, TContext>>();
            services.AddScoped<IValidator<RefreshTokenCommand>, RefreshTokenCommandValidator>();

            services.AddScoped<IRequestHandler<LogoutCommand, Result>, LogoutCommandHandler<TUser, TKey, TContext>>();
            services.AddScoped<IValidator<LogoutCommand>, LogoutCommandValidator>();

            services.AddScoped<IRequestHandler<LogoutAllCommand, Result>, LogoutAllCommandHandler<TUser, TKey>>();
            services.AddScoped<IRequestHandler<AssignRoleCommand, Result>, AssignRoleCommandHandler<TUser, TRole, TKey>>();

            services.AddScoped<IRequestHandler<GetRolesQuery, Result<PagedResult<RoleDto>>>, GetRolesQueryHandler<TRole, TKey>>();
            services.AddScoped<IValidator<GetRolesQuery>, GetRolesQueryValidator>();
            services.AddScoped<IRequestHandler<GetPermissionsQuery, Result<List<AuthKit.Features.AccessControl.GetPermissions.Query.PermissionDto>>>, GetPermissionsQueryHandler<TContext>>();

            services.AddScoped<IRequestHandler<GetRolePermissionsQuery, Result<List<AuthKit.Features.AccessControl.GetPermissions.Query.PermissionDto>>>, GetRolePermissionsQueryHandler<TRole, TKey, TContext>>();

            services.AddScoped<IRequestHandler<AssignPermissionToRoleCommand, Result>, AssignPermissionToRoleCommandHandler<TRole, TKey, TContext>>();
            services.AddScoped<IValidator<AssignPermissionToRoleCommand>, AssignPermissionToRoleCommandValidator>();

            services.AddScoped<IRequestHandler<RevokePermissionFromRoleCommand, Result>, RevokePermissionFromRoleCommandHandler<TRole, TKey, TContext>>();

            services.AddScoped<IRequestHandler<CreateRoleCommand, Result<string>>, CreateRoleCommandHandler<TRole, TKey>>();
            services.AddScoped<IValidator<CreateRoleCommand>, CreateRoleCommandValidator>();
            services.AddScoped<IRequestHandler<UpdateRoleCommand, Result>, UpdateRoleCommandHandler<TRole, TKey>>();
            services.AddScoped<IValidator<UpdateRoleCommand>, UpdateRoleCommandValidator>();
            services.AddScoped<IRequestHandler<DeleteRoleCommand, Result>, DeleteRoleCommandHandler<TUser, TRole, TKey>>();

            services.AddScoped<IRequestHandler<RemoveRoleFromUserCommand, Result>, RemoveRoleFromUserCommandHandler<TUser, TRole,TKey>>();
            services.AddScoped<IRequestHandler<RegisterCommand<TUser, TKey>, Result<RegisterResult>>, RegisterCommandHandler<TUser, TKey, TContext>>();
            services.AddScoped<IRequestHandler<ChangePasswordCommand, Result<AuthResult>>, ChangePasswordCommandHandler<TUser, TKey, TContext>>();
            services.AddScoped<IValidator<ChangePasswordCommand>, ChangePasswordCommandValidator>();
            services.AddSingleton<RefreshTokenCleanupService<TKey, TContext>>();
            services.AddHostedService(sp => sp.GetRequiredService<RefreshTokenCleanupService<TKey, TContext>>());
            services.AddHttpContextAccessor();
            services.AddScoped<IAuditContext, HttpAuditContext>();
            services.AddScoped<IRequestHandler<GetAuditLogsQuery, Result<PagedResult<AuditLogDto>>>, GetAuditLogsQueryHandler<TContext>>();
            services.AddScoped<IValidator<GetAuditLogsQuery>, GetAuditLogsQueryValidator>();
            services.TryAddSingleton(TimeProvider.System);
            services.AddScoped<ICurrentUser, HttpCurrentUser>();
            services.AddScoped<IPrivilegeGuard<TKey>, PrivilegeGuard<TUser, TKey, TContext>>();
            services.TryAddSingleton<IAuthErrorDescriber, NullAuthErrorDescriber>();
            services.AddScoped<IRequestHandler<GetCurrentUserQuery, Result<CurrentUserDto>>, GetCurrentUserQueryHandler<TUser, TKey>>();
            return services;
        }
        public static IServiceCollection AddAuthRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, cancellationToken) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString();
                    }

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        error = "عدد المحاولات تجاوز الحد المسموح به، حاول مرة أخرى بعد قليل."
                    }, cancellationToken);
                };

                // سياسة صارمة: Login/Register — الأكثر حساسية (استهداف مباشر لمحاولات التخمين)
                options.AddPolicy(AuthKitRateLimitPolicies.Sensitive, httpContext =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: GetClientIp(httpContext),
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 4,
                            QueueLimit = 0
                        }));

                // سياسة معتدلة: Refresh/Logout — أقل حساسية لكن تستاهل حد أعلى شوية
                options.AddPolicy(AuthKitRateLimitPolicies.Moderate, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: GetClientIp(httpContext),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));
            });

            return services;
        }
        public static IServiceCollection AddAuthKitPasswordRecovery<TUser, TKey, TContext>(this IServiceCollection services)
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    where TContext : DbContext
        {
            services.AddScoped<IRequestHandler<ForgotPasswordCommand, Result>, ForgotPasswordCommandHandler<TUser, TKey>>();
            services.AddScoped<IValidator<ForgotPasswordCommand>, ForgotPasswordCommandValidator>();

            services.AddScoped<IRequestHandler<ResetPasswordCommand, Result>, ResetPasswordCommandHandler<TUser, TKey, TContext>>();
            services.AddScoped<IValidator<ResetPasswordCommand>, ResetPasswordCommandValidator>();

            return services;
        }
        public static IServiceCollection AddAuthKitEmailConfirmation<TUser, TKey>(
    this IServiceCollection services, bool requireConfirmedEmail = true)
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
        {
            services.Configure<AuthKitOptions>(options =>
            {
                options.SendConfirmationEmailOnRegister = true;
                options.RequireConfirmedEmail = requireConfirmedEmail;
            });

            services.AddScoped<IRequestHandler<ConfirmEmailCommand, Result>, ConfirmEmailCommandHandler<TUser, TKey>>();
            services.AddScoped<IValidator<ConfirmEmailCommand>, ConfirmEmailCommandValidator>();

            services.AddScoped<IRequestHandler<ResendConfirmationEmailCommand, Result>, ResendConfirmationEmailCommandHandler<TUser, TKey>>();
            services.AddScoped<IValidator<ResendConfirmationEmailCommand>, ResendConfirmationEmailCommandValidator>();

            return services;
        }

        private static string GetClientIp(HttpContext httpContext) =>
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        public static IServiceCollection AddConfiguredCors(
    this IServiceCollection services, IConfiguration configuration, string sectionName = "Cors")
        {
            var origins = configuration.GetSection($"{sectionName}:AllowedOrigins").Get<string[]>() ?? [];

            services.AddCors(options => options.AddDefaultPolicy(policy =>
            {
                if (origins.Length > 0)
                    policy.WithOrigins(origins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .WithExposedHeaders("Retry-After");
            }));

            return services;
        }
        public static MediatRServiceConfiguration AddAuthKitBehaviors(this MediatRServiceConfiguration cfg)
        {
            cfg.AddOpenBehavior(typeof(AuditBehavior<,>));           // الخارجي: يسجّل المرفوض كمان
            cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));   // قبل التحقق: غير المصرّح له ما يشوفش رسائل التحقق
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            return cfg;
        }
        public static IServiceCollection AddAuthKitAuditing<TContext>(this IServiceCollection services)
    where TContext : DbContext
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IAuditLogger, EfAuditLogger<TContext>>();
            return services;
        }
        public static IServiceCollection AddAuthKitUserInvitation<TUser, TRole, TKey, TContext>(this IServiceCollection services)
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TContext : DbContext
        {
            services.AddScoped<IUserPermissionResolver<TKey>, UserPermissionResolver<TKey, TContext>>();

            services.AddScoped<IRequestHandler<InviteUserCommand<TUser, TKey>, Result<string>>,
                InviteUserCommandHandler<TUser, TRole, TKey, TContext>>();
            services.AddScoped<IValidator<InviteUserCommand<TUser, TKey>>, InviteUserCommandValidator<TUser, TKey>>();

            return services;
        }
        public static IServiceCollection AddAuthKitExceptionHandling(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            return services;
        }
        public static IServiceCollection AddAuthKitSeeding<TUser, TRole, TKey, TContext>(
    this IServiceCollection services, Action<AuthKitSeedBuilder<TUser, TRole, TKey>> configure)
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>, new()
    where TKey : IEquatable<TKey>
    where TContext : DbContext
        {
            var builder = new AuthKitSeedBuilder<TUser, TRole, TKey>();
            configure(builder);

            services.AddSingleton(builder);
            services.AddScoped<IAuthKitSeeder, AuthKitSeeder<TUser, TRole, TKey, TContext>>();

            // دور الأدمن بيتحمى تلقائياً بدون ما تكرر اسمه في الإعدادات
            if (builder.AdminRoleName is { } adminRole)
                services.PostConfigure<AuthKitOptions>(o =>
                {
                    if (!o.ProtectedRoleNames.Contains(adminRole, StringComparer.OrdinalIgnoreCase))
                        o.ProtectedRoleNames.Add(adminRole);
                });

            return services;
        }

        public static async Task SeedAuthKitAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
        {
            using var scope = services.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IAuthKitSeeder>().SeedAsync(cancellationToken);
        }

    }
}
