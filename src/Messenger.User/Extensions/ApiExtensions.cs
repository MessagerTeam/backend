using Messenger.Shared.Correlation;
using Messenger.Shared.Errors;
using Messenger.Shared.Health;
using Messenger.Shared.Swagger;
using Messenger.User.Infrastructure.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.User.Extensions;

/// <summary>HTTP-слой User Service: контроллеры, ошибки, Swagger и middleware.</summary>
public static class ApiExtensions
{
    private const string SwaggerTitle = "Messenger User API";
    private const string SwaggerDescription = "Регистрация, вход, профиль и поиск пользователей.";
    private const string SwaggerXmlFile = "Messenger.User.xml";

    public static IServiceCollection AddUserApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddExceptionHandler<ApiExceptionHandler>();
        services.AddProblemDetails();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            // ProblemDetails нужен UseExceptionHandler без пути, но 404/501 отдаём
            // сами через UseStatusCodePages в формате ApiError.
            options.SuppressMapClientErrors = true;

            options.InvalidModelStateResponseFactory = context =>
                new BadRequestObjectResult(
                    ApiErrorFactory.FromModelState(context.HttpContext, context.ModelState));
        });

        services.AddMessengerSwagger(
            configuration,
            SwaggerTitle,
            SwaggerDescription,
            SwaggerXmlFile);

        return services;
    }

    public static WebApplication UseUserApi(this WebApplication app)
    {
        app.UseCorrelationId();
        app.UseExceptionHandler();
        app.UseStatusCodePages(async context =>
        {
            var response = context.HttpContext.Response;

            if (response.StatusCode is not (
                StatusCodes.Status404NotFound or
                StatusCodes.Status501NotImplemented))
            {
                return;
            }

            var correlationId =
                context.HttpContext.Items[CorrelationIdMiddleware.ItemKey] as string
                ?? context.HttpContext.TraceIdentifier;

            var detail = response.StatusCode == StatusCodes.Status404NotFound
                ? "Ресурс не найден"
                : "Функция пока не реализована";

            await response.WriteAsJsonAsync(new ApiError(
                response.StatusCode,
                detail,
                correlationId));
        });

        app.UseMessengerSwagger(SwaggerTitle);
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication MapUserEndpoints(this WebApplication app)
    {
        app.MapMessengerHealthChecks();
        app.MapControllers();

        return app;
    }
}
