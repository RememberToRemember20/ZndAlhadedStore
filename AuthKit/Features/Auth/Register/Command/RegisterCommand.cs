using AuthKit.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs.Auth;


namespace AuthKit.Features.Auth.Register.Command
{
    public record RegisterCommand<TUser, TKey>(TUser User, string Password) : IRequest<Result<AuthResult>>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey>;
}
