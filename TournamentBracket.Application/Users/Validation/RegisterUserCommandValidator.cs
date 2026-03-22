using FluentValidation;
using TournamentBracket.Application.Users.Commands;

namespace TournamentBracket.Application.Users.Validation;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(256);
        RuleFor(c => c.Role)
            .NotNull()
            .IsInEnum();
    }

}