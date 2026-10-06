using AuthKit.Common;
using MediatR;


namespace AuthKit.Features.Auth.LogoutAll.Command
{
    public record LogoutAllCommand(string UserId) : IRequest<Result>;
}
