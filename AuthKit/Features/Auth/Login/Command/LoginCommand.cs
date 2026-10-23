using AuthKit.Common;
using AuthKit.Models;
using MediatR;



namespace AuthKit.Features.Auth.Login.Command
{
    public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResult>>;
}
