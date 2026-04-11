using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Divisions;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Divisions.Interfaces;

public interface IDivisionsService
{
	public Task<Result<IReadOnlyCollection<Division>>> GetDivisionsByCompetitionId(Guid competitionId, CancellationToken ct = default);
	public Task<Result<Division?>> GetSuitableDivision(Guid competitionId, Competitor competitor, CancellationToken ct = default);
	public Task<Result<Division>> ConstructSuitableDivision(Guid competitionId, Competitor competitor, CancellationToken ct = default);
	public Task<Result<IReadOnlyCollection<Division>>> GetDivisionsWithCompetitor(Guid competitorId, CancellationToken ct = default);
}