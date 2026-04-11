using TournamentBracket.Application.Brackets.Interfaces;
using TournamentBracket.Application.Common.Helpers;
using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Matches;
using TournamentBracket.Infrastructure.Brackets.Interfaces;
using TournamentBracket.Infrastructure.Common;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Brackets.Services;

public class TournamentBracketsService : ITournamentBracketsService
{
    private readonly AppDbContext dbContext;
    private readonly BracketFactoryResolver bracketFactoryResolver;
    private readonly BracketTypeResolver bracketTypeResolver;
    private readonly IBracketsRepository bracketsRepository;

    public TournamentBracketsService(
        AppDbContext dbContext,
        BracketFactoryResolver bracketFactoryResolver,
        BracketTypeResolver bracketTypeResolver,
        IBracketsRepository bracketsRepository)
    {
        this.dbContext = dbContext;
        this.bracketFactoryResolver = bracketFactoryResolver;
        this.bracketTypeResolver = bracketTypeResolver;
        this.bracketsRepository = bracketsRepository;
    }

    public async Task<Result<Bracket>> GetBracket(Guid id, BracketType type, CancellationToken ct = default)
    {
        try
        {
            var bracketResult = await bracketsRepository.GetBracket(id, type, ct);
            return bracketResult;
        }
        catch (Exception e)
        {
            e.ToResult();
            throw;
        }
    }

    public async Task<Result<IReadOnlyCollection<Bracket>>> GetBracketsByIds(IReadOnlyCollection<Guid> ids,
        CancellationToken ct = default)
    {
        var bracketResult = await bracketsRepository.GetBrackets(ids, ct);
        return bracketResult;
    }

    public async Task<Result<Bracket>> CreateBracket(
        List<Competitor> competitors,
        CancellationToken ct = default)
    {
        try
        {
            var bracket = bracketFactoryResolver.ResolveByCompetitorsCount(competitors.Count)
                .CreateBracket(competitors);

            await dbContext.AddAsync(bracket, ct);
            await dbContext.SaveChangesAsync(ct);

            return Result<Bracket>.Success(bracket);
        }
        catch (Exception e)
        {
            return e.ToResult<Bracket>();
        }
    }

    public async Task<Result<Bracket>> AddCompetitorToBracketAuto(Guid bracketId, BracketType bracketType,
        Competitor competitor, CancellationToken ct = default)
    {
        try
        {
            var bracketResult = await GetBracket(bracketId, bracketType, ct);
            if (!bracketResult.IsSuccess)
                return bracketResult;
            var bracket = bracketResult.Item!;

            var competitors = bracket.GetAllCompetitors();
            var newBracketType = bracketTypeResolver.Resolve(competitors.Count + 1);
            if (bracket.Type != newBracketType)
            {
				await bracketsRepository.RemoveBracket(bracket, ct);
                return await CreateBracket([.. competitors, competitor], ct);
            }

            if (bracket.HasFreeMatch())
                return TryAddCompetitor(bracket);

            bracketFactoryResolver.ResolveByBracketType(newBracketType).ExtendBracket(bracket);
            if (bracket.HasFreeMatch())
                return TryAddCompetitor(bracket);

            throw new Exception($"Bracket with id {bracketId} has no mathces to add after extensions");
        }
        catch (Exception e)
        {
            return e.ToResult<Bracket>();
        }

        Result<Bracket> TryAddCompetitor(Bracket bracket)
        {
            var successAdd = bracket.TryAddCompetitorAuto(competitor);
            return successAdd
                ? Result<Bracket>.Success(bracket)
                : Result<Bracket>.FailedWith("Something goes wrong while adding competitor to bracket");
        }
    }

    public async Task<Result<Bracket>> RemoveCompetitorFromBracketAuto(Guid bracketId, BracketType bracketType,
        Competitor competitor,
        CancellationToken ct = default)
    {
        try
        {
            var bracketResult = await GetBracket(bracketId, bracketType, ct);
            if (!bracketResult.IsSuccess)
                return bracketResult;
            var bracket = bracketResult.Item!;

            var competitors = bracket.GetAllCompetitors();
            var currentMatches = bracket.GetAllMatches();
            if (competitors.Count == 1 && competitors[0] == competitor)
            {
                dbContext.Matches.RemoveRange(currentMatches);
                var removeResult = await bracketsRepository.RemoveBracket(bracket, ct);
            }

            var newBracketType = bracketTypeResolver.Resolve(competitors.Count - 1);
            if (bracket.Type != newBracketType)
            {
                var removeOldBracketResult = await bracketsRepository.RemoveBracket(bracket, ct);
                if (!removeOldBracketResult.IsSuccess)
                    return Result<Bracket>.FailedWith(removeOldBracketResult.Error!);

                return await CreateBracket([.. competitors.Except([competitor])], ct);
            }

            var successRemove = bracket.TryRemoveCompetitorAuto(competitor, out var hasEmptyMatch);
            if (!successRemove)
                return Result<Bracket>.FailedWith("Something goes wrong while removing competitor");

            var bracketFactory = bracketFactoryResolver.ResolveByBracketType(newBracketType);
            if (bracketFactory.NeedToReduce(bracket))
            {
                bracketFactory.ReduceBracket(bracket);

                var newMatches = bracket.GetAllMatches();
                var trimResult = await bracketsRepository.TrimBracket(bracket, ct);
                if (!trimResult.IsSuccess)
                    return Result<Bracket>.FailedWith(trimResult.Error!);
                dbContext.Matches.RemoveRange(currentMatches.Except(newMatches));
            }
            else if (hasEmptyMatch)
                bracketFactory.RebalanceBracket(bracket);

            return Result<Bracket>.Success(bracket);
        }
        catch (Exception e)
        {
            return e.ToResult<Bracket>();
        }
    }

    public async Task<Result> UpdateBracketFromMatch(Guid bracketId, Match match, CancellationToken ct = default)
    {
        try
        {
            var bracketResult = await bracketsRepository.GetBracketById(bracketId, ct);
            if (!bracketResult.IsSuccess)
                return bracketResult;

            bracketResult.Item!.RefreshBracketAfterMatchUpdate(match);
            return Result.Success();
        }
        catch (Exception e)
        {
            return e.ToResult();
        }
    }
}