using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Share.Abstractions;

namespace TournamentBracket.Domain.Matches;

public class Match : IEntity<Guid>
{
    public Guid Id { get; }
    public string Index { get; set; }
    public Competitor FirstCompetitor { get; set; }
    public Competitor? SecondCompetitor { get; set; }
    public MatchStatus Status { get; set; }
    public MatchProcess?  MatchProcess { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}