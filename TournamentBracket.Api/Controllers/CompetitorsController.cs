using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentBracket.Application.Common.Responses;
using TournamentBracket.Application.Competitors.Commands;
using TournamentBracket.Application.Competitors.Interfaces;
using TournamentBracket.Application.Competitors.Queries;
using TournamentBracket.Application.Competitors.Validators;
using TournamentBracket.Domain.Competitors;

namespace TournamentBracket.Api.Controllers;

[ApiController]
[Route("api/competitors")]
public class CompetitorsController : ExtendedControllerBase
{
    private readonly ICompetitorService competitorService;

    public CompetitorsController(ICompetitorService competitorService)
    {
        this.competitorService = competitorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompetitors(
        [FromQuery] CompetitorsPageQuery query,
        [FromServices] CompetitorsPageQueryValidator queryValidator)
    {
        var validationResult = await queryValidator.ValidateAsync(query);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var competitorsResult = await competitorService.GetCompetitors(query);
        if (!competitorsResult.IsSuccess)
            return BadRequest(competitorsResult.Error);

        var countResult = await competitorService.GetCount(query.CreateFilter());
        if (!countResult.IsSuccess)
            return BadRequest(countResult.Error);

        return Ok(new PageResponse<Competitor>(competitorsResult.Item!, countResult.Item));
    }

    [HttpPost]
    [Authorize(Roles = "Organizer, Administrator")]
    public async Task<IActionResult> CreateCompetitor(
        [FromBody] CreateCompetitorCommand command,
        [FromServices] CreateCompetitorCommandValidator commandValidator)
    {
        var validationResult = await commandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var creationResult = await competitorService.CreateCompetitor(command);
        return ToActionResult(creationResult, 201);
    }

    [HttpPatch]
    [Authorize(Roles = "Organizer, Administrator")]
    public async Task<IActionResult> UpdateCompetitor(
        [FromBody] UpdateCompetitorCommand command,
        [FromServices] UpdateCompetitorCommandValidator commandValidator)
    {
        var validationResult = await commandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var updateResult = await competitorService.UpdateCompetitor(command);
        return ToActionResult(updateResult);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompetitorById([FromRoute] Guid id)
    {
        var result = await competitorService.GetCompetitor(id);
        return ToActionResult(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Organizer, Administrator")]
    public async Task<IActionResult> DeleteCompetitor([FromRoute] Guid id)
    {
        var result = await competitorService.DeleteCompetitor(id);
        return ToActionResult(result);
    }
}