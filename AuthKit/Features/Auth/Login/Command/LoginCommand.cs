using AuthKit.Common;
using MediatR;
using Shared.DTOs.Auth;


namespace AuthKit.Features.Auth.Login.Command
{
    public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResult>>;
}
