using AuthKit.Common;
using AuthKit.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.Audit.Query
{
    public record AuditLogDto(long Id, DateTime OccurredAtUtc, string EventType, bool Succeeded,
     string? FailureCode, string? ActorUserId, string? Subject, string? TargetId, string? IpAddress, string? Details);

    public record GetAuditLogsQuery(
        int Page = 1, int PageSize = 20,
        string? EventType = null, string? ActorUserId = null, bool? Succeeded = null,
        DateTime? FromUtc = null, DateTime? ToUtc = null) : IRequest<Result<PagedResult<AuditLogDto>>>;

}
