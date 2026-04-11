using Microsoft.AspNetCore.Authorization;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Common.Authorization.Interfaces;

public interface IResourceAuthorizationService
{
	public Task<Result> Authorize<TResource>(TResource resource, IAuthorizationRequirement? requirement = null);
}