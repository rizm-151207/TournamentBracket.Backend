using TournamentBracket.Domain.Competitors;

namespace TournamentBracket.Domain.Brackets.Abstractions;

public interface IBracketFactory
{
	public Bracket CreateBracket(IList<Competitor> competitors);
	public void ExtendBracket(Bracket bracket);
	public bool NeedToReduce(Bracket bracket);
	public void ReduceBracket(Bracket bracket);
	public void RebalanceBracket(Bracket bracket);
}