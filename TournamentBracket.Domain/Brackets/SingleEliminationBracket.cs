using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Share.Abstractions;

namespace TournamentBracket.Domain.Brackets;

public class SingleEliminationBracket : IEntity<Guid>, ITournamentBracket
{
    public Guid Id { get; }
    public BracketNode Root { get; set; }
    public BracketNode ThirdPlace { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}