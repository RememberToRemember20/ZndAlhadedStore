using AuthKit.Common;
using MediatR;
using Shared.DTOs.Auth;


namespace AuthKit.Features.Auth.RefreshToken.Command
{
    public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<AuthResult>>;
    
}
