using System.Security.Claims;

namespace TournamentBracket.Api.Extensions;

public static class ClaimPrincipalExtensions
{
	public static string? GetClaimByType(this ClaimsPrincipal principal, string claimType)
	{
		return principal.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
	}

	public static string GetClaimByTypeEnsure(this ClaimsPrincipal principal, string claimType)
	{
		var claimValue = principal.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
		if (string.IsNullOrWhiteSpace(claimValue))
			throw new ArgumentException($"Claim {claimType} not found in token");
		return claimValue;
	}
}