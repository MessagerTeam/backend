namespace Messenger.User.Infrastructure.Security;

/// <summary>Бизнес-константы аутентификации.</summary>
public static class AuthConstants
{
    public const int AccessTokenMinutes = 15;
    public const int RefreshTokenDays = 30;
    public const int RefreshTokenBytes = 32;
    public const int BcryptWorkFactor = 12;

    public const string RefreshCookieName = "refresh_token";
    public const string RefreshCookiePath = "/api/v1/auth";
}
