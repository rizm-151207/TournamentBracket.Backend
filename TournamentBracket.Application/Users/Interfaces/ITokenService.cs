using TournamentBracket.Domain.Users;

namespace TournamentBracket.Application.Users.Interfaces;

public interface ITokenService
{
	public ValueTask<string> GenerateAccessToken(User user, CancellationToken ct = default);
	public ValueTask<string> GenerateRefreshToken(CancellationToken ct = default);
}