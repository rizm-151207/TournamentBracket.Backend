using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Brackets.RoundRobinBracket;
using TournamentBracket.Domain.Brackets.SingleEliminationBracket;

namespace TournamentBracket.Domain.Brackets;

public class BracketFactoryResolver
{
    private readonly BracketTypeResolver bracketTypeResolver;
    private readonly SingleEliminationBracketFactory singleEliminationBracketFactory;
	private readonly RoundRobinBracketFactory roundRobinBracketFactory;

    public BracketFactoryResolver(
        BracketTypeResolver bracketTypeResolver,
        SingleEliminationBracketFactory singleEliminationBracketFactory,
		RoundRobinBracketFactory roundRobinBracketFactory)
    {
        this.bracketTypeResolver = bracketTypeResolver;
        this.singleEliminationBracketFactory = singleEliminationBracketFactory;
		this.roundRobinBracketFactory = roundRobinBracketFactory;
    }

    public IBracketFactory ResolveByCompetitorsCount(int competitorsCount)
    {
        return ResolveByBracketType(bracketTypeResolver.Resolve(competitorsCount));
    }

    public IBracketFactory ResolveByBracketType(BracketType bracketType)
    {
        if (bracketType is BracketType.SingleElimination)
            return singleEliminationBracketFactory;
		
		if(bracketType is BracketType.RoundRobin)
			return roundRobinBracketFactory;

        throw new NotImplementedException($"No implementation for bracket type {bracketType}");
    }
}