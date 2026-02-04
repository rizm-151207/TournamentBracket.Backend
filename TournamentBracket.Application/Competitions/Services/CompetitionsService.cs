using Microsoft.EntityFrameworkCore;
using TournamentBracket.Application.Brackets.Interfaces;
using TournamentBracket.Application.Common.Authorization.Interfaces;
using TournamentBracket.Application.Common.Helpers;
using TournamentBracket.Application.Competitions.Commands;
using TournamentBracket.Application.Competitions.DTO;
using TournamentBracket.Application.Competitions.Interfaces;
using TournamentBracket.Application.Competitions.Queries;
using TournamentBracket.Application.Competitions.Responses;
using TournamentBracket.Application.Competitors.Interfaces;
using TournamentBracket.Application.Divisions.Interfaces;
using TournamentBracket.Application.Matches.Commands;
using TournamentBracket.Application.Matches.Interface;
using TournamentBracket.Domain.Competitions;
using TournamentBracket.Infrastructure.Common;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Competitions.Services;

public class CompetitionsService : ICompetitionsService
{
    private readonly AppDbContext dbContext;
    private readonly IResourceAuthorizationService authorizationService;
    private readonly ICompetitorService competitorService;
    private readonly IDivisionsService divisionsService;
    private readonly ITournamentBracketsService tournamentBracketsService;
    private readonly IMatchesService matchesService;


    public CompetitionsService(AppDbContext dbContext,
        IResourceAuthorizationService authorizationService,
        ICompetitorService competitorService,
        IDivisionsService divisionsService,
        ITournamentBracketsService tournamentBracketsService,
        IMatchesService matchesService)
    {
        this.dbContext = dbContext;
        this.authorizationService = authorizationService;
        this.competitorService = competitorService;
        this.divisionsService = divisionsService;
        this.tournamentBracketsService = tournamentBracketsService;
        this.matchesService = matchesService;
    }

    public async Task<Result<CreateCompetitionResponse>> CreateCompetition(
        CreateCompetitionCommand command,
        string userEmail,
        CancellationToken ct = default)
    {
        var competition = new Competition
        {
            Name = command.Name,
            Location = command.Location,
            StartDateTime = command.StartDateTime,
            Status = CompetitionStatus.Planned,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OwnerEmail = userEmail
        };

        dbContext.Competitions.Add(competition);
        await dbContext.SaveChangesAsync(ct);
        return Result<CreateCompetitionResponse>.Success(new CreateCompetitionResponse(competition.Id));
    }

    public async Task<Result<IReadOnlyCollection<Competition>>> GetCompetitions(CompetitionsPageQuery query,
        CancellationToken ct = default)
    {
        var competitionsQueryable = ApplyFilter(dbContext.Competitions.AsQueryable(), query.CreateFilter());
        var competitions = await competitionsQueryable
            .SelectPage(query)
            .Include(c => c.Divisions)
            .ToListAsync(ct);
        return Result<IReadOnlyCollection<Competition>>.Success(competitions);
    }

    public async Task<Result<int>> GetCount(CompetitionsFilter filter, CancellationToken ct = default)
    {
        var competitionsCount = await ApplyFilter(dbContext.Competitions.AsQueryable(), filter).CountAsync(ct);
        return Result<int>.Success(competitionsCount);
    }

