using AuthKit.Common;
using AuthKit.Entities;
using AuthKit.Features.Auth.Logout.Command;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace AuthKit.Features.Auth.Logout.Handler
{
    public class LogoutCommandHandler<TUser, TKey, TContext> : IRequestHandler<LogoutCommand, Result>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>
    where TContext : DbContext
    {
        private readonly TContext _dbContext;
        private readonly ITokenService<TUser, TKey> _tokenService;

        public LogoutCommandHandler(TContext dbContext, ITokenService<TUser, TKey> tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var userId = (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromString(request.UserId)!;
            var tokenHash = _tokenService.HashToken(request.RefreshToken);

            var storedToken = await _dbContext.Set<RefreshToken<TKey>>()
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash && rt.UserId.Equals(userId), cancellationToken);

            if (storedToken is not null && storedToken.RevokedAt is null)
            {
                storedToken.RevokedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Result.Success();
        }
    }
}
