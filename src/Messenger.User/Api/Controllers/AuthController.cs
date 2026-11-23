using Messenger.Shared.Errors;
using Messenger.User.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.User.Api.Controllers;

/// <summary>Регистрация, вход и обновление токенов.</summary>
[ApiController]
[AllowAnonymous]
[Route("api/v1/auth")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    /// <summary>Регистрирует пользователя.</summary>
    /// <remarks>Access token возвращается в теле, refresh token — в HttpOnly cookie.</remarks>
    /// <response code="201">Пользователь создан.</response>
    /// <response code="400">Ошибка валидации полей.</response>
    /// <response code="409">Username или email уже заняты.</response>
    /// <response code="501">Контракт подготовлен, бизнес-логика будет добавлена позже.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public ActionResult<AuthResponse> Register([FromBody] RegisterRequest request)
        => StatusCode(StatusCodes.Status501NotImplemented);

    /// <summary>Выполняет вход по username и паролю.</summary>
    /// <response code="200">Успешный вход.</response>
    /// <response code="401">Неверный username или пароль.</response>
    /// <response code="501">Контракт подготовлен, бизнес-логика будет добавлена позже.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public ActionResult<AuthResponse> Login([FromBody] LoginRequest request)
        => StatusCode(StatusCodes.Status501NotImplemented);

    /// <summary>Обновляет access token по refresh-cookie.</summary>
    /// <remarks>Refresh token передаётся только в HttpOnly cookie и в теле ответа не возвращается.</remarks>
    /// <response code="200">Новая пара токенов.</response>
    /// <response code="401">Refresh token истёк, отозван или использован повторно.</response>
    /// <response code="501">Контракт подготовлен, бизнес-логика будет добавлена позже.</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public ActionResult<AuthResponse> Refresh()
        => StatusCode(StatusCodes.Status501NotImplemented);
}
