using FluentValidation;
using FluentValidation.HttpExtensions;
using TournamentBracket.Application.Competitions.Commands;
using TournamentBracket.Application.Competitors.Interfaces;

namespace TournamentBracket.Application.Competitions.Validators;

public class SetWinnerCommandValidator : AbstractValidator<SetWinnerCommand>
{
	public SetWinnerCommandValidator(ICompetitorService competitorService)
	{
		RuleFor(command => command.CompetitorId)
			.NotEmpty()
			.Must(g => Guid.TryParse(g.ToString(), out _))
			.MustAsync((_, id, ct) => CompetitorExist(competitorService, id, ct))
			.WithMessage((_, id) => $"The competitor with id {id} not found.")
			.AsNotFound();
	}

	private async Task<bool> CompetitorExist(ICompetitorService competitorService, Guid id,
		CancellationToken ct)
	{
		return (await competitorService.GetCompetitor(id, ct)).IsSuccess;
	}
}
