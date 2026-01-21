using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Brackets;

public class BracketNodeFactory
{
    public BracketNode Create(
        Match match,
        int roundFromFinal,
        int indexInRounds,
        BracketNode? parent = null,
        List<BracketNode>? children = null)
    {
        var node = new BracketNode
        {
            RoundFromFinal = roundFromFinal,
            IndexInRound = indexInRounds,
            Parent = parent,
            Children = children,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        node.SetMatch(match);
        return node;
    }
}