using TournamentBracket.Domain.Competitions;
using TournamentBracket.Domain.Divisions;
using TournamentBracket.Domain.Share.Abstractions;

namespace TournamentBracket.Domain.Competitors;

public class Competitor : IEntity<Guid>
{
    public Guid Id { get; init; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public bool Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public float Weight { get; set; }
    public string? Rank { get; set; }
    public string? Kyu { get; set; }
    public string Subject { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Trainer> Trainers { get; set; }
    public int Age => DateTime.Now.Year - DateOfBirth.Year;
    
    //links
    public List<Competition> Competitions { get; set; } = new();
    public List<Division> Divisions { get; set; } = new();
}