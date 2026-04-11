namespace TournamentBracket.Domain.Matches;

public enum MatchStatus
{
	WaitingOtherCompetitor,
	Unplanned,
	Planned,
	Started,
	Finished,
	Paused,
	Postponed,
}