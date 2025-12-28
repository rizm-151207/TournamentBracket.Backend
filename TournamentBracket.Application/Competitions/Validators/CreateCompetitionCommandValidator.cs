using System.Globalization;
using FluentValidation;
using FluentValidation.HttpExtensions;
using TournamentBracket.Application.Competitions.Commands;
using TournamentBracket.Application.Competitors.Interfaces;

namespace TournamentBracket.Application.Competitions.Validators;

public class CreateCompetitionCommandValidator : AbstractValidator<CreateCompetitionCommand>
{
    public CreateCompetitionCommandValidator(ICompetitorService competitorsService)
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
        When(command => command.CompetitorsIds != null,
            () =>
            {
                RuleForEach(command => command.CompetitorsIds)
                    .NotEmpty()
                    .Must(g => Guid.TryParse(g.ToString(), out _));
                RuleFor(command => command.CompetitorsIds)
                    .MustAsync((g, ct) => CompetitorsExist(competitorsService, g!, ct))
                    .WithMessage(_ => "Not all competitors are found")
                    .AsNotFound();
            });
    }

    private async Task<bool> CompetitorsExist(ICompetitorService competitorsService, List<Guid> competitorsIds,
        CancellationToken ct)
    {
        return (await competitorsService.GetCompetitorsById(competitorsIds, ct)).IsSuccess;
    }
}