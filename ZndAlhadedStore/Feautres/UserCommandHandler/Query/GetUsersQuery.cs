using AuthKit.Common;
using AuthKit.Models;
using MediatR;
using Shared.DTOs;

namespace ZndAlhadedStore.Feautres.UserCommandHandler.Query
{
    public record GetUsersQuery(int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<Result<PagedResult<UserListItemDto>>>;
}
