using AuthKit.Common;
using AuthKit.Entities;
using AuthKit.Features.Auth.Logout.Handler;
using AuthKit.Features.Auth.RefreshToken.Command;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs.Auth;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace AuthKit.Features.Auth.RefreshToken.Handler
{
    public class RefreshTokenCommandHandler<TUser, TKey, TContext> : IRequestHandler<RefreshTokenCommand, Result<AuthResult>>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    where TContext : DbContext
    {
        private readonly ITokenService<TUser, TKey> _tokenService;
        private readonly UserManager<TUser> _userManager;
        private readonly TContext _dbContext;
        private readonly IAuthTokenIssuer<TUser, TKey> _tokenIssuer;
        private readonly ILogger<RefreshTokenCommandHandler<TUser, TKey, TContext>> _logger;

        public RefreshTokenCommandHandler(
            ITokenService<TUser, TKey> tokenService, UserManager<TUser> userManager, TContext dbContext,
            IAuthTokenIssuer<TUser, TKey> tokenIssuer, ILogger<RefreshTokenCommandHandler<TUser, TKey, TContext>> logger)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _dbContext = dbContext;
            _tokenIssuer = tokenIssuer;
            _logger = logger;
        }

        public async Task<Result<AuthResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            ClaimsPrincipal principal;
            try
            {
                principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken)!;
            }
            catch (Exception ex) when (ex is SecurityTokenException or ArgumentException)
            {
                _logger.LogWarning(ex, "فشل التحقق من الـ Access Token أثناء محاولة التجديد.");
                return Result<AuthResult>.Failure("بيانات التوكن غير صالحة.");
            }

            var userIdString = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                return Result<AuthResult>.Failure("بيانات التوكن غير صالحة.");

            var userId = (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromString(userIdString)!;
            var incomingHash = _tokenService.HashToken(request.RefreshToken);

            var storedToken = await _dbContext.Set<RefreshToken<TKey>>()
                .FirstOrDefaultAsync(rt => rt.TokenHash == incomingHash, cancellationToken);

            if (storedToken is null || !storedToken.UserId.Equals(userId))
                return Result<AuthResult>.Failure("جلسة غير صالحة، يرجى تسجيل الدخول مرة أخرى.");

            if (storedToken.RevokedAt is not null)
            {
                await _tokenIssuer.RevokeAllActiveTokensAsync(userId, cancellationToken);
                _logger.LogWarning("اكتشاف إعادة استخدام Refresh Token ملغى للمستخدم {UserId}.", userId);
                return Result<AuthResult>.Failure("تم اكتشاف نشاط مشبوه، تم إنهاء جميع الجلسات. يرجى تسجيل الدخول مرة أخرى.");
            }

            if (!storedToken.IsActive)
                return Result<AuthResult>.Failure("انتهت صلاحية الجلسة، يرجى تسجيل الدخول مرة أخرى.");

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user is null)
                return Result<AuthResult>.Failure("الحساب غير متاح.");

            var authResult = await _tokenIssuer.RotateTokensAsync(user, storedToken);
            return Result<AuthResult>.Success(authResult);
        }
    }
}
