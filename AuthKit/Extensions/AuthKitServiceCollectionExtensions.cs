

using AuthKit.Authorization.PermissionFoldar.Command;
using AuthKit.Authorization.PermissionFoldar.Handler;
using AuthKit.Common;
using AuthKit.Features.AccessControl.AssignPermissionToRole.Command;
using AuthKit.Features.AccessControl.AssignPermissionToRole.Handler;
using AuthKit.Features.AccessControl.AssignRole.Command;
using AuthKit.Features.AccessControl.AssignRole.Handler;
using AuthKit.Features.AccessControl.GetPermissions.Handler;
using AuthKit.Features.AccessControl.GetPermissions.Query;
using AuthKit.Features.AccessControl.GetRolePermissions.Handler;
using AuthKit.Features.AccessControl.GetRolePermissions.Quey;
using AuthKit.Features.AccessControl.GetRoles.Handler;
using AuthKit.Features.AccessControl.GetRoles.Query;
using AuthKit.Features.AccessControl.RemoveRoleFromUser.Command;
using AuthKit.Features.AccessControl.RemoveRoleFromUser.Handler;
using AuthKit.Features.AccessControl.RevokePermissionFromRole;
using AuthKit.Features.Auth.Login.Command;
using AuthKit.Features.Auth.Login.Handler;
using AuthKit.Features.Auth.Logout.Command;
using AuthKit.Features.Auth.Logout.Handler;
using AuthKit.Features.Auth.LogoutAll.Command;
using AuthKit.Features.Auth.LogoutAll.Handler;
using AuthKit.Features.Auth.RefreshToken.Command;
using AuthKit.Features.Auth.RefreshToken.Handler;
using AuthKit.Features.Auth.Register.Command;
using AuthKit.Features.Auth.Register.Handler;
using AuthKit.Features.Auth.Role.Command;
using AuthKit.Features.Auth.Role.Handler;
using AuthKit.Services.Implementations;
using AuthKit.Services.Interfaces;
using AuthKit.Settings;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs.Auth;
using System.Text;

namespace AuthKit.Extensions
{
    public static class AuthKitServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services, IConfiguration configuration, string sectionName = "Jwt")
        {
            var jwtSettings = configuration.GetSection(sectionName).Get<JwtSettings>()
                ?? throw new InvalidOperationException($"قسم الإعدادات '{sectionName}' غير موجود.");

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
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TContext : IdentityDbContext<TUser, TRole, TKey>
        {
            services.AddSingleton<ITokenService<TUser, TKey>, TokenService<TUser, TKey>>();
            services.AddScoped<IAuthTokenIssuer<TUser, TKey>, AuthTokenIssuer<TUser, TRole, TKey, TContext>>();

            services.AddScoped<IRequestHandler<RegisterCommand<TUser, TKey>, Result<AuthResult>>, RegisterCommandHandler<TUser, TKey>>();
            services.AddScoped<IValidator<RegisterCommand<TUser, TKey>>, RegisterCommandValidator<TUser, TKey>>();

            services.AddScoped<IRequestHandler<LoginCommand, Result<AuthResult>>, LoginCommandHandler<TUser, TKey>>();
            services.AddScoped<IValidator<LoginCommand>, LoginCommandValidator>();

            services.AddScoped<IRequestHandler<RefreshTokenCommand, Result<AuthResult>>, RefreshTokenCommandHandler<TUser, TKey, TContext>>();
            services.AddScoped<IValidator<RefreshTokenCommand>, RefreshTokenCommandValidator>();

            services.AddScoped<IRequestHandler<LogoutCommand, Result>, LogoutCommandHandler<TUser, TKey, TContext>>();
            services.AddScoped<IValidator<LogoutCommand>, LogoutCommandValidator>();

            services.AddScoped<IRequestHandler<LogoutAllCommand, Result>, LogoutAllCommandHandler<TUser, TKey>>();
            services.AddScoped<IRequestHandler<AssignRoleCommand, Result>, AssignRoleCommandHandler<TUser, TRole, TKey>>();

            services.AddScoped<IRequestHandler<GetRolesQuery, Result<List<AuthKit.Features.AccessControl.GetRoles.Query.RoleDto>>>, GetRolesQueryHandler<TRole, TKey>>();

            services.AddScoped<IRequestHandler<GetPermissionsQuery, Result<List<AuthKit.Features.AccessControl.GetPermissions.Query.PermissionDto>>>, GetPermissionsQueryHandler<TContext>>();

            services.AddScoped<IRequestHandler<GetRolePermissionsQuery, Result<List<AuthKit.Features.AccessControl.GetPermissions.Query.PermissionDto>>>, GetRolePermissionsQueryHandler<TRole, TKey, TContext>>();

            services.AddScoped<IRequestHandler<AssignPermissionToRoleCommand, Result>, AssignPermissionToRoleCommandHandler<TRole, TKey, TContext>>();
            services.AddScoped<IValidator<AssignPermissionToRoleCommand>, AssignPermissionToRoleCommandValidator>();

            services.AddScoped<IRequestHandler<RevokePermissionFromRoleCommand, Result>, RevokePermissionFromRoleCommandHandler<TRole, TKey, TContext>>();

            services.AddScoped<IRequestHandler<CreateRoleCommand<TRole, TKey>, Result<TKey>>, CreateRoleCommandHandler<TRole, TKey>>();
            services.AddScoped<IValidator<CreateRoleCommand<TRole, TKey>>, CreateRoleCommandValidator<TRole, TKey>>();

            services.AddScoped<IRequestHandler<UpdateRoleCommand<TRole, TKey>, Result>, UpdateRoleCommandHandler<TRole, TKey>>();
            services.AddScoped<IValidator<UpdateRoleCommand<TRole, TKey>>, UpdateRoleCommandValidator<TRole, TKey>>();

            services.AddScoped<IRequestHandler<DeleteRoleCommand<TKey>, Result>, DeleteRoleCommandHandler<TUser, TRole, TKey>>();

            services.AddScoped<IRequestHandler<RemoveRoleFromUserCommand, Result>, RemoveRoleFromUserCommandHandler<TUser, TKey>>();
            services.AddScoped<IRequestHandler<RegisterCommand<TUser, TKey>, Result<AuthResult>>, RegisterCommandHandler<TUser, TKey, TContext>>();
            return services;
        }
    }
}
