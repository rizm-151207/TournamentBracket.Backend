using System.Security.Claims;

namespace TournamentBracket.Api.Extensions;

public static class ClaimPrincipalExtensions
{
    public static string? GetClaimByType(this ClaimsPrincipal principal, string claimType)
    {
        return principal.Claims.FirstOrDefault(c=>c.Type == claimType)?.Value;
    }
}