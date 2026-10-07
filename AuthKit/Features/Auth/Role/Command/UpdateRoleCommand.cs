using AuthKit.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Command
{
    public record UpdateRoleCommand<TRole, TKey>(TKey RoleId, string NewName) : IRequest<Result>
     where TRole : IdentityRole<TKey>
     where TKey : IEquatable<TKey>;
}
