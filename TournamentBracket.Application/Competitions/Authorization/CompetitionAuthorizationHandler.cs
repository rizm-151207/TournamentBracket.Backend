using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TournamentBracket.Application.Common.Authorization;
using TournamentBracket.Domain.Competitions;

namespace TournamentBracket.Application.Competitions.Authorization;

public class CompetitionAuthorizationHandler : AuthorizationHandler<BaseResourceRequirement, Competition>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BaseResourceRequirement requirement,
        Competition resource)
    {
        if (context.User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value.ToLower() == "administrator"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var userEmailClaim = context.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Email);
        if (userEmailClaim is null)
        {
            context.Fail(new AuthorizationFailureReason(this, "Can't find Email in token"));
            return Task.CompletedTask;
        }

        if (userEmailClaim.Value == resource.OwnerEmail)
            context.Succeed(requirement);
        else
            context.Fail();

        return Task.CompletedTask;
    }
}