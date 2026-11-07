using AuthKit.Common;
using AuthKit.Models;
using AuthKit.Services.Interfaces;
using MediatR;



namespace AuthKit.Features.Auth.Login.Command
{
    public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResult>>, IAuditableRequest
    {
        public AuditDescriptor Describe() => new(AuditEvents.Login, Subject: Email);
        public override string ToString() => nameof(LoginCommand);
    }
}
