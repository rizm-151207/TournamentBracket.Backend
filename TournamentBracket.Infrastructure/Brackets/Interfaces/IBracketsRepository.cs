using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Infrastructure.Brackets.Interfaces;

public interface IBracketsRepository
{
    public Task<Result<Bracket>> GetBracket(Guid id, BracketType bracketType, CancellationToken ct = default);
}