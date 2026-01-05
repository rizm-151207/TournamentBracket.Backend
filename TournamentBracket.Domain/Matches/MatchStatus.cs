namespace TournamentBracket.Domain.Matches;

public enum MatchStatus
{
    WaitingCompetitors,
    Planned,
    Started,
    Finished,
    Paused,
    Postponed
}