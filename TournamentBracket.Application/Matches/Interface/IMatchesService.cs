using TournamentBracket.Domain.Competitions;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Matches.Interface;

public interface IMatchesService
{
    public Task<Result> PlanMatches(Competition competition, CancellationToken ct = default);
}