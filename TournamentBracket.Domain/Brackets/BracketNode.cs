using TournamentBracket.Domain.Matches;
using TournamentBracket.Domain.Share.Abstractions;

namespace TournamentBracket.Domain.Brackets;

public class BracketNode : IEntity<Guid>
{
    public Guid Id { get; }
    public BracketNode? Parent { get; set; }
    public List<BracketNode>? Children { get; set; }
    public int RoundFromFinal { get; set; }
    public int IndexInRound { get; set; }
    public Match? Match { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}