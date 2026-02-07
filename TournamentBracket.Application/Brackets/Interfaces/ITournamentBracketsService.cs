using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Matches;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Brackets.Interfaces;

public interface ITournamentBracketsService
{
    public Task<Result<Bracket>> GetBracket(Guid id, BracketType type, CancellationToken ct = default);

    public Task<Result<IReadOnlyCollection<Bracket>>> GetBracketsByIds(IReadOnlyCollection<Guid> ids,
        CancellationToken ct = default);

    public Task<Result<Bracket>> CreateBracket(List<Competitor> competitors,
        CancellationToken ct = default);

    public Task<Result<Bracket>> AddCompetitorToBracketAuto(Guid bracketId, BracketType bracketType,
        Competitor competitor, CancellationToken ct = default);

    public Task<Result> RemoveCompetitorFromBracketAuto(Guid bracketId, BracketType bracketType,
        Competitor competitor, CancellationToken ct = default);

    public Task<Result> UpdateBracketFromMatch(Guid bracketId, Match match, CancellationToken ct = default);
}