using TournamentBracket.Domain.Competitors;

namespace TournamentBracket.Application.Competitors.Responses;

public record CompetitorsPageResponse(IReadOnlyCollection<Competitor> Data, int AllCount);