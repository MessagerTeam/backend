using System.Text;
using Messenger.Shared.Correlation;
using Messenger.Shared.Errors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Messenger.Shared.Auth;

public static class AuthenticationExtensions
{
    /// <summary>
    /// Подключает общую проверку JWT для Gateway и всех сервисов.
    /// Access token читается из заголовка Authorization, а для SignalR —
    /// ещё и из query-параметра access_token.
    /// </summary>
    public static IServiceCollection AddMessengerJwt(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection("Jwt");
        var secret = section["Secret"];

        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException(
                "Jwt:Secret is not configured. Set Jwt__Secret environment variable.");
        }

        if (Encoding.UTF8.GetByteCount(secret) < 32)
        {
            throw new InvalidOperationException(
                "Jwt:Secret must be at least 32 bytes (256 bits).");
        }

        var issuer = section["Issuer"] ?? "messenger";
        var audience = section["Audience"] ?? "messenger-api";

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    NameClaimType = "sub",
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(accessToken)
                            && context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },

                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        var correlationId =
                            context.HttpContext.Items[CorrelationIdMiddleware.ItemKey] as string
                            ?? context.HttpContext.TraceIdentifier;

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.Headers["WWW-Authenticate"] = "Bearer";

                        await context.Response.WriteAsJsonAsync(new ApiError(
                            StatusCodes.Status401Unauthorized,
                            "Требуется авторизация",
                            correlationId));
                    },
                };
            });

        services.AddAuthorization();
        return services;
    }
}
