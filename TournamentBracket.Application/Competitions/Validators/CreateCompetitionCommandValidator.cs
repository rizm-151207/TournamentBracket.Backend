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
        RuleFor(command => command.Location)
            .NotEmpty();
        RuleFor(command => command.TatamiCount)
            .NotEmpty()
            .GreaterThan(0);
        RuleFor(command => command.StartDateTime)
            .NotEmpty()
            .Must(d => DateTime.TryParse(d.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var date) && date > DateTime.UtcNow)
            .WithMessage(
                $"Start date must be a valid date, after {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)} by UTC");
    }
}