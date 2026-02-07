using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Divisions;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Competitions;

public class MatchPlanner
{
    private readonly TimeSpan matchDuration = TimeSpan.FromMinutes(3);
    
    public bool PlanMatchesForCompetitions(Competition competition, IReadOnlyDictionary<Division, Bracket> divisionsWithBrackets)
    {
        var orderedMatches = divisionsWithBrackets
            .SelectMany(kvp => GetSortKeyForRound(kvp.Key, kvp.Value))
            .OrderBy(tuple => (-tuple.roundFromFinal, tuple.minAge))
            .SelectMany(tuple => tuple.orderedMatches);

        var counter = 1;
        var currentMatchDateTime = competition.StartDateTime;
        foreach (var match in orderedMatches)
        {
            if (match.IsByeMatch)
            {
                match.UnplanBye();
                continue;
            }
            match.Plan(counter, currentMatchDateTime);
            
            currentMatchDateTime += matchDuration;
            counter++;
        }

        return true;
    }

    private IEnumerable<(int? minAge, int roundFromFinal, IReadOnlyCollection<Match> orderedMatches)> GetSortKeyForRound(
        Division division,
        Bracket bracket)
    {
        return bracket.GetGroupedMatchesByRounds().Select(kvp => (division.MinAge, kvp.Key, kvp.Value));
    }
}