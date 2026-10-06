using AuthKit.Entities;
using Microsoft.AspNetCore.Identity;

namespace ZndAlhadedStore.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
        public ICollection<RolePermission<string>> RolePermissions { get; set; } = new List<RolePermission<string>>();
    }
}
