using TournamentBracket.Domain.Competitors;

namespace TournamentBracket.Domain.Matches;

public class MatchFactory
{
    public Match CreateLeafMatch(
        Competitor? firstCompetitor,
        Competitor? secondCompetitor)
    {
        if (firstCompetitor == null && secondCompetitor == null)
            return CreateEmptyUnplannedMatch();

        if ((firstCompetitor is null || secondCompetitor is null)
            && firstCompetitor != secondCompetitor)
            return CreateByeMatch(firstCompetitor, secondCompetitor);

        return new Match
        {
            FirstCompetitor = firstCompetitor!,
            SecondCompetitor = secondCompetitor!,
            Status = MatchStatus.Unplanned,
            MatchProcess = new MatchProcess(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }

    public Match CreateEmptyUnplannedMatch()
    {
        return new Match
        {
            Status = MatchStatus.Unplanned,
            MatchProcess = new MatchProcess(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }

    private Match CreateByeMatch(Competitor? firstCompetitor, Competitor? secondCompetitor)
    {
        var winner = firstCompetitor ?? secondCompetitor;
        return new Match
        {
            FirstCompetitor = winner ?? throw new NullReferenceException(message: "Bye match can't has null winner"),
            Status = MatchStatus.Finished,
            MatchProcess = new MatchProcess { Winner = MatchWinner.FirstWinner, WinReason = WinReason.Bye },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }
}