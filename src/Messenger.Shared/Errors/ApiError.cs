using System.Text.Json.Serialization;

namespace Messenger.Shared.Errors;

/// <summary>Единый формат ошибки API.</summary>
/// <param name="Status">HTTP-статус.</param>
/// <param name="Detail">Понятное человеку описание ошибки.</param>
/// <param name="CorrelationId">Идентификатор запроса для поиска в логах.</param>
/// <param name="Errors">Ошибки по полям; возвращаются только для 400.</param>
public sealed record ApiError(
    int Status,
    string Detail,
    string CorrelationId,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyDictionary<string, string[]>? Errors = null);
