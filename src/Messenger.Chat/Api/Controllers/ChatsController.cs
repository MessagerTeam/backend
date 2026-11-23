using System.ComponentModel.DataAnnotations;
using Messenger.Chat.Api.Contracts;
using Messenger.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.Chat.Api.Controllers;

/// <summary>Создание, список и детали чатов.</summary>
[ApiController]
[Authorize]
[Route("api/v1/chats")]
[Produces("application/json")]
public sealed class ChatsController : ControllerBase
{
    /// <summary>Создаёт direct-чат с одним пользователем.</summary>
    /// <remarks>Повторный вызов для той же пары возвращает существующий чат с кодом 200.</remarks>
    /// <response code="201">Чат создан.</response>
    /// <response code="200">Чат уже существовал.</response>
    /// <response code="400">Некорректный запрос.</response>
    /// <response code="401">Нет или истёк access token.</response>
    /// <response code="404">Пользователь не найден.</response>
    /// <response code="503">User Service недоступен.</response>
    /// <response code="501">Контракт подготовлен, бизнес-логика будет добавлена позже.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ChatSummary), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ChatSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public ActionResult<ChatSummary> Create([FromBody] CreateChatRequest request)
        => StatusCode(StatusCodes.Status501NotImplemented);

    /// <summary>Возвращает список чатов пользователя.</summary>
    /// <response code="200">Список чатов.</response>
    /// <response code="400">Некорректный cursor или limit.</response>
    /// <response code="401">Нет или истёк access token.</response>
    /// <response code="501">Контракт подготовлен, бизнес-логика будет добавлена позже.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ChatListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public ActionResult<ChatListResponse> GetPagedChats(
        [FromQuery] string? cursor,
        [FromQuery, Range(1, 100)] int limit = 50)
        => StatusCode(StatusCodes.Status501NotImplemented);

    /// <summary>Возвращает детали чата.</summary>
    /// <response code="200">Чат найден.</response>
    /// <response code="401">Нет или истёк access token.</response>
    /// <response code="403">Пользователь не является участником чата.</response>
    /// <response code="404">Чат не найден.</response>
    /// <response code="501">Контракт подготовлен, бизнес-логика будет добавлена позже.</response>
    [HttpGet("{chatId:guid}")]
    [ProducesResponseType(typeof(ChatSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public ActionResult<ChatSummary> Get(Guid chatId)
        => StatusCode(StatusCodes.Status501NotImplemented);
}
