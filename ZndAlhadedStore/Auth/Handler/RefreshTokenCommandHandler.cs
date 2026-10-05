using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Auth.Command;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.Identity;
using ZndAlhadedStore.Interfaces;

namespace ZndAlhadedStore.Auth.Handler
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResult>>
    {
    
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _dbContext;
        private readonly IAuthTokenIssuer _tokenIssuer;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            AppDbContext dbContext,
            IAuthTokenIssuer tokenIssuer,
            ILogger<RefreshTokenCommandHandler> logger)
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
                principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            }
            catch (Exception ex) when (ex is SecurityTokenException or ArgumentException)
            {
                return Result<AuthResult>.Failure("بيانات التوكن غير صالحة.");
            }

            var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Result<AuthResult>.Failure("بيانات التوكن غير صالحة.");

            var incomingHash = _tokenService.HashToken(request.RefreshToken);

            var storedToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.TokenHash == incomingHash, cancellationToken);

            if (storedToken is null || storedToken.UserId != userId)
                return Result<AuthResult>.Failure("جلسة غير صالحة، يرجى تسجيل الدخول مرة أخرى.");

            if (storedToken.RevokedAt is not null)
            {
                await _tokenIssuer.RevokeAllActiveTokensAsync(userId, cancellationToken);
                _logger.LogWarning("اكتشاف إعادة استخدام Refresh Token ملغى للمستخدم {UserId}.", userId);
                return Result<AuthResult>.Failure("تم اكتشاف نشاط مشبوه، تم إنهاء جميع الجلسات. يرجى تسجيل الدخول مرة أخرى.");
            }

            if (!storedToken.IsActive)
                return Result<AuthResult>.Failure("انتهت صلاحية الجلسة، يرجى تسجيل الدخول مرة أخرى.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null || !user.IsActive)
                return Result<AuthResult>.Failure("الحساب غير متاح.");

            var authResult = await _tokenIssuer.RotateTokensAsync(user, storedToken);
            return Result<AuthResult>.Success(authResult);
        }

        //private async Task RevokeAllUserTokensAsync(string userId, CancellationToken cancellationToken)
        //{
        //    var activeTokens = await _dbContext.RefreshTokens
        //        .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
        //        .ToListAsync(cancellationToken);

        //    foreach (var token in activeTokens)
        //        token.RevokedAt = DateTime.UtcNow;

        //    await _dbContext.SaveChangesAsync(cancellationToken);
        //}
    }
}
