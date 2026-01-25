namespace TournamentBracket.Domain.Matches;

public class MatchProcess
{
    public Guid MatchId { get; set; }
    public int FirstCompetitorWazari { get; set; }
    public int FirstCompetitorPenalty { get; set; }
    public int SecondCompetitorWazari { get; set; }
    public int SecondCompetitorPenalty { get; set; }
    public MatchWinner? Winner { get; set; }
    public WinReason? WinReason { get; set; }
}

public enum MatchWinner
{
    FirstWinner,
    SecondWinner,
    Draw
}

public enum WinReason
{
    Bye,
    Ippon,
    Judges,
    
    Draw
}