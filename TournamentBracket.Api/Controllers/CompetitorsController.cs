using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentBracket.Application.Competitors.Commands;
using TournamentBracket.Application.Competitors.Interfaces;
using TournamentBracket.Application.Competitors.Queries;
using TournamentBracket.Application.Competitors.Responses;

namespace TournamentBracket.Api.Controllers;

[ApiController]
[Route("api/competitors")]
public class CompetitorsController : ControllerBase
{
    private readonly ICompetitorService competitorService;

    public CompetitorsController(ICompetitorService competitorService)
    {
        this.competitorService = competitorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCompetitors([FromQuery] CompetitorsPageQuery query)
    {
        var competitorsResult = await competitorService.GetCompetitors(query);
        if (!competitorsResult.IsSuccess)
            return BadRequest(competitorsResult.Error);

        var countResult = await competitorService.GetCount(query.Filter);
        if (!countResult.IsSuccess)
            return BadRequest(countResult.Error);

        return Ok(new CompetitorsPageResponse(competitorsResult.Item!, countResult.Item));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompetitor([FromBody] CreateCompetitorCommand command)
    {
        var creationResult = await competitorService.CreateCompetitor(command);
        return creationResult.IsSuccess ? Created() : BadRequest(creationResult.Error);
    }


    [HttpPatch]
    public async Task<IActionResult> UpdateCompetitor([FromBody] UpdateCompetitorCommand command)
    {
        var updateResult = await competitorService.UpdateCompetitor(command);
        return updateResult.IsSuccess
            ? Ok()
            : updateResult.Error?.Code == 404
                ? NotFound(updateResult.Error)
                : BadRequest(updateResult.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompetitorById([FromRoute] Guid id)
    {
        var result = await competitorService.GetCompetitor(id);
        return result.IsSuccess
            ? Ok(result.Item)
            : result.Error?.Code == 404
                ? NotFound(result.Error)
                : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompetitor([FromRoute] Guid id)
    {
        var result = await competitorService.DeleteCompetitor(id);
        return result.IsSuccess
            ? Ok()
            : result.Error?.Code == 404
                ? NotFound(result.Error)
                : BadRequest(result.Error);
    }
}