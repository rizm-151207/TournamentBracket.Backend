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
			var constructedBracket = await LoadSingleEliminationBracket(bracket as SingleEliminationBracket, ct);
			return Result<Bracket>.Success(constructedBracket);
		}

		if (bracket!.Type == BracketType.RoundRobin)
		{
			return Result<Bracket>.Success(bracket);
		}

		throw new NotImplementedException($"Bracket with type {bracket.Type} not implemented.");
	}

	public async Task<Result<IReadOnlyCollection<Bracket>>> GetBrackets(IReadOnlyCollection<Guid> ids,
		CancellationToken ct = default)
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
			.ToDictionaryAsync(g => g.BracketBase, g => g.BracketNodes, ct);

		IEnumerable<Bracket> enrichedBrackets = singleEliminationBrackets.Select(kvp =>
			singleEliminationBracketFactory.EnrichBracketWithNodes(kvp.Key, kvp.Value));

		var rrBrackets = await dbContext.RoundRobinBrackets
			.Where(b => ids.Contains(b.Id))
			.ToListAsync();

		enrichedBrackets = enrichedBrackets.Union(rrBrackets);

		return Result<IReadOnlyCollection<Bracket>>.Success(enrichedBrackets.ToList());
	}

	public async Task<Result<Bracket>> GetBracketById(Guid id, CancellationToken ct = default)
	{
		var seBase =
			await dbContext.SingleEliminationBrackets.SingleOrDefaultAsync(b => b.Id == id, ct);
		if (seBase is not null)
		{
			var seBracket = await LoadSingleEliminationBracket(seBase, ct);
			return Result<Bracket>.Success(seBracket);
		}

		var rrBracket = await dbContext.RoundRobinBrackets
			.Include(b => b.Matches)
			.SingleOrDefaultAsync(b => b.Id == id, ct);
		if (rrBracket is not null)
		{
			return Result<Bracket>.Success(rrBracket);
		}

		return Result<Bracket>.FailedWith(new Error($"Bracket with id {id} not found", 404));
	}

	public Task<Result> RemoveBracket(Bracket bracket, CancellationToken ct = default)
	{
		dbContext.Matches.RemoveRange(bracket.GetAllMatches());

		if (bracket.Type is BracketType.SingleElimination)
			dbContext.BracketNodes.RemoveRange((bracket as SingleEliminationBracket)!.GetAllNodes());
		dbContext.Remove(bracket);

		return Task.FromResult(Result.Success());
	}

	public async Task<Result> TrimBracket(Bracket bracket, CancellationToken ct = default)
	{
		if (bracket.Type is BracketType.SingleElimination)
			return await DeleteUnusedNodes((bracket as SingleEliminationBracket)!, ct);
		return Result.Success();
	}

	public async Task<Result> DeleteUnusedNodes(SingleEliminationBracket singleEliminationBracket,
		CancellationToken ct = default)
	{
		var bracketInDbResult = await GetBracket(singleEliminationBracket.Id, singleEliminationBracket.Type, ct);
		if (!bracketInDbResult.IsSuccess)
			return bracketInDbResult;
		var bracketInDb = (bracketInDbResult.Item as SingleEliminationBracket)!;

		var currentNodes = singleEliminationBracket.GetAllNodes();
		var nodesInDb = bracketInDb.GetAllNodes();

		dbContext.BracketNodes.RemoveRange(nodesInDb.Except(currentNodes));
		return Result.Success();
	}

	private async Task<SingleEliminationBracket> LoadSingleEliminationBracket(SingleEliminationBracket? bracketBase,
		CancellationToken ct = default)
	{
		if (bracketBase is null)
			throw new ArgumentNullException(nameof(bracketBase));

		var nodes = await dbContext.BracketNodes
			.Where(b => b.BracketId == bracketBase.Id)
			.ToListAsync(ct);

		bracketBase = singleEliminationBracketFactory.EnrichBracketWithNodes(bracketBase, nodes);
		return bracketBase;
	}

	private IQueryable<Bracket> ResolveDbSet(BracketType type)
	{
		return type switch
		{
			BracketType.SingleElimination => dbContext.SingleEliminationBrackets,
			BracketType.RoundRobin => dbContext.RoundRobinBrackets,
			_ => throw new ArgumentOutOfRangeException(nameof(type), $"Unknown bracket type {type}.")
		};
	}
}