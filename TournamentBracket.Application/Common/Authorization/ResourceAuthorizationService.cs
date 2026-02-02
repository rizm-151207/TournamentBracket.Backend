using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using TournamentBracket.Application.Common.Authorization.Interfaces;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Common.Authorization;

public class ResourceAuthorizationService : IResourceAuthorizationService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IAuthorizationService authorizationService;

    public ResourceAuthorizationService(IHttpContextAccessor httpContextAccessor,
        IAuthorizationService authorizationService)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.authorizationService = authorizationService;
    }

    public async Task<Result> Authorize<TResource>(TResource resource, IAuthorizationRequirement? requirement = null)
    {
        requirement ??= BaseResourceRequirement.Item;
        var user = httpContextAccessor.HttpContext?.User
                   ?? throw new InvalidOperationException(
                       $"Can't authorize {nameof(HttpContent)} or {nameof(ClaimsPrincipal)} null");
        var authorizeResult = await authorizationService.AuthorizeAsync(user, resource, requirement);

        return authorizeResult.Succeeded
            ? Result.Success()
            : Result.Failed(FailureToError(authorizeResult.Failure));
    }

    private Error FailureToError(AuthorizationFailure failure)
    {
        return new Error(string.Join("\n ", failure.FailureReasons.Select(r => r.Message)), 403);
    }
}