using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZndAlhadedStore.Feautres.UserCommandHandler.Command
{
    public record SetUserActiveCommand(string CurrentUserId, string TargetUserId, bool IsActive)
    : IRequest<Result>, IAuditableRequest
    {
        public AuditDescriptor Describe() => new("user.set_active", TargetId: TargetUserId, Details: $"active={IsActive}");
    }

}
