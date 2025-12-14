using Microsoft.EntityFrameworkCore;
using TournamentBracket.Application.Common.Helpers;
using TournamentBracket.Application.Common.Queries;
using TournamentBracket.Application.Competitors.Commands;
using TournamentBracket.Application.Competitors.Interfaces;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Infrastructure.Common;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Competitors.Services;

public class TrainerService : ITrainerService
{
    private readonly AppDbContext dbContext;

    public TrainerService(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Result> CreateTrainer(CreateTrainerCommand command, CancellationToken ct = default)
    {
        var trainer = new Trainer
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            MiddleName = command.MiddleName,
            Club = command.Club,
            Subject = command.Subject,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Trainers.Add(trainer);
        await dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<Trainer>>> GetTrainers(PageQuery query,
        CancellationToken ct = default)
    {
        var trainers = await dbContext.Trainers
            .AsQueryable()
            .SelectPage(query)
            .ToListAsync(ct);

        return Result<IReadOnlyCollection<Trainer>>.Success(trainers);
    }

    public async Task<Result<int>> GetCount(CancellationToken ct = default)
    {
        return Result<int>.Success(await dbContext.Trainers.CountAsync(ct));
    }

    public async Task<Result<Trainer>> GetTrainer(Guid id, CancellationToken ct = default)
    {
        var trainer = await dbContext.Trainers.FindAsync([id], cancellationToken: ct);
        return trainer is null
            ? Result<Trainer>.FailedWith(new Error("Not Found", 404))
            : Result<Trainer>.Success(trainer);
    }

    public async Task<Result> UpdateTrainer(UpdateTrainerCommand command, CancellationToken ct = default)
    {
        var trainerResult = await GetTrainer(command.Id, ct);
        if (!trainerResult.IsSuccess)
            return Result.Failed(trainerResult.Error!);

        var trainer = trainerResult.Item!;
        trainer.FirstName = command.FirstName;
        trainer.LastName = command.LastName;
        trainer.MiddleName = command.MiddleName;
        trainer.Club = command.Club;
        trainer.Subject = command.Subject;
        trainer.UpdatedAt = DateTime.UtcNow;

        dbContext.Trainers.Update(trainer);
        await dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result> DeleteTrainer(Guid id, CancellationToken ct = default)
    {
        var trainerResult = await GetTrainer(id, ct);
        if (!trainerResult.IsSuccess)
            return Result.Failed(trainerResult.Error!);

        dbContext.Trainers.Remove(trainerResult.Item!);
        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}