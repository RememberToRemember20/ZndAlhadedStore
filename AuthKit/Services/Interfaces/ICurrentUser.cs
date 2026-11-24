using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Interfaces
{
    public interface ICurrentUser
    {
        string? UserId { get; }
        bool IsAuthenticated { get; }

        /// سياق نظام (Seeding، أدوات إدارية). يتخطى فحوصات الصلاحيات. التنفيذ الافتراضي دائماً false
        bool IsSystem { get; }

        bool HasPermission(string permission);
    }
}
