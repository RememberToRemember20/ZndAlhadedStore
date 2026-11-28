using AuthKit.Common;
using AuthKit.Entities;
using AuthKit.Features.Auth.Logout.Handler;
using AuthKit.Features.Auth.RefreshToken.Command;
using AuthKit.Models;
using AuthKit.Services.Interfaces;
using AuthKit.Settings;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace AuthKit.Features.Auth.RefreshToken.Handler
{

    internal sealed class RefreshTokenCommandHandler<TUser, TKey, TContext> : IRequestHandler<RefreshTokenCommand, Result<AuthResult>>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TContext : DbContext
    {
        private readonly ITokenService<TUser, TKey> _tokenService;
        private readonly UserManager<TUser> _userManager;
        private readonly TContext _dbContext;
        private readonly IAuthTokenIssuer<TUser, TKey> _tokenIssuer;
        private readonly IAuditContext _audit;
        private readonly AuthKitOptions _options;
        private readonly TimeProvider _time;
        private readonly ILogger<RefreshTokenCommandHandler<TUser, TKey, TContext>> _logger;
        private readonly IUserAccessGuard<TUser>? _accessGuard;

        public RefreshTokenCommandHandler(
            ITokenService<TUser, TKey> tokenService, UserManager<TUser> userManager, TContext dbContext,
            IAuthTokenIssuer<TUser, TKey> tokenIssuer, IAuditContext audit, IOptions<AuthKitOptions> options,
            TimeProvider time, ILogger<RefreshTokenCommandHandler<TUser, TKey, TContext>> logger,
            IUserAccessGuard<TUser>? accessGuard = null)
        {
            _tokenService = tokenService; _userManager = userManager; _dbContext = dbContext;
            _tokenIssuer = tokenIssuer; _audit = audit; _options = options.Value;
            _time = time; _logger = logger; _accessGuard = accessGuard;
        }

        public async Task<Result<AuthResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            string? claimedUserId = null;

            // اختياري: لو اتبعت، لازم يكون لنفس صاحب الـ Refresh Token
            if (!string.IsNullOrEmpty(request.AccessToken))
            {
                try
                {
                    claimedUserId = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken)
                        ?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                }
                catch (Exception ex) when (ex is SecurityTokenException or ArgumentException)
                {
                    _logger.LogWarning(ex, "Access token validation failed during refresh.");
                }

                if (string.IsNullOrEmpty(claimedUserId))
                    return Result<AuthResult>.Failure(AuthErrorCodes.InvalidToken);
            }

            var hash = _tokenService.HashToken(request.RefreshToken);
            var stored = await _dbContext.Set<RefreshToken<TKey>>()
                .FirstOrDefaultAsync(rt => rt.TokenHash == hash, cancellationToken);
            if (stored is null)
                return Result<AuthResult>.Failure(AuthErrorCodes.InvalidToken);

            var user = await _userManager.FindByIdAsync(stored.UserId.ToString()!);
            if (user is null)
                return Result<AuthResult>.Failure(AuthErrorCodes.AccountUnavailable);

            var userId = user.Id.ToString()!;
            if (claimedUserId is not null && !string.Equals(claimedUserId, userId, StringComparison.Ordinal))
                return Result<AuthResult>.Failure(AuthErrorCodes.InvalidToken);

            _audit.SetActor(userId);

            if (stored.RevokedAt is not null)
            {
                await _tokenIssuer.RevokeAllActiveTokensAsync(user.Id, cancellationToken);
                _logger.LogWarning("Refresh token reuse detected for user {UserId}.", userId);
                return Result<AuthResult>.Failure(AuthErrorCodes.TokenReuseDetected);
            }

            if (!stored.IsActiveAt(_time.GetUtcNow().UtcDateTime))
                return Result<AuthResult>.Failure(AuthErrorCodes.SessionExpired);

            if (_accessGuard is not null && !_accessGuard.CanSignIn(user))
                return Result<AuthResult>.Failure(AuthErrorCodes.AccountUnavailable);

            if (_options.RequireConfirmedEmail && !await _userManager.IsEmailConfirmedAsync(user))
                return Result<AuthResult>.Failure(AuthErrorCodes.EmailNotConfirmed);

            var result = await _tokenIssuer.RotateTokensAsync(user, stored);
            return result is null
                ? Result<AuthResult>.Failure(AuthErrorCodes.InvalidToken)
                : Result<AuthResult>.Success(result);
        }
    }
}

