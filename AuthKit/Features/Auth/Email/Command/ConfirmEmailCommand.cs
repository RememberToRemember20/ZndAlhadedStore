using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Email.Command
{
    public record ConfirmEmailCommand(string UserId, string Token) : IRequest<Result>, IAuditableRequest
    {
        public AuditDescriptor Describe() => new(AuditEvents.EmailConfirm, TargetId: UserId);
        public override string ToString() => nameof(ConfirmEmailCommand);
    }
}
