using AuthKit.Blazor.Configuration;
using AuthKit.Blazor.Http;
using AuthKit.Blazor.Localization;
using AuthKit.Blazor.Session;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Blazor.Extensions
{
    public static class AuthKitUiServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthKitUi(this IServiceCollection services, Action<AuthKitUiOptions> configure)
        {
            var options = new AuthKitUiOptions();
            configure(options);
            options.Validate();

            services.TryAddSingleton(TimeProvider.System);
            services.AddSingleton(options);          // إعدادات مشتركة، لا تحمل حالة مستخدم

            // كل ما هو خاص بالمستخدم Scoped: دائرة Blazor Server لكل مستخدم، وفي WebAssembly نسخة واحدة
            services.AddScoped<AuthKitJs>();
            services.AddScoped<ITokenStore, JsTokenStore>();
            services.AddScoped<IRefreshLock, JsRefreshLock>();
            services.AddScoped<AuthKitSession>();
            services.AddScoped<AkThemeState>();
            services.AddScoped<IAkLocalizer, AkLocalizer>();
            services.AddScoped<IAuthKitApi, AuthKitApi>();

           

            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, AuthKitAuthenticationStateProvider>();
            services.AddCascadingAuthenticationState();
            if (options.RefreshTokenMode == AkRefreshMode.Cookie && !OperatingSystem.IsBrowser())
                throw new InvalidOperationException(
                    "RefreshTokenMode.Cookie requires Blazor WebAssembly (the browser must send the cookie). Use AkRefreshMode.Body for Blazor Server.");

            services.AddScoped<ISessionHint, JsSessionHint>();

            var http = services.AddHttpClient<AuthKitTransport>((sp, client) =>
                client.BaseAddress = sp.GetRequiredService<AuthKitUiOptions>().BaseUri);

            if (options.RefreshTokenMode == AkRefreshMode.Cookie)
            {
                services.AddTransient<CookieModeHandler>();
                http.AddHttpMessageHandler<CookieModeHandler>();
            }

            return services;
        }
    }
}
