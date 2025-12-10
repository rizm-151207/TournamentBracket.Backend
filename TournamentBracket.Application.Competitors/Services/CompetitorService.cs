using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using TournamentBracket.Application.Common.Helpers;
using TournamentBracket.Application.Competitors.Commands;
using TournamentBracket.Application.Competitors.DTO;
using TournamentBracket.Application.Competitors.Interfaces;
using TournamentBracket.Application.Competitors.Queries;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Infrastructure.Common;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Competitors.Services;

public class CompetitorService : ICompetitorService
{
    private readonly AppDbContext dbContext;

    public CompetitorService(AppDbContext context)
    {
        dbContext = context;
    }

    public async Task<Result> CreateCompetitor(CreateCompetitorCommand command, CancellationToken ct = default)
    {
        var competitor = new Competitor
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            MiddleName = command.MiddleName,
            DateOfBirth = command.DateOfBirth,
            Gender = command.Gender,
            Weight = command.Weight,
            Rank = command.Rank,
            Kyu = command.Kyu,
            Subject = command.Subject,
            UpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        var trainersIdsSet = command.TrainersIds.ToHashSet();

        var competitorTrainers = await dbContext.Trainers
            .Where(t => trainersIdsSet.Contains(t.Id))
            .ToListAsync(cancellationToken: ct);

        competitor.Trainers = competitorTrainers;
        dbContext.Competitors.Add(competitor);
        await dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<Competitor>>> GetCompetitors(CompetitorsPageQuery query,
        CancellationToken ct = default)
    {
        var queryable = dbContext.Competitors.AsQueryable().SelectPage(query);
        queryable = AddFilterToQueryable(queryable, query.Filter);

        var competitors = await queryable.ToListAsync(ct);
        return Result<IReadOnlyCollection<Competitor>>.Success(competitors);
    }

    public async Task<Result<int>> GetCount(CompetitorsFilter filter, CancellationToken ct = default)
    {
        var queryable = AddFilterToQueryable(dbContext.Competitors.AsQueryable(), filter);
        var count = await queryable.CountAsync(ct);
        return Result<int>.Success(count);
    }


    public async Task<Result<Competitor>> GetCompetitor(Guid id, CancellationToken ct = default)
    {
        var competitor = await dbContext.Competitors.FindAsync([id], cancellationToken: ct);
        return competitor == null
            ? Result<Competitor>.FailedWith(new Error("Not found", 404))
            : Result<Competitor>.Success(competitor);
    }

    public async Task<Result> UpdateCompetitor(UpdateCompetitorCommand command, CancellationToken ct = default)
    {
        var competitorResult = await GetCompetitor(command.Id, ct);
        if (!competitorResult.IsSuccess)
            return competitorResult;

        var trainersIdsSet = command.TrainersIds.ToHashSet();
        var competitorTrainers = await dbContext.Trainers
            .Where(t => trainersIdsSet.Contains(t.Id))
            .ToListAsync(cancellationToken: ct);

        var competitor = competitorResult.Item!;
        competitor.FirstName = command.FirstName;
        competitor.LastName = command.LastName;
        competitor.MiddleName = command.MiddleName;
        competitor.DateOfBirth = command.DateOfBirth;
        competitor.Gender = command.Gender;
        competitor.Weight = command.Weight;
        competitor.Rank = command.Rank;
        competitor.Kyu = command.Kyu;
        competitor.Subject = command.Subject;
        competitor.UpdatedAt = DateTime.UtcNow;
        competitor.Trainers = competitorTrainers;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteCompetitor(Guid id, CancellationToken ct = default)
    {
        var competitorResult = await GetCompetitor(id, ct);
        if (!competitorResult.IsSuccess)
            return competitorResult;
        var deletedEntity = dbContext.Competitors.Remove(competitorResult.Item!);
        return deletedEntity.State == EntityState.Deleted
            ? Result.Success()
            : Result.Failed("Some error while deleting");
    }

    private IQueryable<Competitor> AddFilterToQueryable(IQueryable<Competitor> queryable, CompetitorsFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.FIO))
        {
            var queryFioParts = filter.FIO.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var anyPartRegex = $"(?i)({string.Join('|', queryFioParts)})";
            queryable = queryable.Where(c => Regex.IsMatch(c.FirstName, anyPartRegex)
                                             || Regex.IsMatch(c.LastName, anyPartRegex)
                                             || (c.MiddleName != null && Regex.IsMatch(c.MiddleName, anyPartRegex)));
        }

        return queryable;
    }
}