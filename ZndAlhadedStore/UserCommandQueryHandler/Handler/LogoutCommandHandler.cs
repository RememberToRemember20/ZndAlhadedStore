using MediatR;
using Microsoft.EntityFrameworkCore;
using ZndAlhadedStore.AppDB;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.Interfaces;
using ZndAlhadedStore.UserCommandQueryHandler.Command;

namespace ZndAlhadedStore.UserCommandQueryHandler.Handler
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly AppDbContext _dbContext;
        private readonly ITokenService _tokenService;

        public LogoutCommandHandler(AppDbContext dbContext, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var tokenHash = _tokenService.HashToken(request.RefreshToken);

            var storedToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash && rt.UserId == request.UserId, cancellationToken);

            if (storedToken is not null && storedToken.RevokedAt is null)
            {
                storedToken.RevokedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Result.Success();
        }
    }
}
