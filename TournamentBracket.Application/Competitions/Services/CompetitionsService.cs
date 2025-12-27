using Microsoft.EntityFrameworkCore;
using TournamentBracket.Application.Common.Helpers;
using TournamentBracket.Application.Competitions.Commands;
using TournamentBracket.Application.Competitions.DTO;
using TournamentBracket.Application.Competitions.Interfaces;
using TournamentBracket.Application.Competitions.Queries;
using TournamentBracket.Application.Competitors.Interfaces;
using TournamentBracket.Domain.Competitions;
using TournamentBracket.Infrastructure.Common;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Competitions.Services;

public class CompetitionsService : ICompetitionsService
{
    private readonly AppDbContext dbContext;
    private readonly ICompetitorService competitorService;

    public CompetitionsService(AppDbContext dbContext, ICompetitorService competitorService)
    {
        this.dbContext = dbContext;
        this.competitorService = competitorService;
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

        if (command.CompetitorsIds != null)
        {
            var competitorsResult = await competitorService.GetCompetitorsById(command.CompetitorsIds, ct);
            if (!competitorsResult.IsSuccess)
                return competitorsResult;
            competition.Competitors = competitorsResult.Item!.ToList();
        }

        dbContext.Competitions.Add(competition);
        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<Competition>>> GetCompetitions(CompetitionsPageQuery query,
        CancellationToken ct = default)
    {
        var competitionsQueryable = ApplyFilter(dbContext.Competitions.AsQueryable(), query.CreateFilter());
        var competitions = await competitionsQueryable.SelectPage(query).ToListAsync(ct);
        return Result<IReadOnlyCollection<Competition>>.Success(competitions);
    }

    public async Task<Result<int>> GetCount(CompetitionsFilter filter, CancellationToken ct = default)
    {
        var competitionsCount = await ApplyFilter(dbContext.Competitions.AsQueryable(), filter).CountAsync(ct);
        return Result<int>.Success(competitionsCount);
    }

    public async Task<Result<Competition>> GetCompetition(Guid id, CancellationToken ct = default)
    {
        var competition = await dbContext.Competitions.FindAsync([id], cancellationToken: ct);
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
        competition.Competitors.Clear();
        if (command.CompetitorsIds != null)
        {
            var competitorsResult = await competitorService.GetCompetitorsById(command.CompetitorsIds, ct);
            if (!competitorsResult.IsSuccess)
                return competitorsResult;
            competition.Competitors = competitorsResult.Item!.ToList();
        }

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

    private IQueryable<Competition> ApplyFilter(IQueryable<Competition> queryable, CompetitionsFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Name))
            queryable = queryable.Where(c => c.Name.Contains(filter.Name));
        return queryable;
    }
}