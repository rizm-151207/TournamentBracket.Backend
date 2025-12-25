using System.Globalization;
using FluentValidation;
using FluentValidation.HttpExtensions;
using TournamentBracket.Application.Competitors.Commands;
using TournamentBracket.Application.Competitors.Interfaces;

namespace TournamentBracket.Application.Competitors.Validators;

public class CreateCompetitorCommandValidator : AbstractValidator<CreateCompetitorCommand>
{
    public CreateCompetitorCommandValidator(ITrainerService trainerService)
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
        RuleFor(c => c.DateOfBirth)
            .NotEmpty()
            .Must(d => DateTime.TryParse(d.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture,
                DateTimeStyles.None, out _));
        RuleFor(c => c.Gender)
            .NotEmpty();
        RuleFor(c => c.Weight)
            .NotEmpty()
            .GreaterThan(0);
        RuleFor(c => c.Rank)
            .NotEmpty()
            .MaximumLength(50)
            .When(c => c.Rank is not null);
        RuleFor(c => c.Kyu)
            .NotEmpty()
            .MaximumLength(50)
            .When(c => c.Kyu is not null);
        RuleFor(c => c.Subject)
            .NotEmpty()
            .MaximumLength(100);
        When(c => c.TrainersIds is not null,
            () => RuleForEach(c => c.TrainersIds)
                .NotEmpty()
                .Must(g => Guid.TryParse(g.ToString(), out _))
                .MustAsync((g, ct) => TrainerExist(trainerService, g, ct))
                .WithMessage((_, trainerId) => $"Trainer with id {trainerId} not found")
                .AsNotFound());
    }

    protected async Task<bool> TrainerExist(ITrainerService trainerService, Guid trainerId, CancellationToken ct)
    {
        return (await trainerService.GetTrainer(trainerId, ct)).IsSuccess;
    }
}