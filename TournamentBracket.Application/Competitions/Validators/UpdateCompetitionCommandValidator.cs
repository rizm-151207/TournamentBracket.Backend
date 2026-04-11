using FluentValidation;
using FluentValidation.HttpExtensions;
using TournamentBracket.Application.Competitions.Commands;
using TournamentBracket.Application.Competitions.Interfaces;

namespace TournamentBracket.Application.Competitions.Validators;

public class UpdateCompetitionCommandValidator : AbstractValidator<UpdateCompetitionCommand>
{
	public UpdateCompetitionCommandValidator(ICompetitionsService competitionsService)
	{
		Include(new CreateCompetitionCommandValidator());
		RuleFor(command => command.Id)
			.NotEmpty()
			.Must(g => Guid.TryParse(g.ToString(), out _))
			.MustAsync((_, id, ct) => CompetitionExist(competitionsService, id, ct))
			.WithMessage((_, id) => $"The competition with id {id} not found.")
			.AsNotFound();
		RuleFor(command => command.Status)
			.IsInEnum();
	}

	private async Task<bool> CompetitionExist(ICompetitionsService competitionsService, Guid id,
		CancellationToken ct)
	{
		return (await competitionsService.GetCompetitionWithoutCompetitors(id, ct)).IsSuccess;
	}
}