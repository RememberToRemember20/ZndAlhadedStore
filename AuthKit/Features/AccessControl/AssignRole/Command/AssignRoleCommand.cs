using MediatR;
using AuthKit.Common;
using MediatR;

namespace AuthKit.Features.AccessControl.AssignRole.Command
{
    public record AssignRoleCommand(string TargetUserId, string RoleName) : IRequest<Result>;
}
