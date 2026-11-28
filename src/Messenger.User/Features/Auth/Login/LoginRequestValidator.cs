using FluentValidation;
using Messenger.User.Api.Contracts;

namespace Messenger.User.Features.Auth.Login;

/// <summary>Правила входа.</summary>
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Username)
            .NotEmpty().WithMessage("Username обязателен.");

        RuleFor(request => request.Password)
            .NotEmpty().WithMessage("Пароль обязателен.");
    }
}
