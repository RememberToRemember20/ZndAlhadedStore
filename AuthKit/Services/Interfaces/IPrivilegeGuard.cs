using AuthKit.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Interfaces
{
    public interface IPrivilegeGuard<TKey> where TKey : IEquatable<TKey>
    {
        Task<Result> RequireCallerHoldsAsync(IReadOnlyCollection<string> permissionNames, CancellationToken cancellationToken);
        Task<Result> RequireCallerDominatesRoleAsync(TKey roleId, CancellationToken cancellationToken);
        Result EnsureRoleIsNotProtected(string roleName);
        Task<Result> EnsureNotRemovingLastHolderAsync(TKey userId, TKey roleId, CancellationToken cancellationToken);
        Task<Result> EnsureNotOrphaningPermissionAsync(TKey roleId, string permissionName, CancellationToken cancellationToken);
        /// يرفض تعطيل مستخدم هو آخر حامل فعّال لصلاحية حرجة
        Task<Result> EnsureNotDisablingLastHolderAsync(TKey userId, CancellationToken cancellationToken);
    }
}
