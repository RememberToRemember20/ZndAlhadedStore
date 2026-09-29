using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.Entity
{
    public class ManagerProfile
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public string Department { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public int TeamSize { get; set; }
    }
}
