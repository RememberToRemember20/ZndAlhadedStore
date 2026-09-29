using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.Entity
{
    public class EmployeeProfile
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }

        public string? ManagerId { get; set; }
        public ApplicationUser? Manager { get; set; }
    }

  
}
