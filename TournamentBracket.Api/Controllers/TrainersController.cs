using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentBracket.Application.Common.Queries;
using TournamentBracket.Application.Common.Responses;
using TournamentBracket.Application.Common.Validators;
using TournamentBracket.Application.Competitors.Commands;
using TournamentBracket.Application.Competitors.Interfaces;
using TournamentBracket.Application.Competitors.Validators;
using TournamentBracket.Domain.Competitors;

namespace TournamentBracket.Api.Controllers;

[ApiController]
[Route("api/trainers")]
public class TrainersController : ControllerBase
{
    private readonly ITrainerService trainerService;

    public TrainersController(ITrainerService trainerService)
    {
        this.trainerService = trainerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrainers(
        [FromQuery] PageQuery query,
        [FromServices] PageQueryValidator queryValidator)
    {
        var validationResult = await queryValidator.ValidateAsync(query);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var competitorsResult = await trainerService.GetTrainers(query);
        if (!competitorsResult.IsSuccess)
            return BadRequest(competitorsResult.Error);

        var countResult = await trainerService.GetCount();
        if (!countResult.IsSuccess)
            return BadRequest(countResult.Error);

        return Ok(new PageResponse<Trainer>(competitorsResult.Item!, countResult.Item));
    }

    [HttpPost]
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> CreateTrainer(
        [FromBody] CreateTrainerCommand command,
        [FromServices] CreateTrainerCommandValidator commandValidator)
    {
        var validationResult = await commandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var trainersResult = await trainerService.CreateTrainer(command);
        return trainersResult.IsSuccess
            ? Ok()
            : BadRequest(trainersResult.Error);
    }

    [HttpPatch]
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> UpdateTrainer(
        [FromBody] UpdateTrainerCommand command,
        [FromServices] UpdateTrainerCommandValidator commandValidator)
    {
        var validationResult = await commandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var result = await trainerService.UpdateTrainer(command);
        return result.IsSuccess
            ? Ok()
            : result.Error?.Code == 404
                ? NotFound(result.Error)
                : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTrainerById([FromRoute] Guid id)
    {
        var result = await trainerService.GetTrainer(id);
        return result.IsSuccess
            ? Ok(result.Item)
            : result.Error?.Code == 404
                ? NotFound(result.Error)
                : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Organizer")]
    public async Task<IActionResult> DeleteTrainer([FromRoute] Guid id)
    {
        var result = await trainerService.DeleteTrainer(id);
        return result.IsSuccess
            ? Ok()
            : result.Error?.Code == 404
                ? NotFound(result.Error)
                : BadRequest(result.Error);
    }
}