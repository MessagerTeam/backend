using System.Buffers.Text;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Messenger.User.Domain;

namespace Messenger.User.Infrastructure.Security;

/// <summary>Новый refresh-токен: raw-значение для cookie и сущность для БД.</summary>
public sealed record IssuedRefreshToken(string RawToken, RefreshToken Entity);

/// <summary>Выпуск, ротация и отзыв refresh-токенов. В БД ничего не сохраняет.</summary>
public sealed class RefreshTokenService(TimeProvider timeProvider)
{
    public IssuedRefreshToken Issue(
        Guid userId,
        Guid? familyId,
        string? userAgent,
        IPAddress? ip)
    {
        var now = timeProvider.GetUtcNow();
        var rawToken = CreateRawToken();

        var entity = new RefreshToken
        {
            FamilyId = familyId ?? Guid.CreateVersion7(),
            UserId = userId,
            TokenHash = Hash(rawToken),
            CreatedAt = now,
            ExpiresAt = now.AddDays(AuthConstants.RefreshTokenDays),
            UserAgent = userAgent,
            Ip = ip,
        };

        return new IssuedRefreshToken(rawToken, entity);
    }

    public IssuedRefreshToken Rotate(
        RefreshToken current,
        string? userAgent,
        IPAddress? ip)
    {
        ArgumentNullException.ThrowIfNull(current);

        var issued = Issue(current.UserId, current.FamilyId, userAgent, ip);

        Revoke(current, timeProvider.GetUtcNow());
        current.ReplacedById = issued.Entity.Id;

        return issued;
    }

    public byte[] Hash(string rawToken) =>
        SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));

    public bool IsActive(RefreshToken token, DateTimeOffset now) =>
        token.RevokedAt is null && token.ExpiresAt > now;

    public void Revoke(RefreshToken token, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(token);
        token.RevokedAt ??= now;
    }

    public void RevokeFamily(IEnumerable<RefreshToken> tokens, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(tokens);

        foreach (var token in tokens)
        {
            Revoke(token, now);
        }
    }

    private static string CreateRawToken() =>
        Base64Url.EncodeToString(
            RandomNumberGenerator.GetBytes(AuthConstants.RefreshTokenBytes));
}
