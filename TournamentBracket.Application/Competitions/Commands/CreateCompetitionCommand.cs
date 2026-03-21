namespace TournamentBracket.Application.Competitions.Commands;

public class CreateCompetitionCommand
{
    public CreateCompetitionCommand(
        string name,
        string location,
        int tatamiCount,
        DateTime startDateTime
    )
    {
        Name = name;
        Location = location;
        TatamiCount = tatamiCount;
        StartDateTime = startDateTime;
    }

    public string Name { get; set; }
    public string Location { get; set; }
    public int TatamiCount { get; set; }
    public DateTime StartDateTime { get; set; }
}
