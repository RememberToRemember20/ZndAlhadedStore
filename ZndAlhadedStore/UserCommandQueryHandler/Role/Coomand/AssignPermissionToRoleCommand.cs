using MediatR;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.UserCommandQueryHandler.Role.Coomand
{
    public record AssignPermissionToRoleCommand(string RoleId, int PermissionId) : IRequest<Result>;
}
