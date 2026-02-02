using Microsoft.EntityFrameworkCore;
using TournamentBracket.Application.Common.Helpers;
using TournamentBracket.Application.Competitors.Commands;
using TournamentBracket.Application.Competitors.DTO;
using TournamentBracket.Application.Competitors.Interfaces;
using TournamentBracket.Application.Competitors.Queries;
using TournamentBracket.Application.Competitors.Responses;
using TournamentBracket.Application.Divisions.Interfaces;
using TournamentBracket.Application.Matches.Interface;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Infrastructure.Common;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Competitors.Services;

public class CompetitorService : ICompetitorService
{
    private readonly AppDbContext dbContext;
    private readonly IMatchesService matchesService;
    private readonly IDivisionsService divisionsService;

    public CompetitorService(AppDbContext context, IMatchesService matchesService, IDivisionsService divisionsService)
    {
        dbContext = context;
        this.matchesService = matchesService;
        this.divisionsService = divisionsService;
    }

    public async Task<Result<CreateCompetitorResponse>> CreateCompetitor(CreateCompetitorCommand command,
        CancellationToken ct = default)
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

        if (command.TrainersIds != null)
        {
            var trainersIdsSet = command.TrainersIds.ToHashSet();
            var competitorTrainers = await dbContext.Trainers
                .Where(t => trainersIdsSet.Contains(t.Id))
                .ToListAsync(cancellationToken: ct);
            competitor.Trainers = competitorTrainers;
        }

        dbContext.Competitors.Add(competitor);
        await dbContext.SaveChangesAsync(ct);

        return Result<CreateCompetitorResponse>.Success(new CreateCompetitorResponse(competitor.Id));
    }

    public async Task<Result<IReadOnlyCollection<Competitor>>> GetCompetitors(CompetitorsPageQuery query,
        CancellationToken ct = default)
    {
        var queryable = dbContext.Competitors.AsQueryable();
        queryable = AddFilterToQueryable(queryable, query.CreateFilter())
            .SelectPage(query);

        var competitors = await queryable.ToListAsync(ct);
        return Result<IReadOnlyCollection<Competitor>>.Success(competitors);
    }

    public async Task<Result<IReadOnlyCollection<Competitor>>> GetCompetitorsById(
        IReadOnlyCollection<Guid> competitorsIds,
        CancellationToken ct = default)
    {
        var distinctCompetitorsIds = competitorsIds.ToHashSet();
        var competitors = await dbContext.Competitors
            .Where(c => distinctCompetitorsIds.Contains(c.Id))
            .ToListAsync(ct);

        return competitors.Count == distinctCompetitorsIds.Count
            ? Result<IReadOnlyCollection<Competitor>>.Success(competitors)
            : Result<IReadOnlyCollection<Competitor>>.FailedWith(new Error(GetNotFoundCompetitorsIdsMessage(), 404));

        string GetNotFoundCompetitorsIdsMessage()
        {
            var notFoundCompetitors = competitors
                .Select(c => c.Id)
                .Except(distinctCompetitorsIds);
            return $"Competitors not found: {string.Join(", ", notFoundCompetitors)}";
        }
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
        competitor.Trainers.Clear();
        if (command.TrainersIds != null)
        {
            var trainersIdsSet = command.TrainersIds.ToHashSet();
            var competitorTrainers = await dbContext.Trainers
                .Where(t => trainersIdsSet.Contains(t.Id))
                .ToListAsync(cancellationToken: ct);
            competitor.Trainers = competitorTrainers;
        }

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteCompetitor(Guid id, CancellationToken ct = default)
    {
        var competitorResult = await GetCompetitor(id, ct);
        if (!competitorResult.IsSuccess)
            return competitorResult;

        var competitor = competitorResult.Item!;
        competitor.IsDeleted = true;
        competitor.DeletedAt = DateTime.UtcNow;

        var unfinishedMatches = await matchesService.GetUnfinishedMatchesForCompetitor(id, ct);
        if(!unfinishedMatches.IsSuccess)
            return unfinishedMatches;
        if (unfinishedMatches.Item!.Any())
            return Result.Failed(new Error($"Can't delete competitor. He participate in {unfinishedMatches.Item!.Count} matches"));
        
        var divisionsWithCompetitorResult = await divisionsService.GetDivisionsWithCompetitor(id, ct);
        if(!divisionsWithCompetitorResult.IsSuccess)
            return divisionsWithCompetitorResult;
        var divisions = divisionsWithCompetitorResult.Item!;
        foreach (var division in divisions)
            division.Competitors.Remove(competitor);

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    private IQueryable<Competitor> AddFilterToQueryable(IQueryable<Competitor> queryable, CompetitorsFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.FIO))
        {
            var queryFioParts = filter.FIO
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.ToLower())
                .ToList();
            if (queryFioParts.Any())
            {
                var anyPartRegex = $"%{string.Join("%|%", queryFioParts)}%";
                queryable = queryable
                    .Where(c => !c.IsDeleted)
                    .Where(c => EF.Functions.Like(c.FirstName.ToLower(), anyPartRegex)
                                                 || EF.Functions.Like(c.LastName.ToLower(), anyPartRegex)
                                                 || (c.MiddleName != null &&
                                                     EF.Functions.Like(c.MiddleName.ToLower(), anyPartRegex)));
            }
        }

        return queryable;
    }
}