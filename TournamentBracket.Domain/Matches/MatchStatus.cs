namespace TournamentBracket.Domain.Matches;

public enum MatchStatus
{
    WaitingCompetitors,
    Unplanned,
    Planned,
    Started,
    Finished,
    Paused,
    Postponed,
}