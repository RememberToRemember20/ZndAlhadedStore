using Microsoft.AspNetCore.Authorization;

namespace ZndAlhadedStore.PermissionFoldar.Command
{
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string permission) : base(policy: permission)
        {
        }
    }
}