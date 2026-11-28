using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Messenger.User.Infrastructure.Security;

/// <summary>Выпущенный access token и его время жизни в секундах.</summary>
public sealed record AccessTokenResult(string Token, int ExpiresIn);

/// <summary>Выпуск access JWT (HS256).</summary>
public sealed class TokenService
{
    private const int MinSecretBytes = 32;

    private readonly JsonWebTokenHandler _handler = new();
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SigningCredentials _signingCredentials;
    private readonly TimeProvider _timeProvider;

    public TokenService(IConfiguration configuration, TimeProvider timeProvider)
    {
        var section = configuration.GetSection("Jwt");
        var secret = section["Secret"];

        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException(
                "Jwt:Secret is not configured. Set Jwt__Secret environment variable.");
        }

        if (Encoding.UTF8.GetByteCount(secret) < MinSecretBytes)
        {
            throw new InvalidOperationException(
                "Jwt:Secret must be at least 32 bytes (256 bits).");
        }

        _issuer = section["Issuer"] ?? "messenger";
        _audience = section["Audience"] ?? "messenger-api";
        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256);
        _timeProvider = timeProvider;
    }

    public AccessTokenResult CreateAccessToken(Guid userId, string username, string displayName)
    {
        var now = _timeProvider.GetUtcNow();
        var expires = now.AddMinutes(AuthConstants.AccessTokenMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            IssuedAt = now.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Expires = expires.UtcDateTime,
            SigningCredentials = _signingCredentials,
            Claims = new Dictionary<string, object>
            {
                ["sub"] = userId.ToString(),
                ["username"] = username,
                ["displayName"] = displayName,
                ["jti"] = Guid.NewGuid().ToString(),
            },
        };

        var token = _handler.CreateToken(descriptor);
        return new AccessTokenResult(token, AuthConstants.AccessTokenMinutes * 60);
    }
}
