using AuthKit.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Command
{
    public record CreateRoleCommand<TRole, TKey>(TRole Role) : IRequest<Result<TKey>>
     where TRole : IdentityRole<TKey>
     where TKey : IEquatable<TKey>;
}
