using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Share.Abstractions;

namespace TournamentBracket.Domain.Competitions;

public class Competition : IEntity<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDateTime { get; set; }
    public CompetitionStatus Status { get; set; }
    public List<Competitor> Competitors { get; set; }
}