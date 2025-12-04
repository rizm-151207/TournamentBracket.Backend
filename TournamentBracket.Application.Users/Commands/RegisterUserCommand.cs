namespace TournamentBracket.Application.Users.Commands;

public record RegisterUserCommand(
    String Email,
    String Password,
    String FirstName,
    String LastName,
    String? MiddleName);