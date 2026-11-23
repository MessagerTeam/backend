namespace Messenger.User.Api.Contracts;

/// <summary>Текущий профиль пользователя.</summary>
public sealed class UserMe
{
    public required Guid UserId { get; init; }
    public required string Username { get; init; }
    public required string DisplayName { get; init; }
    public string? AvatarUrl { get; init; }
    public required string Email { get; init; }
    public string? Bio { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>Краткая информация о пользователе.</summary>
public sealed class UserSummary
{
    public required Guid UserId { get; init; }
    public required string Username { get; init; }
    public required string DisplayName { get; init; }
    public string? AvatarUrl { get; init; }
}

/// <summary>Результат поиска пользователей.</summary>
public sealed class UserSearchResponse
{
    public required IReadOnlyList<UserSummary> Items { get; init; }
    public int? NextOffset { get; init; }
}
