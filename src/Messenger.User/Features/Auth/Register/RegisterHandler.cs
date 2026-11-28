using System.Net;
using FluentValidation;
using Messenger.User.Api.Contracts;
using Messenger.User.Features.Auth.Common;
using Messenger.User.Features.Common;
using Messenger.User.Infrastructure.Errors;
using Messenger.User.Infrastructure.Persistence;
using Messenger.User.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using UserEntity = Messenger.User.Domain.User;

namespace Messenger.User.Features.Auth.Register;

/// <summary>Регистрация пользователя и выпуск первой пары токенов.</summary>
public sealed class RegisterHandler(
    UserDbContext db,
    IValidator<RegisterRequest> validator,
    BcryptPasswordHasher passwordHasher,
    RefreshTokenService refreshTokenService,
    TokenService tokenService,
    TimeProvider timeProvider)
{
    public async Task<AuthResult> HandleAsync(
        RegisterRequest request,
        string? userAgent,
        IPAddress? ip,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var username = request.Username.ToLowerInvariant();
        var email = request.Email.Trim().ToLowerInvariant();

        if (await db.Users.AnyAsync(
                user => user.Username.ToLower() == username,
                cancellationToken))
        {
            throw new DomainException(
                StatusCodes.Status409Conflict,
                "Username уже занят.");
        }

        if (await db.Users.AnyAsync(
                user => user.Email.ToLower() == email,
                cancellationToken))
        {
            throw new DomainException(
                StatusCodes.Status409Conflict,
                "Email уже занят.");
        }

        var now = timeProvider.GetUtcNow();

        var user = new UserEntity
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHasher.Hash(request.Password),
            DisplayName = request.DisplayName,
            CreatedAt = now,
            UpdatedAt = now,
        };

        var issued = refreshTokenService.Issue(
            user.Id,
            familyId: null,
            userAgent,
            ip);

        db.Users.Add(user);
        db.RefreshTokens.Add(issued.Entity);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (TryMapUniqueViolation(exception) is { } detail)
        {
            throw new DomainException(StatusCodes.Status409Conflict, detail);
        }

        return AuthResult.Create(user, tokenService, issued.RawToken);
    }

    private static string? TryMapUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
        } postgres
            ? postgres.ConstraintName switch
            {
                "ux_users_username" => "Username уже занят.",
                "ux_users_email" => "Email уже занят.",
                _ => null,
            }
            : null;
}
