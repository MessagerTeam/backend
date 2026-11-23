using System.ComponentModel.DataAnnotations;

namespace Messenger.User.Api.Contracts;

/// <summary>Запрос на регистрацию.</summary>
public sealed class RegisterRequest
{
    [Required]
    [StringLength(20, MinimumLength = 3)]
    [RegularExpression("^[a-z0-9_]{3,20}$")]
    public required string Username { get; init; }

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public required string Email { get; init; }

    [Required]
    [StringLength(128, MinimumLength = 8)]
    public required string Password { get; init; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public required string DisplayName { get; init; }
}

/// <summary>Запрос на вход.</summary>
public sealed class LoginRequest
{
    [Required]
    public required string Username { get; init; }

    [Required]
    public required string Password { get; init; }
}

/// <summary>Пользователь в ответе на аутентификацию.</summary>
public sealed class AuthUser
{
    public required Guid UserId { get; init; }
    public required string Username { get; init; }
    public required string DisplayName { get; init; }
    public string? AvatarUrl { get; init; }
    public required string Email { get; init; }
}

/// <summary>Ответ на регистрацию, вход или refresh.</summary>
public sealed class AuthResponse
{
    public required string AccessToken { get; init; }
    public string TokenType { get; init; } = "Bearer";
    public int ExpiresIn { get; init; } = 900;
    public required AuthUser User { get; init; }
}
