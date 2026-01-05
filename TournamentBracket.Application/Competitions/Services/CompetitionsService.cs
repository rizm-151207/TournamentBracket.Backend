using Microsoft.EntityFrameworkCore;
using TournamentBracket.Application.Common.Helpers;
using TournamentBracket.Application.Competitions.Commands;
using TournamentBracket.Application.Competitions.DTO;
using TournamentBracket.Application.Competitions.Interfaces;
using TournamentBracket.Application.Competitions.Queries;
using TournamentBracket.Application.Competitors.Interfaces;
using TournamentBracket.Application.Divisions.Interfaces;
using TournamentBracket.Domain.Competitions;
using TournamentBracket.Infrastructure.Common;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Competitions.Services;

public class CompetitionsService : ICompetitionsService
{
    private readonly AppDbContext dbContext;
    private readonly ICompetitorService competitorService;
    private readonly IDivisionsService divisionsService;

    public CompetitionsService(AppDbContext dbContext, 
        ICompetitorService competitorService,
        IDivisionsService divisionsService)
    {
        this.dbContext = dbContext;
        this.competitorService = competitorService;
        this.divisionsService = divisionsService;
    }

    public async Task<Result> CreateCompetition(CreateCompetitionCommand command, CancellationToken ct = default)
    {
        var competition = new Competition
        {
            Name = command.Name,
            Description = command.Description,
            StartDateTime = command.StartDateTime,
            Status = CompetitionStatus.Planned,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Competitions.Add(competition);
        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<Competition>>> GetCompetitions(CompetitionsPageQuery query,
        CancellationToken ct = default)
    {
        var competitionsQueryable = ApplyFilter(dbContext.Competitions.AsQueryable(), query.CreateFilter());
        var competitions = await competitionsQueryable
            .SelectPage(query)
            .Include(e => e.Competitors)
            .ToListAsync(ct);
        return Result<IReadOnlyCollection<Competition>>.Success(competitions);
    }

    public async Task<Result<IReadOnlyCollection<Competition>>> GetCompetitionsShorts(CompetitionsPageQuery query,
        CancellationToken ct = default)
    {
        var competitionsQueryable = ApplyFilter(dbContext.Competitions.AsQueryable(), query.CreateFilter());
        var competitions = await competitionsQueryable
            .SelectPage(query)
            .ToListAsync(ct);
        return Result<IReadOnlyCollection<Competition>>.Success(competitions);
    }

    public async Task<Result<int>> GetCount(CompetitionsFilter filter, CancellationToken ct = default)
    {
        var competitionsCount = await ApplyFilter(dbContext.Competitions.AsQueryable(), filter).CountAsync(ct);
        return Result<int>.Success(competitionsCount);
    }

    public async Task<Result<Competition>> GetCompetition(Guid id, CancellationToken ct = default)
    {
        var competition = await dbContext.Competitions
            .Include(e => e.Competitors)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken: ct);
        return competition == null
            ? Result<Competition>.FailedWith(new Error("Not found", 404))
            : Result<Competition>.Success(competition);
    }

    public async Task<Result> UpdateCompetition(UpdateCompetitionCommand command, CancellationToken ct = default)
    {
        var competitionResult = await GetCompetition(command.Id, ct);
        if (!competitionResult.IsSuccess)
            return competitionResult;

        var competition = competitionResult.Item!;
        competition.Name = command.Name;
        competition.Description = command.Description;
        competition.StartDateTime = command.StartDateTime;
        competition.Status = command.Status;
        competition.UpdatedAt = DateTime.UtcNow;
        competition.Competitors?.Clear();

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteCompetition(Guid id, CancellationToken ct = default)
    {
        var competitionResult = await GetCompetition(id, ct);
        if (!competitionResult.IsSuccess)
            return competitionResult;
        dbContext.Competitions.Remove(competitionResult.Item!);
        var competitionsDeleted = await dbContext.SaveChangesAsync(ct);
        return competitionsDeleted > 0
            ? Result.Success()
            : Result.Failed("Some error while deleting");
    }

    public async Task<Result> AddCompetitor(Guid competitionId, AddCompetitorCommand command, CancellationToken ct = default)
    {
        var competitionResult = await GetCompetition(competitionId, ct);
        if (!competitionResult.IsSuccess)
            return competitionResult;
        var competition = competitionResult.Item!;
        
        var competitorResult = await competitorService.GetCompetitor(command.CompetitorId, ct);
        if (!competitorResult.IsSuccess)
            return competitorResult;
        var competitor = competitorResult.Item!;

        if (competition.Competitors?.Contains(competitor) ?? false)
            return Result.Failed($"Competitor {competitor.Id} already added in competition {competition.Id}");

        var suitableDivisionResult = await divisionsService.GetSuitableDivision(competitionId, competitor, ct);
        if (!suitableDivisionResult.IsSuccess)
            return suitableDivisionResult;

        var suitableDivision = suitableDivisionResult.Item;
        if (suitableDivision is null)
        {
            var defaultSuitableDivisionResult = await divisionsService.ConstructSuitableDivision(competitionId, competitor, ct);
            if (!defaultSuitableDivisionResult.IsSuccess)
                return defaultSuitableDivisionResult;
            suitableDivision = defaultSuitableDivisionResult.Item!;
        }

        var successAddCompetitor = suitableDivision.TryAddCompetitor(competitor);
        if(!successAddCompetitor)
            return Result.Failed($"Some error while adding competitor {competitor.Id}");
        var successAddDivision = competition.TryAddDivision(suitableDivision);
        if (!successAddDivision)
            return Result.Failed($"Some error while adding division to competition {competitionId}");
        
        dbContext.Divisions.Add(suitableDivision);
        await dbContext.SaveChangesAsync(ct);
        
        return Result.Success();
    }

    public async Task<Result> RemoveCompetitor(Guid competitionId, RemoveCompetitorCommand command, CancellationToken ct = default)
    {
        var competitionResult = await GetCompetition(competitionId, ct);
        if (!competitionResult.IsSuccess)
            return competitionResult;
        var competition = competitionResult.Item!;
        
        var competitorResult = await competitorService.GetCompetitor(command.CompetitorId, ct);
        if (!competitorResult.IsSuccess)
            return competitorResult;
        var competitor = competitorResult.Item!;

        var divisionsResult = await divisionsService.GetDivisionByCompetitionId(competitionId, ct);
        if (!divisionsResult.IsSuccess)
            return divisionsResult;
        var divisions = divisionsResult.Item!;
        
        var division = divisions.FirstOrDefault(d => d.Competitors.Contains(competitor));
        if(division is null)
            return Result.Failed($"Competition {competition.Id} does not contains division with competitor {competitor.Id}");
        
        var successRemoveFromDivision= division.TryRemoveCompetitor(competitor);
        if (!successRemoveFromDivision)
            return Result.Failed($"Some error while removing from division {competitor.Id}");

        if (division.Competitors.Count == 0)
        {
            dbContext.Divisions.Remove(division);
        }
        
        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    private IQueryable<Competition> ApplyFilter(IQueryable<Competition> queryable, CompetitionsFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Name))
            queryable = queryable.Where(c => c.Name.Contains(filter.Name));
        return queryable;
    }
}