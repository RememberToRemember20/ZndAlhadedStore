using AuthKit.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Interfaces
{
    public interface IAuditLogger
    {
        /// التنفيذ المعتمد لا يجب أن يرمي استثناءً أبداً
        Task LogAsync(AuditLog entry, CancellationToken cancellationToken = default);
    }
}
