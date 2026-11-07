using AuthKit.Common;
using AuthKit.Entities;
using AuthKit.Features.AccessControl.Audit.Query;
using AuthKit.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Features.AccessControl.Audit.Handler
{
    public class GetAuditLogsQueryHandler<TContext> : IRequestHandler<GetAuditLogsQuery, Result<PagedResult<AuditLogDto>>>
     where TContext : DbContext
    {
        private readonly TContext _db;
        public GetAuditLogsQueryHandler(TContext db) => _db = db;

        public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsQuery r, CancellationToken ct)
        {
            var query = _db.Set<AuditLog>().AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(r.EventType)) query = query.Where(a => a.EventType == r.EventType);
            if (!string.IsNullOrWhiteSpace(r.ActorUserId)) query = query.Where(a => a.ActorUserId == r.ActorUserId);
            if (r.Succeeded is not null) query = query.Where(a => a.Succeeded == r.Succeeded);
            if (r.FromUtc is not null) query = query.Where(a => a.OccurredAtUtc >= r.FromUtc);
            if (r.ToUtc is not null) query = query.Where(a => a.OccurredAtUtc <= r.ToUtc);

            var total = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(a => a.OccurredAtUtc).ThenByDescending(a => a.Id)
                .Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)
                .Select(a => new AuditLogDto(a.Id, a.OccurredAtUtc, a.EventType, a.Succeeded, a.FailureCode,
                    a.ActorUserId, a.Subject, a.TargetId, a.IpAddress, a.Details))
                .ToListAsync(ct);

            return Result<PagedResult<AuditLogDto>>.Success(new PagedResult<AuditLogDto>(items, r.Page, r.PageSize, total));
        }
    }
}
