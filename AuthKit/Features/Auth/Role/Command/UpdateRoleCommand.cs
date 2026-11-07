using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Command
{
    public record UpdateRoleCommand<TRole, TKey>(TKey RoleId, string NewName) : IRequest<Result>, IAuditableRequest
    where TRole : IdentityRole<TKey> where TKey : IEquatable<TKey>
    {
        public AuditDescriptor Describe() => new(AuditEvents.RoleUpdate, TargetId: RoleId?.ToString(), Details: $"new_name={NewName}");
    }
}
