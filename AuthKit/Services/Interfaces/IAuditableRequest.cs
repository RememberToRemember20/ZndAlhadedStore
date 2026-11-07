using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Interfaces
{
    public record AuditDescriptor(string EventType, string? Subject = null, string? TargetId = null, string? Details = null);

    public interface IAuditableRequest
    {
        AuditDescriptor Describe();
    }
}
