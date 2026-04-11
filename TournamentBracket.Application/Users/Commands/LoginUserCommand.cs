namespace TournamentBracket.Application.Users.Commands;

public record LoginUserCommand(
	String Email,
	String Password);