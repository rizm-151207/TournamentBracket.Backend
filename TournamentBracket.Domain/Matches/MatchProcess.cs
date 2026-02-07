namespace TournamentBracket.Domain.Matches;

public class MatchProcess
{
    public Guid MatchId { get; set; }

    public int FirstCompetitorWazari { get; set; }
    public int FirstCompetitorKeikoku { get; set; } //Verbal warning
    public int FirstCompetitorChui { get; set; } //Warning

    public int SecondCompetitorWazari { get; set; }
    public int SecondCompetitorKeikoku { get; set; }
    public int SecondCompetitorChui { get; set; }

    public MatchWinner? Winner { get; set; }
    public WinReason? WinReason { get; set; }

    public void AddWazari(bool isFirstCompetitor)
    {
        if (isFirstCompetitor)
            FirstCompetitorWazari++;
        else
            SecondCompetitorWazari++;
    }

    public void AddKeikoku(bool isFirstCompetitor)
    {
        if (isFirstCompetitor)
        {
            FirstCompetitorKeikoku++;
            if (FirstCompetitorKeikoku == 2)
            {
                FirstCompetitorKeikoku = 0;
                AddChui(true);
            }
        }
        else
        {
            SecondCompetitorKeikoku++;
            if (SecondCompetitorKeikoku == 2)
            {
                SecondCompetitorKeikoku = 0;
                AddChui(false);
            }
        }
    }

    public void AddChui(bool isFirstCompetitor)
    {
        if (isFirstCompetitor)
        {
            FirstCompetitorChui++;
        }
        else
        {
            SecondCompetitorChui++;
        }
    }

    public void SetWinner(bool isFirstCompetitor, WinReason winReason)
    {
        Winner = isFirstCompetitor ? MatchWinner.FirstCompetitor : MatchWinner.SecondCompetitor;
        WinReason = winReason;
    }

    public void Clear()
    {
        FirstCompetitorWazari = 0;
        FirstCompetitorKeikoku = 0;
        FirstCompetitorChui = 0;
        SecondCompetitorWazari = 0;
        SecondCompetitorKeikoku = 0;
        SecondCompetitorChui = 0;
        Winner = null;
        WinReason = null;
    }
}

public enum MatchWinner
{
    FirstCompetitor,
    SecondCompetitor,
    Draw
}

public enum WinReason
{
    Bye,
    Ippon,
    Wazari,
    ByJudges, //hantei
    Sikkaku, //Sikkaku
    Denial, //Kiken
    Draw
}