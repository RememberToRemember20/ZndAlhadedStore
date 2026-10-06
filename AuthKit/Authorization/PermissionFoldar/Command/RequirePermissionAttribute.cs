using Microsoft.AspNetCore.Authorization;

namespace AuthKit.Authorization.PermissionFoldar.Command
{
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string permission) : base(policy: permission)
        {
        }
    }
}