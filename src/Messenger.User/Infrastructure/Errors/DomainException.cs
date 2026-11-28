namespace Messenger.User.Infrastructure.Errors;

/// <summary>Доменная ошибка с HTTP-статусом и понятным описанием.</summary>
public sealed class DomainException(int status, string detail) : Exception(detail)
{
    public int Status { get; } = status;

    public string Detail { get; } = detail;
}
