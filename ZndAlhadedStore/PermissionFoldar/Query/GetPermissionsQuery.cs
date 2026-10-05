using MediatR;
using Shared.DTOs.Auth;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.PermissionFoldar.Query
{
    public record GetPermissionsQuery : IRequest<Result<List<PermissionDto>>>;
}
