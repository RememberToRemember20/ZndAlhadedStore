using MediatR;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.UserCommandQueryHandler.Command
{
    public record LogoutAllCommand(string UserId) : IRequest<Result>;
}
