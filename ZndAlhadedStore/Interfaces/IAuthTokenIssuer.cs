using Shared.DTOs.Auth;
using ZndAlhadedStore.Identity;

namespace ZndAlhadedStore.Interfaces
{
    public interface IAuthTokenIssuer
    {
        Task<AuthResult> IssueTokensAsync(ApplicationUser user);
        Task<AuthResult> RotateTokensAsync(ApplicationUser user, RefreshToken currentToken);
        Task RevokeAllActiveTokensAsync(string userId, CancellationToken cancellationToken = default);


    }
}
