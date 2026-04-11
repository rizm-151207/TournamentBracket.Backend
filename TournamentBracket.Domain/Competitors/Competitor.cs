using TournamentBracket.Domain.Common.Abstractions;
using TournamentBracket.Domain.Divisions;

namespace TournamentBracket.Domain.Competitors;

public class Competitor : IEntity<Guid>, ISoftDelete
{
	public Guid Id { get; set; }
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
	public List<Trainer>? Trainers { get; set; }
	public bool IsDeleted { get; set; }
	public DateTime? DeletedAt { get; set; }
	public int Age => DateTime.Now.Year - DateOfBirth.Year;

	//links
	public List<Division> Divisions { get; set; } = new();
}