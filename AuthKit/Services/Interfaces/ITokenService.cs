using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


namespace AuthKit.Services.Interfaces
{
    public interface ITokenService<TUser, TKey>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    {
        string GenerateAccessToken(TUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
        string GenerateRefreshToken();
        string HashToken(string token);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
