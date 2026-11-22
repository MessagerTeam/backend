using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Messenger.Shared.Swagger;

/// <summary>Общая настройка Swagger для сервисов с REST-контроллерами.</summary>
public static class SwaggerExtensions
{
    /// <summary>Подключает Swagger и общие настройки JWT/XML-комментариев.</summary>
    public static IServiceCollection AddMessengerSwagger(
        this IServiceCollection services,
        string title,
        string description,
        string serviceXmlFile)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = title,
                Version = "v1",
                Description = description,
            });

            options.SupportNonNullableReferenceTypes();
            options.NonNullableReferenceTypesAsRequired();

            options.AddServer(new()
            {
                Url = "http://localhost:5000",
                Description = "Gateway (локальный запуск)",
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Access token в формате: Bearer {token}",
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = [],
            });

            options.OperationFilter<AnonymousOperationFilter>();

            foreach (var xmlFile in new[] { serviceXmlFile, "Messenger.Shared.xml" })
            {
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            }
        });

        return services;
    }

    /// <summary>Включает Swagger UI только в Development.</summary>
    public static WebApplication UseMessengerSwagger(this WebApplication app, string title)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{title} v1");
        });

        return app;
    }
}
