using Microsoft.EntityFrameworkCore;
using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Brackets.SingleEliminationBracket;
using TournamentBracket.Infrastructure.Brackets.Interfaces;
using TournamentBracket.Infrastructure.Common;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Infrastructure.Brackets;

public class BracketsRepository : IBracketsRepository
{
    private AppDbContext dbContext;
    private SingleEliminationBracketFactory singleEliminationBracketFactory;

    public BracketsRepository(AppDbContext dbContext, SingleEliminationBracketFactory singleEliminationBracketFactory)
    {
        this.dbContext = dbContext;
        this.singleEliminationBracketFactory = singleEliminationBracketFactory;
    }

    public async Task<Result<Bracket>> GetBracket(Guid id, BracketType type, CancellationToken ct = default)
    {
        var bracket = await ResolveDbSet(type)
            .SingleOrDefaultAsync(b => b.Id == id, ct);

        if (bracket is null)
            Result<Bracket>.FailedWith(new Error($"Bracket with id {id} not found.", 404));

        if (bracket!.Type == BracketType.SingleElimination)
        {
            var constructedBracket = await LoadSingleEliminationBracket(bracket as SingleEliminationBracket);
            return Result<Bracket>.Success(constructedBracket);
        }
        throw new NotImplementedException($"Bracket with type {bracket.Type} not implemented.");
    }

    public async Task<Result<IReadOnlyCollection<Bracket>>> GetBrackets(IReadOnlyCollection<Guid> ids, CancellationToken ct = default)
    {
        var singleEliminationBrackets = await dbContext.SingleEliminationBrackets
            .Where(b => ids.Contains(b.Id))
            .GroupJoin(dbContext.BracketNodes,
                b => b.Id,
                bn => bn.BracketId,
                (b, bn) => new
                {
                    BracketBase = b,
                    BracketNodes = bn.ToList()
                })
            .ToDictionaryAsync(g =>  g.BracketBase, g => g.BracketNodes, ct);

        var enrichedBrackets = singleEliminationBrackets.Select(kvp =>
            singleEliminationBracketFactory.EnrichBracketWithNodes(kvp.Key, kvp.Value));

        return Result<IReadOnlyCollection<Bracket>>.Success(enrichedBrackets.ToList());
    }

    private async Task<SingleEliminationBracket> LoadSingleEliminationBracket(SingleEliminationBracket? bracketBase)
    {
        if (bracketBase is null)
            throw new ArgumentNullException(nameof(bracketBase));

        var nodes = await dbContext.BracketNodes
            .Where(b => b.BracketId == bracketBase.Id)
            .ToListAsync();

        bracketBase = singleEliminationBracketFactory.EnrichBracketWithNodes(bracketBase, nodes);
        return bracketBase;
    }

    private IQueryable<Bracket> ResolveDbSet(BracketType type)
    {
        return type switch
        {
            BracketType.SingleElimination => dbContext.SingleEliminationBrackets,
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unknown bracket type {type}.")
        };
    }
}