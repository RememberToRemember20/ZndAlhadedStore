using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;

namespace AuthKit.Features.Auth.Logout.Command
{
    public record LogoutCommand(string UserId, string RefreshToken) : IRequest<Result>, IAuditableRequest
    {
        public AuditDescriptor Describe() => new(AuditEvents.Logout, TargetId: UserId);
        public override string ToString() => nameof(LogoutCommand);
    }
}
