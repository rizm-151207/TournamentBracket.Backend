using FluentValidation;
using TournamentBracket.Application.Competitors.Commands;

namespace TournamentBracket.Application.Competitors.Validators;

public class CreateTrainerCommandValidator : AbstractValidator<CreateTrainerCommand>
{
	public CreateTrainerCommandValidator()
	{
		RuleFor(c => c.FirstName)
			.NotEmpty()
			.MaximumLength(100);
		RuleFor(c => c.LastName)
			.NotEmpty()
			.MaximumLength(100);
		RuleFor(c => c.MiddleName)
			.NotEmpty()
			.MaximumLength(100)
			.When(c => c.MiddleName is not null);
		RuleFor(c => c.Club)
			.NotEmpty()
			.MaximumLength(50)
			.When(c => c.Club is not null);
		RuleFor(c => c.Subject)
			.NotEmpty()
			.MaximumLength(100)
			.When(c => c.Subject is not null);
	}
}