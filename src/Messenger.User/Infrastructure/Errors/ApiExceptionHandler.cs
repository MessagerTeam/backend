using FluentValidation;
using Messenger.Shared.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Messenger.User.Infrastructure.Errors;

/// <summary>Приводит все необработанные исключения к <see cref="ApiError"/>.</summary>
public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
        {
            logger.LogWarning("Response has already started; ApiError was not written.");
            return false;
        }

        ApiError error;

        switch (exception)
        {
            case ValidationException validationException:
                error = ApiErrorFactory.FromValidationException(
                    httpContext,
                    validationException);
                break;

            case DomainException domainException:
                error = new ApiError(
                    domainException.Status,
                    domainException.Detail,
                    ApiErrorFactory.GetCorrelationId(httpContext));
                break;

            default:
                logger.LogError(exception, "Unhandled exception.");
                error = ApiErrorFactory.Internal(httpContext);
                break;
        }

        httpContext.Response.StatusCode = error.Status;
        await httpContext.Response.WriteAsJsonAsync(error, cancellationToken);

        return true;
    }
}
