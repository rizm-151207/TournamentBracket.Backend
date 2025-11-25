namespace TournamentBracket.Application.Users.DTO;

public record TokensResponse(
    bool IsLogin,
    string? RefreshToken = null);