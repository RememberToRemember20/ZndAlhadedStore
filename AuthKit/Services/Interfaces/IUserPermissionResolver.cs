using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Services.Interfaces
{
    public interface IUserPermissionResolver<TKey> where TKey : IEquatable<TKey>
    {
        Task<IReadOnlyCollection<string>> GetUserPermissionsAsync(TKey userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<string>> GetRolePermissionsAsync(IReadOnlyCollection<TKey> roleIds, CancellationToken cancellationToken = default);
    }
}
