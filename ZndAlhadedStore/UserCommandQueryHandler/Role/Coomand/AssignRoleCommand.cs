using MediatR;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.UserCommandQueryHandler.Role.Coomand
{
    public record AssignRoleCommand(string TargetUserId, string RoleName) : IRequest<Result>;
}
