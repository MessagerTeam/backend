using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Messenger.Chat.Api.Contracts;

/// <summary>Тип чата.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChatType
{
    Direct,
    Group,
    Channel,
}

/// <summary>Роль участника чата.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MemberRole
{
    Owner,
    Admin,
    Member,
    Subscriber,
}

/// <summary>Краткая информация о пользователе в контексте чата.</summary>
public sealed class UserSummary
{
    public required Guid UserId { get; init; }
    public required string Username { get; init; }
    public required string DisplayName { get; init; }
    public string? AvatarUrl { get; init; }
}

/// <summary>Запрос на создание чата.</summary>
public sealed class CreateChatRequest
{
    public ChatType Type { get; init; } = ChatType.Direct;

    [Required]
    [MinLength(1)]
    public IReadOnlyList<Guid> ParticipantIds { get; init; } = [];

    [StringLength(100)]
    public string? Title { get; init; }
}

/// <summary>Краткая информация о чате.</summary>
public sealed class ChatSummary
{
    public required Guid ChatId { get; init; }
    public required ChatType Type { get; init; }
    public string? Title { get; init; }
    public string? AvatarUrl { get; init; }
    public required IReadOnlyList<UserSummary> Participants { get; init; }
    public required MemberRole MyRole { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>Элемент списка чатов.</summary>
public sealed class ChatListItem
{
    public required Guid ChatId { get; init; }
    public required ChatType Type { get; init; }

    /// <summary>Для Direct — имя собеседника, для Group/Channel — название чата.</summary>
    public required string DisplayTitle { get; init; }

    /// <summary>Для Direct — аватар собеседника, для Group/Channel — аватар чата.</summary>
    public string? DisplayAvatarUrl { get; init; }

    public required MemberRole MyRole { get; init; }
    public MessageDto? LastMessage { get; init; }
    public DateTimeOffset LastMessageAt { get; init; }

    /// <summary>null — статус неизвестен; true — прочитано получателем; false — не прочитано.</summary>
    public bool? LastMessageIsRead { get; init; }

    public int UnreadCount { get; init; }
}

/// <summary>Список чатов с курсором.</summary>
public sealed class ChatListResponse
{
    public required IReadOnlyList<ChatListItem> Items { get; init; }
    public string? NextCursor { get; init; }
}
