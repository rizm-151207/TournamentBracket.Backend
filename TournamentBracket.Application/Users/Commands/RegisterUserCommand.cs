using TournamentBracket.Domain.Users;

namespace TournamentBracket.Application.Users.Commands;

public record RegisterUserCommand(
    string Email,
    string Password,
    UserRole Role);