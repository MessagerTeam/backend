using FluentValidation;
using Messenger.User.Features.Auth.Register;
using Messenger.User.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.User.Extensions;

/// <summary>Регистрация auth-сервисов и валидаторов User Service.</summary>
public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddUserAuth(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<BcryptPasswordHasher>();
        services.AddSingleton<TokenService>();
        services.AddSingleton<RefreshTokenService>();
        services.AddSingleton<RefreshTokenCookieService>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

        return services;
    }
}
