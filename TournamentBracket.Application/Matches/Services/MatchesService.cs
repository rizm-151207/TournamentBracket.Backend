using TournamentBracket.Application.Brackets.Interfaces;
using TournamentBracket.Application.Matches.Interface;
using TournamentBracket.Domain.Competitions;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Matches.Services;

public class MatchesService : IMatchesService
{
    private readonly MatchPlanner matchPlanner;
    private readonly ITournamentBracketsService tournamentBracketsService;

    public MatchesService(MatchPlanner matchPlanner, ITournamentBracketsService tournamentBracketsService)
    {
        this.matchPlanner = matchPlanner;
        this.tournamentBracketsService = tournamentBracketsService;
    }

    public async Task<Result> PlanMatches(Competition competition, CancellationToken ct = default)
    {
        if(competition.Divisions is null || competition.Divisions.Count == 0)
            return Result.Success();
        
        var bracketsResult = await tournamentBracketsService.GetBracketsBracketsIds(
            competition.Divisions.Select(d => d.TournamentBracketId).ToList(), ct);

        if (!bracketsResult.IsSuccess)
            return bracketsResult;

        var bracketIds = bracketsResult.Item!.ToDictionary(b => b.Id, b => b);
        var divisionsWithBRackets = competition.Divisions!.ToDictionary(d => d, d => bracketIds[d.TournamentBracketId]);

        return matchPlanner.PlanMatchesForCompetitions(competition, divisionsWithBRackets)
            ? Result.Success()
            : Result.Failed("Something went wring while planning matches");
    }
}