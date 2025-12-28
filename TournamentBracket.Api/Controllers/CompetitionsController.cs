using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentBracket.Application.Common.Responses;
using TournamentBracket.Application.Competitions.Commands;
using TournamentBracket.Application.Competitions.Interfaces;
using TournamentBracket.Application.Competitions.Queries;
using TournamentBracket.Application.Competitions.Validators;
using TournamentBracket.Domain.Competitions;

namespace TournamentBracket.Api.Controllers;

[ApiController]
[Route("api/competitions")]
public class CompetitionsController : ControllerBase
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
        if(!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        
        var findResult = await competitionsService.GetCompetitionsShorts(query);
        if (!findResult.IsSuccess)
            return BadRequest(findResult);

        var countResult = await competitionsService.GetCount(query.CreateFilter());
        return countResult.IsSuccess
            ? Ok(new PageResponse<Competition>(findResult.Item!, countResult.Item))
            : BadRequest(findResult.Error);
    }

    [HttpPost]
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> CreateCompetition(
        [FromBody] CreateCompetitionCommand command,
        [FromServices] CreateCompetitionCommandValidator validator)
    {
        var validationResult = await validator.ValidateAsync(command);
        if(!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        
        var creationResult = await competitionsService.CreateCompetition(command);
        return creationResult.IsSuccess ? Created() : BadRequest(creationResult.Error);
    }

    [HttpPatch]
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> UpdateCompetition(
        [FromBody] UpdateCompetitionCommand command,
        [FromServices] UpdateCompetitionCommandValidator validator)
    {
        var validationResult = await validator.ValidateAsync(command);
        if(!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        
        var updateResult = await competitionsService.UpdateCompetition(command);
        return updateResult.IsSuccess
            ? Ok()
            : updateResult.Error?.Code == 404
                ? NotFound(updateResult.Error)
                : BadRequest(updateResult.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompetitionById([FromRoute] Guid id)
    {
        var result = await competitionsService.GetCompetition(id);
        return result.IsSuccess
            ? Ok(result.Item)
            : result.Error?.Code == 404
                ? NotFound(result.Error)
                : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> DeleteCompetition([FromRoute] Guid id)
    {
        var result = await competitionsService.DeleteCompetition(id);
        return result.IsSuccess
            ? Ok()
            : result.Error?.Code == 404
                ? NotFound(result.Error)
                : BadRequest(result.Error);
    }
}