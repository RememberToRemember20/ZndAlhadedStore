using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.DTOs.Auth;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Identity;
using ZndAlhadedStore.Interfaces;
using ZndAlhadedStore.Setting;

namespace ZndAlhadedStore.Implment
{
    public class AuthTokenIssuer : IAuthTokenIssuer
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _dbContext;
        private readonly JwtSettings _jwtSettings;

        public AuthTokenIssuer(
            ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            AppDbContext dbContext,
            IOptions<JwtSettings> jwtSettings)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _dbContext = dbContext;
            _jwtSettings = jwtSettings.Value;
        }

        public Task<AuthResult> IssueTokensAsync(ApplicationUser user) =>
            GenerateAndStoreTokensAsync(user, replacingToken: null);

        public Task<AuthResult> RotateTokensAsync(ApplicationUser user, RefreshToken currentToken) =>
            GenerateAndStoreTokensAsync(user, replacingToken: currentToken);

        private async Task<AuthResult> GenerateAndStoreTokensAsync(ApplicationUser user, RefreshToken? replacingToken)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var permissions = await _dbContext.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Join(_dbContext.RolePermissions, ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => rp.PermissionId)
                .Join(_dbContext.Permissions, id => id, p => p.Id, (id, p) => p.Name)
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

            _dbContext.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            });

            await _dbContext.SaveChangesAsync();

            return new AuthResult(accessToken, refreshToken, DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes));
        }
        public async Task RevokeAllActiveTokensAsync(string userId, CancellationToken cancellationToken = default)
        {
            var activeTokens = await _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
                token.RevokedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
