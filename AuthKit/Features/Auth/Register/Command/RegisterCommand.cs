using AuthKit.Common;
using AuthKit.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs.Auth;


namespace AuthKit.Features.Auth.Register.Command
{
    public record RegisterCommand<TUser, TKey>(TUser User, string Password) : IRequest<Result<RegisterResult>>
     where TUser : IdentityUser<TKey>
     where TKey : IEquatable<TKey>;
}
