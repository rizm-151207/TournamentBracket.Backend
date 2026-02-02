using Microsoft.AspNetCore.Authorization;

namespace TournamentBracket.Application.Common.Authorization;

public class BaseResourceRequirement : IAuthorizationRequirement
{
    public static BaseResourceRequirement Item = new();
}