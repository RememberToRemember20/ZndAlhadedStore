using MediatR;
using Shared.DTOs.Auth;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.UserCommandQueryHandler.Command
{
    public record RegisterCommand(string Email, string Password, string FullName) : IRequest<Result<AuthResult>>;
}
