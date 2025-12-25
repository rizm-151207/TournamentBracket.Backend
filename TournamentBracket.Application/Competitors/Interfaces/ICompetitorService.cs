using TournamentBracket.Application.Competitors.Commands;
using TournamentBracket.Application.Competitors.DTO;
using TournamentBracket.Application.Competitors.Queries;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Competitors.Interfaces;

public interface ICompetitorService
{
    public Task<Result> CreateCompetitor(CreateCompetitorCommand command, CancellationToken ct = default); 
    public Task<Result<IReadOnlyCollection<Competitor>>> GetCompetitors(CompetitorsPageQuery query, CancellationToken ct = default);
    public Task<Result<int>> GetCount(CompetitorsFilter filter, CancellationToken ct = default); 
    public Task<Result<Competitor>> GetCompetitor(Guid id, CancellationToken ct = default);
    public Task<Result> UpdateCompetitor(UpdateCompetitorCommand command, CancellationToken ct = default);
    public Task<Result> DeleteCompetitor(Guid id, CancellationToken ct = default);
}