using FluentValidation;
using FluentValidation.HttpExtensions;
using TournamentBracket.Application.Competitors.Commands;
using TournamentBracket.Application.Competitors.Interfaces;

namespace TournamentBracket.Application.Competitors.Validators;

public class UpdateTrainerCommandValidator : AbstractValidator<UpdateTrainerCommand>
{
    public UpdateTrainerCommandValidator(ITrainerService trainerService)
    {
        Include(new CreateTrainerCommandValidator());
        RuleFor(c => c.Id)
            .NotEmpty()
            .Must(id => Guid.TryParse(id.ToString(), out _))
            .MustAsync((_, id, ct) => TrainerExist(trainerService, id, ct))
            .WithMessage((_, id) => $"The trainer with id {id} does not found.")
            .AsNotFound();
    }

    private async Task<bool> TrainerExist(ITrainerService trainerService, Guid id, CancellationToken ct)
    {
        return (await trainerService.GetTrainer(id, ct)).IsSuccess;
    }
}