namespace TournamentBracket.Application.Users.Commands;

public record RegisterUserCommand(
    string Email,
    string Password,
    string Role);