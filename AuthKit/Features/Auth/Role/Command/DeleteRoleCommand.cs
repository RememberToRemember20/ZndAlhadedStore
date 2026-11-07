using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Role.Command
{
    public record DeleteRoleCommand<TKey>(TKey RoleId) : IRequest<Result>, IAuditableRequest
     where TKey : IEquatable<TKey>
    {
        public AuditDescriptor Describe() => new(AuditEvents.RoleDelete, TargetId: RoleId?.ToString());
    }
}
