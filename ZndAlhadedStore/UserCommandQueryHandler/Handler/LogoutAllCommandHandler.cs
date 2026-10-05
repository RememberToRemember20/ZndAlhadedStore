using MediatR;
using ZndAlhadedStore.Common;
using ZndAlhadedStore.Interfaces;
using ZndAlhadedStore.UserCommandQueryHandler.Command;

namespace ZndAlhadedStore.UserCommandQueryHandler.Handler
{
    public class LogoutAllCommandHandler : IRequestHandler<LogoutAllCommand, Result>
    {
        private readonly IAuthTokenIssuer _tokenIssuer;

        public LogoutAllCommandHandler(IAuthTokenIssuer tokenIssuer)
        {
            _tokenIssuer = tokenIssuer;
        }

        public async Task<Result> Handle(LogoutAllCommand request, CancellationToken cancellationToken)
        {
            await _tokenIssuer.RevokeAllActiveTokensAsync(request.UserId, cancellationToken);
            return Result.Success();
        }
    }
}
