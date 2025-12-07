namespace TournamentBracket.Application.Competitors.Commands;

public class UpdateCompetitorCommand
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
    public List<Guid> TrainersIds { get; set; }
}