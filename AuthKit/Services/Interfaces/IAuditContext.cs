using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Interfaces
{
    public interface IAuditContext
    {
        string? ActorUserId { get; }
        string? IpAddress { get; }
        string? UserAgent { get; }

        /// للعمليات غير الموثّقة (مثل Refresh): الـ Handler يحدد هوية صاحب الجلسة بعد التحقق منها
        void SetActor(string userId);
    }
}
