using TournamentBracket.Application.Competitions.Commands;
using TournamentBracket.Application.Competitions.DTO;
using TournamentBracket.Application.Competitions.Queries;
using TournamentBracket.Domain.Competitions;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Competitions.Interfaces;

public interface ICompetitionsService
{
    public Task<Result> CreateCompetition(CreateCompetitionCommand command, CancellationToken ct = default); 
    public Task<Result<IReadOnlyCollection<Competition>>> GetCompetitions(CompetitionsPageQuery query, CancellationToken ct = default);
    public Task<Result<IReadOnlyCollection<Competition>>> GetCompetitionsShorts(CompetitionsPageQuery query, CancellationToken ct = default);
    public Task<Result<int>> GetCount(CompetitionsFilter filter, CancellationToken ct = default); 
    public Task<Result<Competition>> GetCompetition(Guid id, CancellationToken ct = default);
    public Task<Result> UpdateCompetition(UpdateCompetitionCommand command, CancellationToken ct = default);
    public Task<Result> DeleteCompetition(Guid id, CancellationToken ct = default);
    public Task<Result> AddCompetitor(Guid competitionId, AddCompetitorCommand command, CancellationToken ct = default);
    public Task<Result> RemoveCompetitor(Guid competitionId, RemoveCompetitorCommand command, CancellationToken ct = default);
}
