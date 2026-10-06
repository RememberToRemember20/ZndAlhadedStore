using AuthKit.Common;
using MediatR;

namespace AuthKit.Features.Auth.Logout.Command
{
    public record LogoutCommand(string UserId, string RefreshToken) : IRequest<Result>;
}
