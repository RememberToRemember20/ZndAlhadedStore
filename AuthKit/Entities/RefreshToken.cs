namespace AuthKit.Entities
{
    public class RefreshToken<TKey> where TKey : IEquatable<TKey>
    {
        public int Id { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public TKey UserId { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByTokenHash { get; set; }

        public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
    }
}
