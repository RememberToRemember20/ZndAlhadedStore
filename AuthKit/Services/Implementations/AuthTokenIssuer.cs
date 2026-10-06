using AuthKit.Entities;
using AuthKit.Services.Interfaces;
using AuthKit.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.DTOs.Auth;



namespace AuthKit.Services.Implementations
{
    public class AuthTokenIssuer<TUser, TRole, TKey, TContext> : IAuthTokenIssuer<TUser, TKey>
    where TUser : IdentityUser<TKey>
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TContext : IdentityDbContext<TUser, TRole, TKey>
    {
        private readonly ITokenService<TUser, TKey> _tokenService;
        private readonly UserManager<TUser> _userManager;
        private readonly TContext _dbContext;
        private readonly JwtSettings _jwtSettings;

        public AuthTokenIssuer(
            ITokenService<TUser, TKey> tokenService,
            UserManager<TUser> userManager,
            TContext dbContext,
            IOptions<JwtSettings> jwtSettings)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _dbContext = dbContext;
            _jwtSettings = jwtSettings.Value;
        }

        public Task<AuthResult> IssueTokensAsync(TUser user) => GenerateAndStoreTokensAsync(user, replacingToken: null);

        public Task<AuthResult> RotateTokensAsync(TUser user, RefreshToken<TKey> currentToken) =>
            GenerateAndStoreTokensAsync(user, replacingToken: currentToken);

        private async Task<AuthResult> GenerateAndStoreTokensAsync(TUser user, RefreshToken<TKey>? replacingToken)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var permissions = await _dbContext.UserRoles
                .Where(ur => ur.UserId.Equals(user.Id))
                .Join(_dbContext.Set<RolePermission<TKey>>(), ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => rp.PermissionId)
                .Join(_dbContext.Set<Permission>(), id => id, p => p.Id, (id, p) => p.Name)
                .Distinct()
                .ToListAsync();

            var accessToken = _tokenService.GenerateAccessToken(user, roles, permissions);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenHash = _tokenService.HashToken(refreshToken);

            if (replacingToken is not null)
            {
                replacingToken.RevokedAt = DateTime.UtcNow;
                replacingToken.ReplacedByTokenHash = refreshTokenHash;
            }

            _dbContext.Set<RefreshToken<TKey>>().Add(new RefreshToken<TKey>
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            });

            await _dbContext.SaveChangesAsync();

            return new AuthResult(accessToken, refreshToken, DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes));
        }

        public async Task RevokeAllActiveTokensAsync(TKey userId, CancellationToken cancellationToken = default)
        {
            var activeTokens = await _dbContext.Set<RefreshToken<TKey>>()
                .Where(rt => rt.UserId.Equals(userId) && rt.RevokedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
                token.RevokedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
