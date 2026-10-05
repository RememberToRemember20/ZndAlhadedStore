using Microsoft.AspNetCore.Identity;
using ZndAlhadedStore.Entity;

namespace ZndAlhadedStore.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public ManagerProfile? ManagerProfile { get; set; }
        public EmployeeProfile? EmployeeProfile { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
