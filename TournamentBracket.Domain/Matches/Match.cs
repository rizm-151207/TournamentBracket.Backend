using TournamentBracket.Domain.Common.Abstractions;
using TournamentBracket.Domain.Competitors;

namespace TournamentBracket.Domain.Matches;

public class Match : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string? Index { get; set; }
    public Competitor? FirstCompetitor { get; set; }
    public Competitor? SecondCompetitor { get; set; }
    public MatchStatus Status { get; set; }
    public required MatchProcess MatchProcess { get; set; }
    public DateTime? PlannedDateTime { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool IsByeMatch => Status is MatchStatus.Finished && MatchProcess.WinReason == WinReason.Bye;

    public void Clear()
    {
        FirstCompetitor = null;
        SecondCompetitor = null;
        PlannedDateTime = null;
        StartDateTime = null;
        EndDateTime = null;
        MatchProcess.WinReason = null;
        MatchProcess.Winner = null;
    }
}
