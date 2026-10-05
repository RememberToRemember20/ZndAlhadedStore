using MediatR;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.UserCommandQueryHandler.Command
{
    public record LogoutCommand(string UserId, string RefreshToken) : IRequest<Result>;
}
