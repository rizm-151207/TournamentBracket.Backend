namespace TournamentBracket.Application.Competitions.Commands;

public class SetWinnerCommand
{
	public Guid CompetitorId { get; set; }
	public Guid BracketId { get; set; }
}
