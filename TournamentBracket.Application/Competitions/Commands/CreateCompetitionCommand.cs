namespace TournamentBracket.Application.Competitions.Commands;

public class CreateCompetitionCommand
{
    public CreateCompetitionCommand(
        string name,
        string description,
        DateTime startDateTime,
        List<Guid>? competitorsIds
    )
    {
        Name = name;
        Description = description;
        StartDateTime = startDateTime;
        CompetitorsIds = competitorsIds;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDateTime { get; set; }
    public List<Guid>? CompetitorsIds { get; set; }
}
