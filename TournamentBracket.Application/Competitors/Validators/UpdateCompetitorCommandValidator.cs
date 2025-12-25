using FluentValidation;
using FluentValidation.HttpExtensions;
using TournamentBracket.Application.Competitors.Commands;
using TournamentBracket.Application.Competitors.Interfaces;

namespace TournamentBracket.Application.Competitors.Validators;

public class UpdateCompetitorCommandValidator : AbstractValidator<UpdateCompetitorCommand>
{
    public UpdateCompetitorCommandValidator(ITrainerService trainerService, ICompetitorService competitorService)
    {
        Include(new CreateCompetitorCommandValidator(trainerService));
        RuleFor(c => c.Id)
            .NotEmpty()
            .Must(g => Guid.TryParse(g.ToString(), out _))
            .MustAsync((_, id, ct) => CompetitorExist(competitorService, id, ct))
            .WithMessage((_, id) => $"The competitor with id {id} does not found.")
            .AsNotFound();
    }

    private async Task<bool> CompetitorExist(ICompetitorService competitorService, Guid id, CancellationToken ct)
    {
        return (await competitorService.GetCompetitor(id, ct)).IsSuccess;
    }
}