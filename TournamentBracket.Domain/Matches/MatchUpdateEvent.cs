namespace TournamentBracket.Domain.Matches;

public class MatchUpdateEvent
{
	public MatchUpdateType Type { get; set; }
	public bool IsFirstCompetitor { get; set; }
}

public enum MatchUpdateType
{
	Keikoku,
	Chui,
	Genten,
	Disqualification,
	Ippon,
	Wazari
}