using MediatR;
using Shared.DTOs.Auth;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.UserCommandQueryHandler.Role.Query
{
    public record GetRolesQuery : IRequest<Result<List<RoleDto>>>;
}
