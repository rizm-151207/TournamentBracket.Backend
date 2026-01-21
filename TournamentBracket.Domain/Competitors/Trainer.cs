using TournamentBracket.Domain.Common.Abstractions;

namespace TournamentBracket.Domain.Competitors;

public class Trainer : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Club { get; set; }
    public string? Subject { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Competitor>? Competitors { get; set; }
}