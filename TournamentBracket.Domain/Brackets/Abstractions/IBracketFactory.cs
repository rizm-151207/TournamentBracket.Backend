using TournamentBracket.Domain.Competitors;

namespace TournamentBracket.Domain.Brackets.Abstractions;

public interface IBracketFactory
{
    public Bracket CreateBracket(IList<Competitor> competitors);
    public void ExtendBracket(Bracket bracket);
}