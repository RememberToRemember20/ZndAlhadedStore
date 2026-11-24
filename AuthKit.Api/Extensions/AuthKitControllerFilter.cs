using AuthKit.Api.Controllers;
using AuthKit.Api.Options;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Api.Extensions
{
    internal sealed class AuthKitControllerFilter : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly AuthKitApiOptions _options;
        public AuthKitControllerFilter(AuthKitApiOptions options) => _options = options;

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var controller in feature.Controllers.Where(c => IsDisabled(c.AsType())).ToList())
                feature.Controllers.Remove(controller);
        }

        private bool IsDisabled(Type type) =>
            (type == typeof(AuthController) && !_options.EnableAuth) ||
            (type == typeof(RolesController) && !_options.EnableRoles) ||
            (type == typeof(PermissionsController) && !_options.EnablePermissions) ||
            (type == typeof(UserRolesController) && !_options.EnableUserRoles) ||
            (type == typeof(AuditLogsController) && !_options.EnableAuditLogs);
    }
}
