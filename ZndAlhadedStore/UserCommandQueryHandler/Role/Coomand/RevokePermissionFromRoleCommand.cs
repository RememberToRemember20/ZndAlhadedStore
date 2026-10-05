using MediatR;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.UserCommandQueryHandler.Role.Coomand
{
    public record RevokePermissionFromRoleCommand(string RoleId, int PermissionId) : IRequest<Result>;
}
