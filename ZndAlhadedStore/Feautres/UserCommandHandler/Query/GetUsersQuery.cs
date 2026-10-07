using AuthKit.Common;
using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.Feautres.UserCommandHandler.Query
{

    public record GetUsersQuery : IRequest<Result<List<UserListItemDto>>>;
}
