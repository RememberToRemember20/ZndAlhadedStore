using AuthKit.Common;
using AuthKit.Models;
using AuthKit.Services.Interfaces;
using MediatR;



namespace AuthKit.Features.Auth.RefreshToken.Command
{
    public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<Result<AuthResult>>, IAuditableRequest
    {
        public AuditDescriptor Describe() => new(AuditEvents.Refresh);
        public override string ToString() => nameof(RefreshTokenCommand);
    }

}
