using AuthKit.Common;
using AuthKit.Services.Interfaces;
using MediatR;
using MediatR;

namespace AuthKit.Features.AccessControl.AssignRole.Command
{
    public record AssignRoleCommand(string TargetUserId, string RoleName) : IRequest<Result>, IAuditableRequest
    {
        public AuditDescriptor Describe() => new(AuditEvents.UserRoleAssign, TargetId: TargetUserId, Details: $"role={RoleName}");
    }
}
