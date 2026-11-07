using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Password.Command
{
    public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result>, IAuditableRequest
    {
        public AuditDescriptor Describe() => new(AuditEvents.PasswordReset, Subject: Email);
        public override string ToString() => nameof(ResetPasswordCommand);
    }
}
