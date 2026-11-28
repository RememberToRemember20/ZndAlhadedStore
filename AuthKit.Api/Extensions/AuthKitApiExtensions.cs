using AuthKit.Api.Controllers;
using AuthKit.Api.Cookies;
using AuthKit.Api.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Api.Extensions
{
    public static class AuthKitApiExtensions
    {
        public static IMvcBuilder AddAuthKitApi(this IMvcBuilder builder, Action<AuthKitApiOptions>? configure = null)
        {
            var options = new AuthKitApiOptions();
            configure?.Invoke(options);

            builder.Services.AddSingleton<IOptions<AuthKitApiOptions>>(Microsoft.Extensions.Options.Options.Create(options));

            builder.AddApplicationPart(typeof(AuthController).Assembly);
            builder.ConfigureApplicationPartManager(manager =>
                manager.FeatureProviders.Add(new AuthKitControllerFilter(options)));
            builder.Services.AddSingleton<RefreshCookieService>();

            return builder;
        }
        public static IApplicationBuilder UseAuthKitSecurityHeaders(this IApplicationBuilder app) =>
    app.Use(async (context, next) =>
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;
            headers["X-Content-Type-Options"] = "nosniff";
            headers["Referrer-Policy"] = "no-referrer";

            if (context.Request.Path.StartsWithSegments("/api/auth"))
            {
                headers["Cache-Control"] = "no-store";
                headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'";
            }

            return Task.CompletedTask;
        });

        await next();
    });
    }
}
