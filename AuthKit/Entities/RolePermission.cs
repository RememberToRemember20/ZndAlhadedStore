namespace AuthKit.Entities
{
    public class RolePermission<TKey> where TKey : IEquatable<TKey>
    {
        public TKey RoleId { get; set; } = default!;
        public int PermissionId { get; set; }
    }
}