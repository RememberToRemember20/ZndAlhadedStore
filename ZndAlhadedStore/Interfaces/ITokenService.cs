using System.Security.Claims;
using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
        string GenerateRefreshToken();
        string HashToken(string token);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
