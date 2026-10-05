using MediatR;
using Shared.DTOs;
using ZndAlhadedStore.Common;

namespace ZndAlhadedStore.UserCommandQueryHandler.Query
{
    public record GetUsersQuery : IRequest<Result<List<UserListItemDto>>>;
}
