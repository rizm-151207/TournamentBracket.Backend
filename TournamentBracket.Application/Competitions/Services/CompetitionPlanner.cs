using TournamentBracket.Application.Brackets.Interfaces;
using TournamentBracket.Application.Competitions.Interfaces;
using TournamentBracket.Domain.Competitions;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Competitions.Services;

public class CompetitionPlanner : ICompetitionPlanner
{
    private readonly ITournamentBracketsService tournamentBracketsService;
    private readonly TatamiDistributor tatamiDistributor;
    private readonly MatchPlanner matchPlanner;

    public CompetitionPlanner(
        ITournamentBracketsService tournamentBracketsService,
        TatamiDistributor tatamiDistributor,
        MatchPlanner matchPlanner
    )
    {
        this.tournamentBracketsService = tournamentBracketsService;
        this.tatamiDistributor = tatamiDistributor;
        this.matchPlanner = matchPlanner;
    }

    public async Task<Result> PlanMatches(Competition competition, CancellationToken ct = default)
    {
        if (competition.Divisions is null || competition.Divisions.Count == 0)
            return Result.Success();

        var bracketsResult = await tournamentBracketsService.GetBracketsByIds(
            competition.Divisions.Select(d => d.TournamentBracketId).ToList(), ct);

        if (!bracketsResult.IsSuccess)
            return bracketsResult;

        var bracketIds = bracketsResult.Item!.ToDictionary(b => b.Id, b => b);
        var divisionsWithBrackets = competition.Divisions!.ToDictionary(d => d, d => bracketIds[d.TournamentBracketId]);

        var distributeSuccess = tatamiDistributor.DistributeTatamisByDivisions(divisionsWithBrackets, competition.TatamiCount);
        if (!distributeSuccess)
            Result.Failed("Something goes wrong while distributing divisions by tatami");

        foreach (var groupedDivisions in competition.Divisions.GroupBy(d => d.Tatami))
        {
            var tatamiNum = groupedDivisions.Key!.Value;
            var successPlanned = matchPlanner.PlanMatchesForDivisionsOnTatami(competition,
            [.. groupedDivisions], tatamiNum, divisionsWithBrackets);
            if (!successPlanned)
                return Result.Failed($"Something went wring while planning matches on tatami {tatamiNum}");
        }

        return Result.Success();
    }
}
