using FluentValidation;
using TournamentBracket.Application.Users.Commands;

namespace TournamentBracket.Application.Users.Validation;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(c => c.RefreshToken)
            .NotEmpty();
    }
}