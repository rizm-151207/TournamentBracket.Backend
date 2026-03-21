using TournamentBracket.Domain.Competitions;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Competitions.Interfaces;

public interface ICompetitionPlanner
{
    public Task<Result> PlanMatches(Competition competition, CancellationToken ct = default);
}
