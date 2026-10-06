
using AuthKit.Entities;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs.Auth;




namespace AuthKit.Services.Interfaces
{
    public interface IAuthTokenIssuer<TUser, TKey>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    {
        Task<AuthResult> IssueTokensAsync(TUser user);
        Task<AuthResult> RotateTokensAsync(TUser user, RefreshToken<TKey> currentToken);
        Task RevokeAllActiveTokensAsync(TKey userId, CancellationToken cancellationToken = default);
    }
}
