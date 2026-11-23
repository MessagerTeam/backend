using System.ComponentModel.DataAnnotations;
using Messenger.Chat.Api.Contracts;
using Messenger.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Chat.Api.Controllers;

/// <summary>Отправка сообщений и история чата.</summary>
[ApiController]
[Authorize]
[Route("api/v1/chats/{chatId:guid}/messages")]
[Produces("application/json")]
public sealed class MessagesController : ControllerBase
{
    /// <summary>Отправляет сообщение в чат.</summary>
    /// <remarks>Повтор с тем же clientMessageId возвращает существующее сообщение с кодом 200.</remarks>
    /// <response code="201">Сообщение создано.</response>
    /// <response code="200">Сообщение уже было создано ранее.</response>
    /// <response code="400">Ошибка валидации.</response>
    /// <response code="401">Нет или истёк access token.</response>
    /// <response code="403">Пользователь не является участником чата.</response>
    /// <response code="404">Чат не найден.</response>
    /// <response code="501">Контракт подготовлен, бизнес-логика будет добавлена позже.</response>
    [HttpPost]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public ActionResult<MessageDto> Send(Guid chatId, [FromBody] SendMessageRequest request)
        => StatusCode(StatusCodes.Status501NotImplemented);

    /// <summary>Возвращает историю сообщений чата.</summary>
    /// <remarks>before и after взаимоисключающие; ответ всегда в возрастающем sequence.</remarks>
    /// <response code="200">История сообщений.</response>
    /// <response code="400">Некорректные cursor или limit.</response>
    /// <response code="401">Нет или истёк access token.</response>
    /// <response code="403">Пользователь не является участником чата.</response>
    /// <response code="404">Чат не найден.</response>
    /// <response code="501">Контракт подготовлен, бизнес-логика будет добавлена позже.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ChatHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public ActionResult<ChatHistoryResponse> GetHistory(
        Guid chatId,
        [FromQuery] long? before,
        [FromQuery] long? after,
        [FromQuery, Range(1, 100)] int limit = 50)
        => StatusCode(StatusCodes.Status501NotImplemented);
}
