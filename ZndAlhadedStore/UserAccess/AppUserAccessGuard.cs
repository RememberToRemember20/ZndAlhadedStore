using AuthKit.Services.Interfaces;
using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.UserAccess
{
    public class AppUserAccessGuard : IUserAccessGuard<ApplicationUser>
    {
        public bool CanSignIn(ApplicationUser user) => user.IsActive;
    }
}
