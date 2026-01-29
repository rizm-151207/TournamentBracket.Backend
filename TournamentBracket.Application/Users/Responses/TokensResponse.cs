namespace TournamentBracket.Application.Users.Responses;

public record TokensResponse(
    string? AccessToken,
    string? RefreshToken);