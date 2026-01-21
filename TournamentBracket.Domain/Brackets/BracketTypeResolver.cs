namespace TournamentBracket.Domain.Brackets;

public class BracketTypeResolver
{
    public BracketType Resolve(int competitorsCount)
    {
        return BracketType.SingleElimination;
    }
}