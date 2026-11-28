namespace Messenger.User.Infrastructure.Security;

/// <summary>Хеширование и проверка паролей через BCrypt.</summary>
public sealed class BcryptPasswordHasher
{
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, AuthConstants.BcryptWorkFactor);

    public bool Verify(string password, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
