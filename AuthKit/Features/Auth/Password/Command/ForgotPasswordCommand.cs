using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.Auth.Password.Command
{
    public record ForgotPasswordCommand(string Email) : IRequest<Result>, IAuditableRequest
    {
        public AuditDescriptor Describe() => new(AuditEvents.PasswordForgot, Subject: Email);
    }
}
