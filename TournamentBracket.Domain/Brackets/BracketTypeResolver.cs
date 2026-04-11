namespace TournamentBracket.Domain.Brackets;

public class BracketTypeResolver
{
    public BracketType Resolve(int competitorsCount)
    {
		return competitorsCount switch
		{
			<= 0 => throw new ArgumentException($"CompetitorsCount must be greater than 0"),
			<= 3 => BracketType.RoundRobin,
			_ => BracketType.SingleElimination 
		};
    }
}