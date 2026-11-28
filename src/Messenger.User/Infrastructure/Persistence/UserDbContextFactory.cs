using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Messenger.User.Infrastructure.Persistence;

/// <summary>
/// Создаёт <see cref="UserDbContext"/> для <c>dotnet ef</c> без запуска приложения
/// и без обязательного JWT-секрета.
/// </summary>
public sealed class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    private const string PlaceholderConnectionString =
        "Host=localhost;Database=messenger_users;Username=user_service;Password=placeholder";

    public UserDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__UsersDb")
            ?? PlaceholderConnectionString;

        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new UserDbContext(options);
    }
}
