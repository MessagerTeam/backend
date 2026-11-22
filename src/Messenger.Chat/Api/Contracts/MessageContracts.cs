using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Messenger.Chat.Api.Contracts;

/// <summary>Тип содержимого сообщения.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MessageContentType
{
    Text,
    Image,
    Voice,
    File,
    Video,
    Album,
}

/// <summary>Запрос на отправку сообщения.</summary>
public sealed class SendMessageRequest
{
    public required Guid ClientMessageId { get; init; }
    public MessageContentType ContentType { get; init; } = MessageContentType.Text;

    [StringLength(4096)]
    public string? Text { get; init; }

    public IReadOnlyList<Guid> MediaIds { get; init; } = [];
}

/// <summary>Вложение сообщения.</summary>
public sealed class AttachmentDto
{
    public required Guid MediaId { get; init; }
    public required string Url { get; init; }
    public required string FileName { get; init; }
    public required string MediaType { get; init; }
    public long SizeBytes { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public decimal? DurationSeconds { get; init; }
}

/// <summary>Сообщение.</summary>
public sealed class MessageDto
{
    /// <summary>Идентификатор сообщения на сервере.</summary>
    public required Guid MessageId { get; init; }

    public required Guid ChatId { get; init; }

    /// <summary>Монотонный номер внутри чата: порядок сообщений и догон истории.</summary>
    public long Sequence { get; init; }

    public required UserSummary Sender { get; init; }
    public MessageContentType ContentType { get; init; }
    public string? Text { get; init; }
    public IReadOnlyList<AttachmentDto> Attachments { get; init; } = [];

    /// <summary>Клиентский идентификатор для идемпотентности и оптимистичного UI.</summary>
    public required Guid ClientMessageId { get; init; }

    public DateTimeOffset SentAt { get; init; }
    public DateTimeOffset? EditedAt { get; init; }
    public DateTimeOffset? DeletedAt { get; init; }
    public bool DeletedForEveryone { get; init; }
    public bool HiddenForMe { get; init; }
}

/// <summary>История сообщений с курсорами.</summary>
public sealed class ChatHistoryResponse
{
    public required IReadOnlyList<MessageDto> Items { get; init; }
    public string? NextCursor { get; init; }
    public string? PrevCursor { get; init; }
    public bool HasMore { get; init; }
}
