namespace TournamentBracket.Application.Competitions.Commands;

public class CreateCompetitionCommand
{
    public CreateCompetitionCommand(
        string name,
        string location,
        DateTime startDateTime
    )
    {
        Name = name;
        Location = location;
        StartDateTime = startDateTime;
    }

    public string Name { get; set; }
    public string Location { get; set; }
    public DateTime StartDateTime { get; set; }
}
