using MediatR;
using Shared.DTOs.Auth;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.UserCommandQueryHandler.Command
{
    public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResult>>;
}
