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

    public async Task<Result<Bracket>> AddCompetitorToBracket(Guid bracketId, BracketType bracketType,
        Competitor competitor, CancellationToken ct = default)
    {
        try
        {
            var bracketResult = await GetBracket(bracketId, bracketType, ct);
            if (!bracketResult.IsSuccess)
                return bracketResult;
            var bracket = bracketResult.Item!;

            var competitors = bracket.GetAllCompetitors();
            var newBracketType = bracketTypeResolver.Resolve(competitors.Count);
            if (bracket.Type != newBracketType)
                return await CreateBracket(competitors, ct);

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