using TournamentBracket.Domain.Share.Abstractions;

namespace TournamentBracket.Domain.Matches;

public class MatchProcess : IEntity<Guid>
{
    public Guid Id { get; }
    public int FirstCompetitorWazari { get; set; }
    public int FirstCompetitorPenalty { get; set; }
    public int SecondCompetitorWazari { get; set; }
    public int SecondCompetitorPenalty { get; set; }
    public MatchWinner? Winner { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum MatchWinner
{
    FirstWinner,
    SecondWinner,
    Draw
}