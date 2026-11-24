using AuthKit.Api.Controllers;
using AuthKit.Api.Options;
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

            builder.Services.AddSingleton<IOptions<AuthKitApiOptions>>(Options.Create(options));

            builder.AddApplicationPart(typeof(AuthController).Assembly);
            builder.ConfigureApplicationPartManager(manager =>
                manager.FeatureProviders.Add(new AuthKitControllerFilter(options)));

            return builder;
        }
    }
}
