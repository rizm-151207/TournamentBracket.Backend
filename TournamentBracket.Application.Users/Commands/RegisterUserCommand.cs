namespace TournamentBracket.Application.Users.Commands;

public record RegisterUserCommand(
    String Username,
    String Password,
    String Email);