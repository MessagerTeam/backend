using System.Net;
using Messenger.User.Features.Auth.Common;
using Messenger.User.Features.Common;
using Messenger.User.Infrastructure.Errors;
using Messenger.User.Infrastructure.Persistence;
using Messenger.User.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Messenger.User.Features.Auth.Refresh;

/// <summary>Ротация refresh-токена и выпуск новой пары токенов.</summary>
public sealed class RefreshHandler(
    UserDbContext db,
    RefreshTokenService refreshTokenService,
    TokenService tokenService,
    TimeProvider timeProvider)
{
    private const string SessionNotFound = "Сессия не найдена или истекла.";

    public async Task<AuthResult> HandleAsync(
        string? rawToken,
        string? userAgent,
        IPAddress? ip,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
        {
            throw Unauthorized();
        }

        var tokenHash = refreshTokenService.Hash(rawToken);

        var token = await db.RefreshTokens
            .Include(candidate => candidate.User)
            .SingleOrDefaultAsync(
                candidate => candidate.TokenHash == tokenHash,
                cancellationToken);

        if (token is null)
        {
            throw Unauthorized();
        }

        if (token.User.DeletedAt is not null)
        {
            throw Unauthorized();
        }

        var now = timeProvider.GetUtcNow();

        if (token.RevokedAt is not null)
        {
            var family = await db.RefreshTokens
                .Where(candidate => candidate.FamilyId == token.FamilyId)
                .ToListAsync(cancellationToken);

            refreshTokenService.RevokeFamily(family, now);
            await db.SaveChangesAsync(cancellationToken);

            throw Unauthorized();
        }

        if (!refreshTokenService.IsActive(token, now))
        {
            throw Unauthorized();
        }

        var issued = refreshTokenService.Rotate(token, userAgent, ip);

        db.RefreshTokens.Add(issued.Entity);
        await db.SaveChangesAsync(cancellationToken);

        return AuthResult.Create(token.User, tokenService, issued.RawToken);
    }

    private static DomainException Unauthorized() =>
        new(StatusCodes.Status401Unauthorized, SessionNotFound);
}
