using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Messenger.Shared.Errors;
using Messenger.User.Api.Contracts;
using Messenger.User.Features.Users.GetMe;
using Messenger.User.Infrastructure.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.User.Api.Controllers;

/// <summary>Профиль и поиск пользователей.</summary>
[ApiController]
[Authorize]
[Route("api/v1/users")]
[Produces("application/json")]
public sealed class UsersController(GetMeHandler getMeHandler) : ControllerBase
{
    private const string SubClaim = "sub";

    /// <summary>Возвращает профиль текущего пользователя.</summary>
    /// <response code="200">Профиль найден.</response>
    /// <response code="401">Нет или истёк access token.</response>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserMe), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserMe>> GetMe(CancellationToken cancellationToken)
    {
        var subject = User.FindFirstValue(SubClaim);

        if (!Guid.TryParse(subject, out var userId))
        {
            throw new DomainException(
                StatusCodes.Status401Unauthorized,
                "Требуется авторизация");
        }

        return Ok(await getMeHandler.HandleAsync(userId, cancellationToken));
    }

    /// <summary>Ищет пользователей по username или displayName.</summary>
    /// <response code="200">Результаты поиска.</response>
    /// <response code="400">Некорректный q, limit или offset.</response>
    /// <response code="401">Нет или истёк access token.</response>
    /// <response code="501">Контракт подготовлен, бизнес-логика будет добавлена позже.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(UserSearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public ActionResult<UserSearchResponse> SearchPagedUsers(
        [FromQuery, Required, StringLength(50, MinimumLength = 2)] string q,
        [FromQuery, Range(1, 50)] int limit = 20,
        [FromQuery, Range(0, 1000)] int offset = 0)
        => StatusCode(StatusCodes.Status501NotImplemented);
}
