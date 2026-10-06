using AuthKit.Services.Interfaces;
using System.Security.Claims;
using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.ClaimsProvider
{
    public class AppCustomClaimsProvider : ICustomClaimsProvider<ApplicationUser>
    {
        public IEnumerable<Claim> GetClaims(ApplicationUser user)
        {
            yield return new Claim("full_name", user.FullName);
        }
    }
}
