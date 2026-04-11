using TournamentBracket.Application.Users.Commands;
using TournamentBracket.Application.Users.DTO;
using TournamentBracket.Domain.Users;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Users.Interfaces;

public interface IAuthService
{
	public Task<Result<User>> Register(RegisterUserCommand command, CancellationToken ct = default);
	public Task<Result<TokensPair>> Login(LoginUserCommand command, CancellationToken ct = default);
	public Task<Result> Logout(string userEmail, CancellationToken ct = default);

	public Task<Result<TokensPair>> RefreshTokens(RefreshTokenCommand command, string userEmail,
		CancellationToken ct = default);
}