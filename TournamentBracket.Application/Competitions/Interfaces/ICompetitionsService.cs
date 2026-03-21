using TournamentBracket.Application.Competitions.Commands;
using TournamentBracket.Application.Competitions.DTO;
using TournamentBracket.Application.Competitions.Queries;
using TournamentBracket.Application.Competitions.Responses;
using TournamentBracket.Application.Matches.Commands;
using TournamentBracket.Domain.Competitions;
using TournamentBracket.Domain.Matches;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Competitions.Interfaces;

public interface ICompetitionsService
{
    public Task<Result<CreateCompetitionResponse>> CreateCompetition(CreateCompetitionCommand command, string userEmail, CancellationToken ct = default); 
    public Task<Result<IReadOnlyCollection<Competition>>> GetCompetitions(CompetitionsPageQuery query, CancellationToken ct = default);
    public Task<Result<int>> GetCount(CompetitionsFilter filter, CancellationToken ct = default); 
    public Task<Result<Competition>> GetCompetitionWithoutCompetitors(Guid id, CancellationToken ct = default);
    public Task<Result<Competition>> GetCompetitionFull(Guid id, CancellationToken ct = default);
    public Task<Result<IReadOnlyCollection<Match>>> GetMatches(Guid competitionId, MatchesQuery query, CancellationToken ct = default);
    public Task<Result> UpdateCompetition(UpdateCompetitionCommand command, CancellationToken ct = default);
    public Task<Result> DeleteCompetition(Guid id, CancellationToken ct = default);
    public Task<Result> AddCompetitorAuto(Guid competitionId, AddCompetitorCommand command, CancellationToken ct = default);
    public Task<Result> RemoveCompetitor(Guid competitionId, RemoveCompetitorCommand command, CancellationToken ct = default);
    public Task<Result> AddMatchEvent(Guid competitionId, UpdateMatchCommand command, CancellationToken ct = default);
}
