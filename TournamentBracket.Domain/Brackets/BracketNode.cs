using TournamentBracket.Domain.Common.Abstractions;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Brackets;

public class BracketNode : IEntity<Guid>
{
    public Guid Id { get; set; }
    public BracketNode? Parent { get; set; }
    public List<BracketNode>? Children { get; set; }
    public int RoundFromFinal { get; set; }
    public int IndexInRound { get; set; }
    public Match Match { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    //links
    public Guid? MatchId { get; set; }
    public Guid? ParentNodeId { get; set; }
    public Guid BracketId { get; set; }

    public void SetParent(BracketNode parent)
    {
        if (Parent is not null)
            throw new InvalidOperationException($"Parent for node {Id} already setted");

        Parent = parent;
    }

    public void SetMatch(Match match)
    {
        Match = match;
        MatchId = match.Id;
    }

    public IEnumerable<BracketNode> GetAllChildren()
    {
        if(Children is null)
            yield break;
        foreach (var child in Children)
        {
            yield return child;
            foreach (var granChild in child.GetAllChildren())
                yield return granChild;
        }
    }
}