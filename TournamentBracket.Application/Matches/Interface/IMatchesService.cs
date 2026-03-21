using TournamentBracket.Application.Matches.Commands;
using TournamentBracket.Domain.Matches;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Matches.Interface;

public interface IMatchesService
{
    public Task<Result> AddMatchEvent(UpdateMatchCommand command, CancellationToken ct = default);
    public Task<Result<IReadOnlyCollection<Match>>> GetUnfinishedMatchesForCompetitor(Guid competitorId, CancellationToken ct = default);
}