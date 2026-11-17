using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.InventUser.Command
{
    public record InviteUserCommand<TUser, TKey>(TUser User, IReadOnlyCollection<string> RoleNames, string InvitedByUserId)
     : IRequest<Result<string>>, IAuditableRequest
     where TUser : IdentityUser<TKey>
     where TKey : IEquatable<TKey>
    {
        public AuditDescriptor Describe() =>
            new(AuditEvents.UserInvite, Subject: User?.Email, Details: $"roles={string.Join(',', RoleNames ?? [])}");
    }

}
