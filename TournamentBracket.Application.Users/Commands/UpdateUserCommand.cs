namespace TournamentBracket.Application.Users.Commands;

public record UpdateUserCommand(
    Guid UserId,
    string Username,
    String Password,
    String Email);