using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentBracket.Api.Extensions;
using TournamentBracket.Application.Common.Responses;
using TournamentBracket.Application.Competitions.Commands;
using TournamentBracket.Application.Competitions.Interfaces;
using TournamentBracket.Application.Competitions.Queries;
using TournamentBracket.Application.Competitions.Validators;
using TournamentBracket.Application.Matches.Commands;
using TournamentBracket.Domain.Competitions;

namespace TournamentBracket.Api.Controllers;

[ApiController]
[Route("api/competitions")]
public class CompetitionsController : ExtendedControllerBase
{
	private readonly ICompetitionsService competitionsService;

	public CompetitionsController(ICompetitionsService competitionsService)
	{
		this.competitionsService = competitionsService;
	}

	[HttpGet]
	public async Task<IActionResult> GetCompetitions(
		[FromQuery] CompetitionsPageQuery query,
		[FromServices] CompetitionsPageQueryValidator validator)
	{
		var validationResult = await validator.ValidateAsync(query);
		if (!validationResult.IsValid)
			return BadRequest(validationResult.Errors);

		var findResult = await competitionsService.GetCompetitions(query);
		if (!findResult.IsSuccess)
			return BadRequest(findResult);

		var countResult = await competitionsService.GetCount(query.CreateFilter());
		return countResult.IsSuccess
			? Ok(new PageResponse<Competition>(findResult.Item!, countResult.Item))
			: BadRequest(findResult.Error);
	}

	[HttpPost]
	[Authorize(Roles = "Organizer, Administrator")]
	public async Task<IActionResult> CreateCompetition(
		[FromBody] CreateCompetitionCommand command,
		[FromServices] CreateCompetitionCommandValidator validator)
	{
		var validationResult = await validator.ValidateAsync(command);
		if (!validationResult.IsValid)
			return BadRequest(validationResult.Errors);

		var userEmail = HttpContext.User.GetClaimByTypeEnsure(ClaimTypes.Email);
		var creationResult = await competitionsService.CreateCompetition(command, userEmail);
		return ToActionResult(creationResult, 201);
	}

	[HttpPatch]
	[Authorize(Roles = "Organizer, Administrator")]
	public async Task<IActionResult> UpdateCompetition(
		[FromBody] UpdateCompetitionCommand command,
		[FromServices] UpdateCompetitionCommandValidator validator)
	{
		var validationResult = await validator.ValidateAsync(command);
		if (!validationResult.IsValid)
			return BadRequest(validationResult.Errors);

		var updateResult = await competitionsService.UpdateCompetition(command);
		return ToActionResult(updateResult);
	}

	[HttpPatch("{id}/addcompetitor")]
	[Authorize(Roles = "Organizer, Administrator")]
	public async Task<IActionResult> AddCompetitor(
		[FromRoute] Guid id,
		[FromBody] AddCompetitorCommand command)
	{
		var result = await competitionsService.AddCompetitorAuto(id, command);
		return ToActionResult(result);
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetCompetitionById([FromRoute] Guid id)
	{
		var result = await competitionsService.GetCompetitionWithoutCompetitors(id);
		return ToActionResult(result);
	}

	[HttpGet("{id}/matches")]
	public async Task<IActionResult> GetCompetitionById([FromRoute] Guid id, [FromQuery] MatchesQuery query)
	{
		var result = await competitionsService.GetMatches(id, query);
		return ToActionResult(result);
	}

	[HttpDelete("{id}")]
	[Authorize(Roles = "Organizer, Administrator")]
	public async Task<IActionResult> DeleteCompetition([FromRoute] Guid id)
	{
		var result = await competitionsService.DeleteCompetition(id);
		return ToActionResult(result);
	}

	[HttpDelete("{id}/removecompetitor")]
	[Authorize(Roles = "Organizer, Administrator")]
	public async Task<IActionResult> RemoveCompetitor(
		[FromRoute] Guid id,
		[FromBody] RemoveCompetitorCommand command)
	{
		var result = await competitionsService.RemoveCompetitor(id, command);
		return ToActionResult(result);
	}

	[Authorize(Roles = "Organizer, Administrator")]
	[HttpPost("{id}/matches/addevent")]
	public async Task<IActionResult> UpdateMatch([FromRoute] Guid id, [FromBody] UpdateMatchCommand command)
	{
		var updateResult = await competitionsService.AddMatchEvent(id, command);
		return ToActionResult(updateResult);
	}
}