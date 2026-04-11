namespace TournamentBracket.Application.Users.DTO;

public class JwtTokenConfiguration
{
	public required string Issuer { get; set; }
	public required string Audience { get; set; }
	public required string Secret { get; set; }
	public required int ExpireInMinutes { get; set; }
	public required int ExpireRefreshTokenInMinutes { get; set; }
}