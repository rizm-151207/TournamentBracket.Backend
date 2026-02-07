using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Infrastructure.Brackets.Interfaces;

public interface IBracketsRepository
{
    public Task<Result<Bracket>> GetBracket(Guid id, BracketType bracketType, CancellationToken ct = default);
    public Task<Result<IReadOnlyCollection<Bracket>>> GetBrackets(IReadOnlyCollection<Guid> ids,  CancellationToken ct = default);
    public Task<Result<Bracket>> GetBracketById(Guid id, CancellationToken ct = default);
    public Task<Result> RemoveBracket(Bracket bracket, CancellationToken ct = default);
    public Task<Result> TrimBracket(Bracket bracket, CancellationToken ct = default);
}