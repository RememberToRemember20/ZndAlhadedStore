using AuthKit.Common;
using AuthKit.Models;
using AuthKit.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Password.Command
{
    public record ChangePasswordCommand(string UserId, string CurrentPassword, string NewPassword)
    : IRequest<Result<AuthResult>>, IAuditableRequest
    {
        public AuditDescriptor Describe() => new(AuditEvents.PasswordChange, TargetId: UserId);
        public override string ToString() => nameof(ChangePasswordCommand);
    }
}
