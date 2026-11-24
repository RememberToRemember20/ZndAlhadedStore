using AuthKit.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.Settings
{
    public class StaffProvisioningRules : IUserProvisioningValidator<ApplicationUser>
    {
        private readonly AppDbContext _db;
        public StaffProvisioningRules(AppDbContext db) => _db = db;

        public async Task<string?> ValidateAsync(UserProvisioningContext<ApplicationUser> context, CancellationToken ct)
        {
            var profile = context.User.StaffProfile;
            if (profile is null)
                return "staff_profile_required";

            if (profile.ReportsToId is not null &&
                !await _db.StaffProfiles.AnyAsync(s => s.UserId == profile.ReportsToId && s.User.IsActive, ct))
                return "reports_to_invalid";

            return null;
        }
    }

}
