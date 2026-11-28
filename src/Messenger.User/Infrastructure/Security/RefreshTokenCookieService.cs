using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Messenger.User.Infrastructure.Security;

/// <summary>Установка и удаление refresh-cookie с едиными параметрами.</summary>
public sealed class RefreshTokenCookieService(IWebHostEnvironment environment)
{
    public void Append(HttpResponse response, string rawToken)
    {
        var options = CreateOptions();
        options.MaxAge = TimeSpan.FromDays(AuthConstants.RefreshTokenDays);

        response.Cookies.Append(AuthConstants.RefreshCookieName, rawToken, options);
    }

    public void Delete(HttpResponse response)
    {
        response.OnStarting(() =>
        {
            response.Cookies.Delete(AuthConstants.RefreshCookieName, CreateOptions());
            return Task.CompletedTask;
        });
    }

    private CookieOptions CreateOptions() => new()
    {
        HttpOnly = true,
        Secure = environment.IsProduction(),
        SameSite = SameSiteMode.Strict,
        Path = AuthConstants.RefreshCookiePath,
    };
}
