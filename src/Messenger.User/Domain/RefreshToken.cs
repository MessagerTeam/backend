using System.Net;

namespace Messenger.User.Domain;

/// <summary>Refresh-токен одной сессии пользователя.</summary>
public sealed class RefreshToken
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid FamilyId { get; set; } = Guid.CreateVersion7();
    public Guid UserId { get; set; }
    public byte[] TokenHash { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public Guid? ReplacedById { get; set; }
    public string? UserAgent { get; set; }
    public IPAddress? Ip { get; set; }
    public User User { get; set; } = null!;
    public RefreshToken? ReplacedBy { get; set; }
    public ICollection<RefreshToken> ReplacedByTokens { get; set; } = [];
}
