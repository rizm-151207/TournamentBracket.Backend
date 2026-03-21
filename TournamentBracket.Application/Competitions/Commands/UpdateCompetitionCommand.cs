using TournamentBracket.Domain.Competitions;

namespace TournamentBracket.Application.Competitions.Commands;

public class UpdateCompetitionCommand : CreateCompetitionCommand
{
    public UpdateCompetitionCommand(
        Guid id,
        string name,
        string location,
        int tatamiCount,
        DateTime startDateTime,
        CompetitionStatus status)
        : base(name, location, tatamiCount, startDateTime)
    {
        Id = id;
        Status = status;
    }
    
    public Guid Id { get; set; }
    public CompetitionStatus Status { get; set; }
}