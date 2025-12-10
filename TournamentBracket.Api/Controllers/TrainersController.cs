using Microsoft.AspNetCore.Mvc;
using TournamentBracket.Application.Common.Queries;
using TournamentBracket.Application.Competitors.Commands;
using TournamentBracket.Application.Competitors.Interfaces;

namespace TournamentBracket.Api.Controllers;

[ApiController]
[Route("api/trainers")]
public class TrainersController: ControllerBase
{
    private readonly ITrainerService trainerService;

    public TrainersController(ITrainerService trainerService)
    {
        this.trainerService = trainerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrainers([FromQuery] PageQuery query)
    {
        var trainersResult = await trainerService.GetTrainers(query);
        return trainersResult.IsSuccess
            ? Ok(trainersResult.Item)
            : BadRequest(trainersResult.Error);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTrainer([FromBody] CreateTrainerCommand command)
    {
        var trainersResult = await trainerService.CreateTrainer(command);
        return trainersResult.IsSuccess
            ? Ok()
            : BadRequest(trainersResult.Error);
    }
    
    [HttpPatch]
    public async Task<IActionResult> UpdateTrainer([FromBody] UpdateTrainerCommand command)
    {
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
    public async Task<IActionResult> DeleteTrainer([FromRoute] Guid id)
    {
        var result = await trainerService.GetTrainer(id);
        return result.IsSuccess
            ? Ok(result.Item)
            : result.Error?.Code == 404
                ? NotFound(result.Error)
                : BadRequest(result.Error);
    }
}