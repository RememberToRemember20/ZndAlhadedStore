using Microsoft.AspNetCore.Authorization;

namespace ZndAlhadedStore.PermissionFoldar.Command
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