    public async Task<Result<Competition>> GetCompetitionWithoutCompetitors(Guid id, CancellationToken ct = default)
    {
        var competition = await dbContext.Competitions
            .Include(c => c.Divisions)
            .IgnoreAutoIncludes()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken: ct);
        return competition == null
            ? Result<Competition>.FailedWith(new Error("Not found", 404))
            : Result<Competition>.Success(competition);
    }

    public async Task<Result<Competition>> GetCompetitionFull(Guid id, CancellationToken ct = default)
    {
        var competition = await dbContext.Competitions
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken: ct);
        return competition == null
            ? Result<Competition>.FailedWith(new Error("Not found", 404))
            : Result<Competition>.Success(competition);
    }

    public async Task<Result> UpdateCompetition(UpdateCompetitionCommand command, CancellationToken ct = default)
    {
        var competitionResult = await GetCompetitionWithoutCompetitors(command.Id, ct);
        if (!competitionResult.IsSuccess)
            return competitionResult;
        var competition = competitionResult.Item!;

        var authorizeResult = await authorizationService.Authorize(competition);
        if (!authorizeResult.IsSuccess)
            return authorizeResult;

        competition.Name = command.Name;
        competition.Location = command.Location;
        competition.StartDateTime = command.StartDateTime;
        competition.Status = command.Status;
        competition.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteCompetition(Guid id, CancellationToken ct = default)
    {
        var competitionResult = await GetCompetitionWithoutCompetitors(id, ct);
        if (!competitionResult.IsSuccess)
            return competitionResult;
        var competition = competitionResult.Item!;

        var authorizeResult = await authorizationService.Authorize(competition);
        if (!authorizeResult.IsSuccess)
            return authorizeResult;
        
        dbContext.Competitions.Remove(competition);
        var competitionsDeleted = await dbContext.SaveChangesAsync(ct);
        return competitionsDeleted > 0
            ? Result.Success()
            : Result.Failed("Some error while deleting");
    }

    public async Task<Result> AddCompetitorAuto(Guid competitionId, AddCompetitorCommand command,
        CancellationToken ct = default)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            var competitionResult = await GetCompetitionWithoutCompetitors(competitionId, ct);
            if (!competitionResult.IsSuccess)
                return competitionResult;
            var competition = competitionResult.Item!;
            
            var authorizeResult = await authorizationService.Authorize(competition);
            if (!authorizeResult.IsSuccess)
                return authorizeResult;

            var competitorResult = await competitorService.GetCompetitor(command.CompetitorId, ct);
            if (!competitorResult.IsSuccess)
                return competitorResult;
            var competitor = competitorResult.Item!;

            var suitableDivisionResult = await divisionsService.GetSuitableDivision(competitionId, competitor, ct);
            if (!suitableDivisionResult.IsSuccess)
                return suitableDivisionResult;

            var suitableDivision = suitableDivisionResult.Item;
            if (suitableDivision is null)
            {
                var defaultSuitableDivisionResult =
                    await divisionsService.ConstructSuitableDivision(competitionId, competitor, ct);
                if (!defaultSuitableDivisionResult.IsSuccess)
                    return defaultSuitableDivisionResult;
                suitableDivision = defaultSuitableDivisionResult.Item!;
            }

            var successAddCompetitor = suitableDivision.TryAddCompetitor(competitor);
            if (!successAddCompetitor)
                return Result.Failed($"Some error while adding competitor {competitor.Id}");
            var successAddDivision = competition.TryAddDivision(suitableDivision);
            if (!successAddDivision)
                return Result.Failed($"Some error while adding division to competition {competitionId}");

            if (suitableDivision.TournamentBracketId == Guid.Empty)
            {
                var createBracketResult =
                    await tournamentBracketsService.CreateBracket(suitableDivision.Competitors, ct);
                if (!createBracketResult.IsSuccess)
                    return createBracketResult;

                var newBracket = createBracketResult.Item!;
                suitableDivision.TournamentBracketId = newBracket.Id;
                suitableDivision.BracketType = newBracket.Type;
            }
            else
            {
                var addCompetitorResult = await tournamentBracketsService.AddCompetitorToBracket(
                    suitableDivision.TournamentBracketId,
                    suitableDivision.BracketType, competitor, ct);
                if (!addCompetitorResult.IsSuccess)
                    return addCompetitorResult;

                var newBracket = addCompetitorResult.Item!;
                suitableDivision.TournamentBracketId = newBracket.Id;
                suitableDivision.BracketType = newBracket.Type;
            }

            //dbContext.Divisions.Add(suitableDivision);
            await transaction.CommitAsync(ct);

            var planResult = await matchesService.PlanMatches(competition, ct);
            if (!planResult.IsSuccess)
                return planResult;

            await dbContext.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(ct);
            return e.ToResult();
        }
    }

    public async Task<Result> RemoveCompetitor(Guid competitionId, RemoveCompetitorCommand command,
        CancellationToken ct = default)
    {
        var competitionResult = await GetCompetitionWithoutCompetitors(competitionId, ct);
        if (!competitionResult.IsSuccess)
            return competitionResult;
        var competition = competitionResult.Item!;
        
        var authorizeResult = await authorizationService.Authorize(competition);
        if (!authorizeResult.IsSuccess)
            return authorizeResult;

        var competitorResult = await competitorService.GetCompetitor(command.CompetitorId, ct);
        if (!competitorResult.IsSuccess)
            return competitorResult;
        var competitor = competitorResult.Item!;

        var divisionsResult = await divisionsService.GetDivisionsByCompetitionId(competitionId, ct);
        if (!divisionsResult.IsSuccess)
            return divisionsResult;
        var divisions = divisionsResult.Item!;

        var division = divisions.FirstOrDefault(d => d.Competitors.Contains(competitor));
        if (division is null)
            return Result.Failed(
                $"Competition {competition.Id} does not contains division with competitor {competitor.Id}");

        var successRemoveFromDivision = division.TryRemoveCompetitor(competitor);
        if (!successRemoveFromDivision)
            return Result.Failed($"Some error while removing from division {competitor.Id}");

        if (division.Competitors.Count == 0)
        {
            dbContext.Divisions.Remove(division);
        }

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> AddMatchEvent(Guid competitionId, UpdateMatchCommand command, CancellationToken ct = default)
    {
        var competitionResult = await GetCompetitionWithoutCompetitors(competitionId, ct);
        if(!competitionResult.IsSuccess)
            return competitionResult;
        var competition = competitionResult.Item!;
        
        if(competition.Divisions?.All(d => d.TournamentBracketId != command.BracketId) ?? true)
            return Result.Failed($"Can't find bracket {command.BracketId} in competition {competition.Id}");
        
        var authorizeResult = await authorizationService.Authorize(competition);
        if (!authorizeResult.IsSuccess)
            return authorizeResult;

        return await matchesService.AddMatchEvent(command, ct);
    }

    private IQueryable<Competition> ApplyFilter(IQueryable<Competition> queryable, CompetitionsFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Name))
            queryable = queryable.Where(c => c.Name.Contains(filter.Name));
        return queryable;
    }
}