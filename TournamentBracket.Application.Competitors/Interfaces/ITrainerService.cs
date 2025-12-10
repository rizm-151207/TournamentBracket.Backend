using TournamentBracket.Application.Common.Queries;
using TournamentBracket.Application.Competitors.Commands;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Competitors.Interfaces;

public interface ITrainerService
{
    public Task<Result> CreateTrainer(CreateTrainerCommand command, CancellationToken ct = default); 
    public Task<Result<IReadOnlyCollection<Trainer>>> GetTrainers(PageQuery query, CancellationToken ct = default);
    public Task<Result<int>> GetCount(CancellationToken ct = default); 
    public Task<Result<Trainer>> GetTrainer(Guid id, CancellationToken ct = default);
    public Task<Result> UpdateTrainer(UpdateTrainerCommand command, CancellationToken ct = default);
    public Task<Result> DeleteTrainer(Guid id, CancellationToken ct = default);
}