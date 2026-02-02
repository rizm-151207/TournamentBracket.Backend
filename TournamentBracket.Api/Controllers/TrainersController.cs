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
public class TrainersController : ExtendedControllerBase
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
    [Authorize(Roles = "Organizer, Administrator")]
    public async Task<IActionResult> CreateTrainer(
        [FromBody] CreateTrainerCommand command,
        [FromServices] CreateTrainerCommandValidator commandValidator)
    {
        var validationResult = await commandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var trainersResult = await trainerService.CreateTrainer(command);
        return ToActionResult(trainersResult, 201);
    }

    [HttpPatch]
    [Authorize(Roles = "Organizer, Administrator")]
    public async Task<IActionResult> UpdateTrainer(
        [FromBody] UpdateTrainerCommand command,
        [FromServices] UpdateTrainerCommandValidator commandValidator)
    {
        var validationResult = await commandValidator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var result = await trainerService.UpdateTrainer(command);
        return ToActionResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTrainerById([FromRoute] Guid id)
    {
        var result = await trainerService.GetTrainer(id);
        return ToActionResult(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Organizer, Administrator")]
    public async Task<IActionResult> DeleteTrainer([FromRoute] Guid id)
    {
        var result = await trainerService.DeleteTrainer(id);
        return ToActionResult(result);
    }
}