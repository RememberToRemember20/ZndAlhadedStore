using AuthKit.Authorization.PermissionFoldar.Command;
using Microsoft.AspNetCore.Authorization;

namespace AuthKit.Authorization.PermissionFoldar.Handler
{
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var hasPermission = context.User.HasClaim(AuthKitClaims.Permission, requirement.Permission);

            if (hasPermission)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}

