using AuthKit.Entities;
using AuthKit.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Implementations
{
    public class EfAuditLogger<TContext> : IAuditLogger where TContext : DbContext
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EfAuditLogger<TContext>> _logger;

        public EfAuditLogger(IServiceScopeFactory scopeFactory, ILogger<EfAuditLogger<TContext>> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task LogAsync(AuditLog entry, CancellationToken cancellationToken = default)
        {
            try
            {
                // الحقول القادمة من العميل (User-Agent, البريد) تُقصّ قبل الحفظ، مش نعتمد على خطأ قاعدة البيانات
                entry.EventType = Cut(entry.EventType, 64)!;
                entry.FailureCode = Cut(entry.FailureCode, 64);
                entry.ActorUserId = Cut(entry.ActorUserId, 128);
                entry.Subject = Cut(entry.Subject, 256);
                entry.TargetId = Cut(entry.TargetId, 128);
                entry.IpAddress = Cut(entry.IpAddress, 45);
                entry.UserAgent = Cut(entry.UserAgent, 256);
                entry.Details = Cut(entry.Details, 512);

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TContext>();
                db.Set<AuditLog>().Add(entry);
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "فشل تسجيل حدث تدقيق من نوع {EventType}.", entry.EventType);
            }
        }

        private static string? Cut(string? value, int max) =>
            value is null || value.Length <= max ? value : value[..max];
    }
}
