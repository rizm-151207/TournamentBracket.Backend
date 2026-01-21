using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Brackets.SingleEliminationBracket;

namespace TournamentBracket.Domain.Brackets;

public class BracketFactoryResolver
{
    private readonly BracketTypeResolver bracketTypeResolver;
    private readonly SingleEliminationBracketFactory singleEliminationBracketFactory;

    public BracketFactoryResolver(
        BracketTypeResolver bracketTypeResolver,
        SingleEliminationBracketFactory singleEliminationBracketFactory)
    {
        this.bracketTypeResolver = bracketTypeResolver;
        this.singleEliminationBracketFactory = singleEliminationBracketFactory;
    }

    public IBracketFactory ResolveByCompetitorsCount(int competitorsCount)
    {
        return ResolveByBracketType(bracketTypeResolver.Resolve(competitorsCount));
    }
    
    public IBracketFactory ResolveByBracketType(BracketType bracketType)
    {
        if (bracketType is BracketType.SingleElimination)
            return singleEliminationBracketFactory;
        throw new NotImplementedException($"No implementation for bracket type {bracketType}");
    }
}