using System.Text.Json;

namespace Messenger.Contracts;

/// <summary>
/// Общий конверт для всех integration events, которые публикуются в RabbitMQ.
/// </summary>
public sealed record EventEnvelope(
    Guid EventId,
    string EventType,
    int EventVersion,
    DateTimeOffset OccurredAt,
    string Producer,
    string? CorrelationId,
    string? CausationId,
    JsonElement Payload);

public static class EventTypes
{
    public const string MessageSent = "MessageSent";
    public const string ChatCreated = "ChatCreated";
}
