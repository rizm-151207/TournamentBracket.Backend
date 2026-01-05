namespace TournamentBracket.Application.Competitions.Commands;

public class CreateCompetitionCommand
{
    public CreateCompetitionCommand(
        string name,
        string description,
        DateTime startDateTime
    )
    {
        Name = name;
        Description = description;
        StartDateTime = startDateTime;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDateTime { get; set; }
}
