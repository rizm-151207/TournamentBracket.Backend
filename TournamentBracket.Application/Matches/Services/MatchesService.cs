using Microsoft.EntityFrameworkCore;
using TournamentBracket.Application.Brackets.Interfaces;
using TournamentBracket.Application.Common.Helpers;
using TournamentBracket.Application.Matches.Commands;
using TournamentBracket.Application.Matches.Interface;
using TournamentBracket.Domain.Matches;
using TournamentBracket.Infrastructure.Common;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Matches.Services;

public class MatchesService : IMatchesService
{
    private readonly AppDbContext dbContext;
    private readonly ITournamentBracketsService tournamentBracketsService;

    public MatchesService(
        AppDbContext dbContext,
        ITournamentBracketsService tournamentBracketsService)
    {
        this.dbContext = dbContext;
        this.tournamentBracketsService = tournamentBracketsService;
    }

    public async Task<Result> AddMatchEvent(UpdateMatchCommand command, CancellationToken ct = default)
    {
        try
        {
            var match = await dbContext.Matches.Where(m => m.Id == command.MatchId).SingleAsync(ct);
            var prevMatchStatus = match.Status;
            match.UpdateMatch(CreateEventFromCommand(command, match));

            if (prevMatchStatus != match.Status && match.Status == MatchStatus.Finished)
            {
                var updateResult = await tournamentBracketsService.UpdateBracketFromMatch(command.BracketId, match, ct);
                if (!updateResult.IsSuccess)
                    return updateResult;
            }

            await dbContext.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception e)
        {
            return e.ToResult();
        }
    }

    public async Task<Result<IReadOnlyCollection<Match>>> GetUnfinishedMatchesForCompetitor(Guid competitorId,
        CancellationToken ct = default)
    {
        var matches = await dbContext.Matches.Where(m =>
                ((m.FirstCompetitor != null && m.FirstCompetitor.Id == competitorId)
                || (m.SecondCompetitor != null && m.SecondCompetitor.Id == competitorId))
                && m.Status != MatchStatus.Finished)
            .ToListAsync(ct);
        return Result<IReadOnlyCollection<Match>>.Success(matches);
    }

    private MatchUpdateEvent CreateEventFromCommand(UpdateMatchCommand command, Match match)
    {
        bool? isFirstCompetitor = null;
        if (match.FirstCompetitor?.Id == command.CompetitorId)
            isFirstCompetitor = true;
        if (match.SecondCompetitor?.Id == command.CompetitorId)
            isFirstCompetitor = false;

        if (!isFirstCompetitor.HasValue)
            throw new ArgumentException($"The competitor {command.CompetitorId} not in match {match.Id}");

        return new MatchUpdateEvent
        {
            Type = command.Type,
            IsFirstCompetitor = isFirstCompetitor.Value,
        };
    }
}