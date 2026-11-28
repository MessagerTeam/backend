using System.Net;
using FluentValidation;
using Messenger.User.Api.Contracts;
using Messenger.User.Features.Auth.Common;
using Messenger.User.Features.Common;
using Messenger.User.Infrastructure.Errors;
using Messenger.User.Infrastructure.Persistence;
using Messenger.User.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Messenger.User.Features.Auth.Login;

/// <summary>Вход по username и паролю.</summary>
public sealed class LoginHandler(
    UserDbContext db,
    IValidator<LoginRequest> validator,
    BcryptPasswordHasher passwordHasher,
    RefreshTokenService refreshTokenService,
    TokenService tokenService)
{
    private const string InvalidCredentials = "Неверный username или пароль.";

    private static readonly string DummyPasswordHash =
        new BcryptPasswordHasher().Hash("dummy-password-for-timing");

    public async Task<AuthResult> HandleAsync(
        LoginRequest request,
        string? userAgent,
        IPAddress? ip,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var username = request.Username.Trim().ToLowerInvariant();

        var user = await db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(
                candidate => candidate.Username.ToLower() == username,
                cancellationToken);

        if (user is null)
        {
            passwordHasher.Verify(request.Password, DummyPasswordHash);
            throw new DomainException(
                StatusCodes.Status401Unauthorized,
                InvalidCredentials);
        }

        var passwordValid = passwordHasher.Verify(request.Password, user.PasswordHash);

        // Удалённый пользователь не должен отличаться от неверного пароля.
        if (!passwordValid || user.DeletedAt is not null)
        {
            throw new DomainException(
                StatusCodes.Status401Unauthorized,
                InvalidCredentials);
        }

        var issued = refreshTokenService.Issue(
            user.Id,
            familyId: null,
            userAgent,
            ip);

        db.RefreshTokens.Add(issued.Entity);
        await db.SaveChangesAsync(cancellationToken);

        return AuthResult.Create(user, tokenService, issued.RawToken);
    }
}
