using TournamentBracket.Domain.Competitions;

namespace TournamentBracket.Application.Competitions.Commands;

public class UpdateCompetitionCommand : CreateCompetitionCommand
{
    public UpdateCompetitionCommand(
        Guid id,
        string name,
        string description,
        DateTime startDateTime,
        List<Guid>? competitorsIds,
        CompetitionStatus status)
        : base(name, description, startDateTime, competitorsIds)
    {
        Id = id;
        Status = status;
    }
    
    public Guid Id { get; set; }
    public CompetitionStatus Status { get; set; }
}