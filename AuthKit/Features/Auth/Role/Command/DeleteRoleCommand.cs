using AuthKit.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Command
{
    public record DeleteRoleCommand<TKey>(TKey RoleId) : IRequest<Result>
     where TKey : IEquatable<TKey>;
}
