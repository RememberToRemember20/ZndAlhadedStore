using AuthKit.Entities;
using AuthKit.Models;
using AuthKit.Services.Interfaces;
using AuthKit.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;




namespace AuthKit.Services.Implementations
{
    internal class AuthTokenIssuer<TUser, TRole, TKey, TContext> : IAuthTokenIssuer<TUser, TKey>
       where TUser : IdentityUser<TKey>
       where TRole : IdentityRole<TKey>
       where TKey : IEquatable<TKey>
       where TContext : IdentityDbContext<TUser, TRole, TKey>
    {
        private readonly ITokenService<TUser, TKey> _tokenService;
        private readonly UserManager<TUser> _userManager;
        private readonly IUserPermissionResolver<TKey> _permissions;
        private readonly TContext _dbContext;
        private readonly JwtSettings _jwtSettings;
        private readonly TimeProvider _time;

        public AuthTokenIssuer(
            ITokenService<TUser, TKey> tokenService,
            UserManager<TUser> userManager,
            IUserPermissionResolver<TKey> permissions,
            TContext dbContext,
            IOptions<JwtSettings> jwtSettings,
            TimeProvider time)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _permissions = permissions;
            _dbContext = dbContext;
            _jwtSettings = jwtSettings.Value;
            _time = time;
        }

        public Task<AuthResult> IssueTokensAsync(TUser user)
        {
            var raw = _tokenService.GenerateRefreshToken();
            return CreateAsync(user, raw, _tokenService.HashToken(raw), _time.GetUtcNow().UtcDateTime);
        }

        public async Task<AuthResult?> RotateTokensAsync(TUser user, RefreshToken<TKey> currentToken)
        {
            var now = _time.GetUtcNow().UtcDateTime;
            var raw = _tokenService.GenerateRefreshToken();
            var hash = _tokenService.HashToken(raw);

            // لو فيه Transaction خارجية بنشارك فيها، وإلا بنفتح واحدة
            await using var transaction = _dbContext.Database.CurrentTransaction is null
                ? await _dbContext.Database.BeginTransactionAsync()
                : null;

            var revoked = await _dbContext.Set<RefreshToken<TKey>>()
                .Where(rt => rt.Id == currentToken.Id && rt.RevokedAt == null)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(rt => rt.RevokedAt, (DateTime?)now)
                    .SetProperty(rt => rt.ReplacedByTokenHash, hash));

            if (revoked == 0)
                return null;   // طلب آخر سبقه. الـ Dispose بيتراجع تلقائياً

            var result = await CreateAsync(user, raw, hash, now);
            if (transaction is not null)
                await transaction.CommitAsync();

            return result;
        }

        public async Task RevokeAllActiveTokensAsync(TKey userId, CancellationToken cancellationToken = default)
        {
            var now = _time.GetUtcNow().UtcDateTime;

            await _dbContext.Set<RefreshToken<TKey>>()
                .Where(rt => rt.UserId.Equals(userId) && rt.RevokedAt == null)
                .ExecuteUpdateAsync(s => s.SetProperty(rt => rt.RevokedAt, (DateTime?)now), cancellationToken);
        }

        private async Task<AuthResult> CreateAsync(TUser user, string rawRefreshToken, string refreshTokenHash, DateTime now)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await _permissions.GetUserPermissionsAsync(user.Id);

            var accessToken = _tokenService.GenerateAccessToken(user, roles, permissions);

            _dbContext.Set<RefreshToken<TKey>>().Add(new RefreshToken<TKey>
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                CreatedAt = now,
                ExpiresAt = now.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            });
            await _dbContext.SaveChangesAsync();

            return new AuthResult(accessToken, rawRefreshToken, now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes));
        }
    }
}

