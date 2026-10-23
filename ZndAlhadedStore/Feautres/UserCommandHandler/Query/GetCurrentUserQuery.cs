using AuthKit.Common;
using MediatR;
using Shared.DTOs.Auth;

namespace ZndAlhadedStore.Feautres.UserCommandHandler.Query
{
    public record GetCurrentUserQuery(string UserId) : IRequest<Result<CurrentUserDto>>;

}
