using TournamentBracket.Domain.Common.Abstractions;
using TournamentBracket.Domain.Divisions;

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
    
    //links
    public List<Division>? Divisions { get; set; }

    public bool TryAddDivision(Division division)
    {
        division.CompetitionId = Id;
        return true;
    }
}