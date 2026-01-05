using System.Globalization;
using FluentValidation;
using TournamentBracket.Application.Competitions.Commands;


namespace TournamentBracket.Application.Competitions.Validators;

public class CreateCompetitionCommandValidator : AbstractValidator<CreateCompetitionCommand>
{
    public CreateCompetitionCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(256);
        RuleFor(command => command.Description)
            .NotEmpty();
        RuleFor(command => command.StartDateTime)
            .NotEmpty()
            .Must(d => DateTime.TryParse(d.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date) && date > DateTime.Now)
            .WithMessage(
                $"Start date must be a valid date, after {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
    }
}