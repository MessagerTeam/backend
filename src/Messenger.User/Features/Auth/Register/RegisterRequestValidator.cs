using FluentValidation;
using Messenger.User.Api.Contracts;

namespace Messenger.User.Features.Auth.Register;

/// <summary>Правила регистрации.</summary>
public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(request => request.Username)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Username обязателен.")
            .Matches("^[a-z0-9_]{3,20}$").WithMessage(
                "Username должен содержать 3–20 символов: латиница в нижнем регистре, цифры и _.");

        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email обязателен.")
            .EmailAddress().WithMessage("Некорректный email.");

        RuleFor(request => request.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Пароль обязателен.")
            .Length(8, 128).WithMessage("Пароль должен содержать от 8 до 128 символов.");

        RuleFor(request => request.DisplayName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("DisplayName обязателен.")
            .Length(1, 50).WithMessage("DisplayName должен содержать от 1 до 50 символов.");
    }
}
