using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Application.Matches.Commands;

public class UpdateMatchCommand
{
    public Guid MatchId { get; set; }
    public Guid CompetitorId { get; set; }
    public Guid BracketId { get; set; }
    public MatchUpdateType Type { get; set; }
}