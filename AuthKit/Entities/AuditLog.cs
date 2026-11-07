using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Entities
{
    // AuthKit/Entities/AuditLog.cs
    public class AuditLog
    {
        public long Id { get; set; }
        public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
        public string EventType { get; set; } = string.Empty;
        public bool Succeeded { get; set; }
        public string? FailureCode { get; set; }
        public string? ActorUserId { get; set; }
        public string? Subject { get; set; }     // البريد المُستخدم في المحاولة (للعمليات غير الموثّقة)
        public string? TargetId { get; set; }    // المستخدم/الدور المتأثر
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Details { get; set; }
    }
}
