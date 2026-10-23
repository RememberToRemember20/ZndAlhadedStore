using AuthKit.Common;
using AuthKit.Models;
using MediatR;



namespace AuthKit.Features.Auth.RefreshToken.Command
{
    public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<AuthResult>>;
    
}
