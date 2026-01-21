using TournamentBracket.Domain.Common.Abstractions;
using TournamentBracket.Domain.Competitors;

namespace TournamentBracket.Domain.Brackets.Abstractions;

public abstract class Bracket : IEntity<Guid>
{
    public Guid Id { get; set; }
    public abstract BracketType Type { get; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public abstract bool TryAddCompetitorAuto(Competitor competitor);
    public abstract bool HasFreeMatch();
    public abstract List<Competitor> GetAllCompetitors();
}