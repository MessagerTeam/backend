using Messenger.User.Api.Contracts;
using Messenger.User.Features.Common;
using Messenger.User.Infrastructure.Security;
using UserEntity = Messenger.User.Domain.User;

namespace Messenger.User.Features.Auth.Common;

/// <summary>Результат auth-handler’а: ответ клиенту и raw refresh-токен для cookie.</summary>
public sealed record AuthResult(AuthResponse Response, string RefreshToken)
{
    public static AuthResult Create(
        UserEntity user,
        TokenService tokenService,
        string rawRefreshToken)
    {
        var accessToken = tokenService.CreateAccessToken(
            user.Id,
            user.Username,
            user.DisplayName);

        return new AuthResult(
            new AuthResponse
            {
                AccessToken = accessToken.Token,
                ExpiresIn = accessToken.ExpiresIn,
                User = user.ToAuthUser(),
            },
            rawRefreshToken);
    }
}
