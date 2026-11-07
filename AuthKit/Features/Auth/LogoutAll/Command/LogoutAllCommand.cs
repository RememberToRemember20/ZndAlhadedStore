using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;


namespace AuthKit.Features.Auth.LogoutAll.Command
{
    public record LogoutAllCommand(string UserId) : IRequest<Result>, IAuditableRequest
    {
        public AuditDescriptor Describe() => new(AuditEvents.LogoutAll, TargetId: UserId);
    }
}
