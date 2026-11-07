using AuthKit.Common;
using AuthKit.Models;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs.Auth;


namespace AuthKit.Features.Auth.Register.Command
{
    public record RegisterCommand<TUser, TKey>(TUser User, string Password)
      : IRequest<Result<RegisterResult>>, IAuditableRequest
      where TUser : IdentityUser<TKey>
      where TKey : IEquatable<TKey>
    {
        public AuditDescriptor Describe() => new(AuditEvents.Register, Subject: User?.Email);
        public override string ToString() => nameof(RegisterCommand<TUser, TKey>);
    }
}
