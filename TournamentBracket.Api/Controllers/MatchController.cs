using Microsoft.AspNetCore.Mvc;
using TournamentBracket.Application.Matches.Commands;
using TournamentBracket.Application.Matches.Interface;

namespace TournamentBracket.Api.Controllers;

[ApiController]
[Route("api/matches")]
public class MatchController : ExtendedControllerBase
{
    private readonly IMatchesService matchService;

    public MatchController(IMatchesService matchService)
    {
        this.matchService = matchService;
    }

    [HttpPost("addevent")]
    public async Task<IActionResult> UpdateMatch([FromBody] UpdateMatchCommand command)
    {
        var updateResult = await matchService.AddMatchEvent(command);
        return ToActionResult(updateResult);
    }
}