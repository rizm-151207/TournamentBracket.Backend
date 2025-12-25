namespace TournamentBracket.Application.Common.Responses;

public record PageResponse<TItem>(IReadOnlyCollection<TItem> Data, int AllCount);