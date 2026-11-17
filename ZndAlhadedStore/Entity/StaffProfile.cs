using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.Entity
{
    public class StaffProfile
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public string? JobTitle { get; set; }       // مسمى وظيفي حر، مش دور
        public string? Department { get; set; }
        public DateOnly HireDate { get; set; }

        public string? ReportsToId { get; set; }    // مسؤوله المباشر (يشير إلى StaffProfiles.UserId)
    }
}
