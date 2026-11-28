using FluentValidation;
using Messenger.Shared.Correlation;
using Messenger.Shared.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Messenger.User.Infrastructure.Errors;

/// <summary>Сборка <see cref="ApiError"/> из исключений и ModelState.</summary>
internal static class ApiErrorFactory
{
    private const string ValidationDetail = "Проверьте поля запроса";
    private const string InternalDetail = "Внутренняя ошибка сервера";

    public static string GetCorrelationId(HttpContext context) =>
        context.Items[CorrelationIdMiddleware.ItemKey] as string
        ?? context.TraceIdentifier;

    public static ApiError FromValidationException(
        HttpContext context,
        ValidationException exception) =>
        new(
            StatusCodes.Status400BadRequest,
            ValidationDetail,
            GetCorrelationId(context),
            ToErrors(exception.Errors.Select(failure =>
                (failure.PropertyName, failure.ErrorMessage))));

    public static ApiError FromModelState(
        HttpContext context,
        ModelStateDictionary modelState) =>
        new(
            StatusCodes.Status400BadRequest,
            ValidationDetail,
            GetCorrelationId(context),
            ToErrors(modelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .SelectMany(entry => entry.Value!.Errors.Select(error => (
                    entry.Key,
                    Message: string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "Некорректное значение."
                        : error.ErrorMessage)))));

    public static ApiError Internal(HttpContext context) =>
        new(
            StatusCodes.Status500InternalServerError,
            InternalDetail,
            GetCorrelationId(context));

    private static IReadOnlyDictionary<string, string[]> ToErrors(
        IEnumerable<(string Key, string Message)> failures) =>
        failures
            .GroupBy(failure => ToCamelCase(failure.Key))
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(failure => failure.Message)
                    .Distinct()
                    .ToArray());

    private static string ToCamelCase(string key)
    {
        var name = key[(key.LastIndexOf('.') + 1)..];

        return string.IsNullOrEmpty(name) || char.IsLower(name[0])
            ? name
            : char.ToLowerInvariant(name[0]) + name[1..];
    }
}
