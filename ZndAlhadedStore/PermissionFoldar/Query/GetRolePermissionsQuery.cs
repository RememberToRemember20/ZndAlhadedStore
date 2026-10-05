using MediatR;
using Shared.DTOs.Auth;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.PermissionFoldar.Query
{
    public record GetRolePermissionsQuery(string RoleId) : IRequest<Result<List<PermissionDto>>>;
}
