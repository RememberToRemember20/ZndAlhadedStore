using MediatR;
using Shared.DTOs.Auth;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.Auth.Command
{
    public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<AuthResult>>;
    
}
