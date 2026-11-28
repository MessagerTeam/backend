using Messenger.User.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.User.Extensions;

/// <summary>Подключение PostgreSQL и миграций User Service.</summary>
public static class PersistenceExtensions
{
    public static IServiceCollection AddUserPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("UsersDb")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:UsersDb is not configured.");

        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static WebApplication MigrateUserDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        db.Database.Migrate();

        return app;
    }
}
