using AuthKit.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.User.Query
{
    public record CurrentUserDto(
    string Id, string? Email, bool EmailConfirmed,
    IReadOnlyList<string> Roles, IReadOnlyList<string> Permissions);

    public record GetCurrentUserQuery : IRequest<Result<CurrentUserDto>>;

}
