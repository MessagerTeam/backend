using Messenger.Shared.Errors;
using Messenger.User.Api.Contracts;
using Messenger.User.Features.Auth.Login;
using Messenger.User.Features.Auth.Refresh;
using Messenger.User.Features.Auth.Register;
using Messenger.User.Infrastructure.Errors;
using Messenger.User.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.User.Api.Controllers;

/// <summary>Регистрация, вход и обновление токенов.</summary>
[ApiController]
[AllowAnonymous]
[Route("api/v1/auth")]
[Produces("application/json")]
public sealed class AuthController(
    RegisterHandler registerHandler,
    LoginHandler loginHandler,
    RefreshHandler refreshHandler,
    RefreshTokenCookieService refreshTokenCookie) : ControllerBase
{
    /// <summary>Регистрирует пользователя.</summary>
    /// <remarks>Access token возвращается в теле, refresh token — в HttpOnly cookie.</remarks>
    /// <response code="201">Пользователь создан.</response>
    /// <response code="400">Ошибка валидации полей.</response>
    /// <response code="409">Username или email уже заняты.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await registerHandler.HandleAsync(
            request,
            GetUserAgent(),
            HttpContext.Connection.RemoteIpAddress,
            cancellationToken);

        refreshTokenCookie.Append(Response, result.RefreshToken);

        return StatusCode(StatusCodes.Status201Created, result.Response);
    }

    /// <summary>Выполняет вход по username и паролю.</summary>
    /// <response code="200">Успешный вход.</response>
    /// <response code="400">Пустые username или пароль.</response>
    /// <response code="401">Неверный username или пароль.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await loginHandler.HandleAsync(
            request,
            GetUserAgent(),
            HttpContext.Connection.RemoteIpAddress,
            cancellationToken);

        refreshTokenCookie.Append(Response, result.RefreshToken);

        return Ok(result.Response);
    }

    /// <summary>Обновляет access token по refresh-cookie.</summary>
    /// <remarks>Refresh token передаётся только в HttpOnly cookie и в теле ответа не возвращается.</remarks>
    /// <response code="200">Новая пара токенов.</response>
    /// <response code="401">Refresh token истёк, отозван или использован повторно.</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Refresh(CancellationToken cancellationToken)
    {
        var rawToken = Request.Cookies[AuthConstants.RefreshCookieName];

        try
        {
            var result = await refreshHandler.HandleAsync(
                rawToken,
                GetUserAgent(),
                HttpContext.Connection.RemoteIpAddress,
                cancellationToken);

            refreshTokenCookie.Append(Response, result.RefreshToken);

            return Ok(result.Response);
        }
        catch (DomainException exception)
            when (exception.Status == StatusCodes.Status401Unauthorized)
        {
            refreshTokenCookie.Delete(Response);
            throw;
        }
    }

    private string? GetUserAgent()
    {
        var userAgent = Request.Headers.UserAgent.ToString();

        return string.IsNullOrWhiteSpace(userAgent) ? null : userAgent;
    }
}
