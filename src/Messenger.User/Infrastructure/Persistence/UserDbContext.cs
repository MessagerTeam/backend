using Messenger.User.Domain;
using Microsoft.EntityFrameworkCore;
using UserEntity = Messenger.User.Domain.User;

namespace Messenger.User.Infrastructure.Persistence;

/// <summary>Unit of Work для базы <c>messenger_users</c>.</summary>
public sealed class UserDbContext(DbContextOptions<UserDbContext> options)
    : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
