using Microsoft.AspNetCore.Mvc;
using TournamentBracket.Application.Brackets.Interfaces;
using TournamentBracket.Domain.Brackets;

namespace TournamentBracket.Api.Controllers;

[ApiController]
[Route("api/brackets")]
public class BracketsController : ExtendedControllerBase
{
	private readonly ITournamentBracketsService tournamentBracketsService;

	public BracketsController(
		ITournamentBracketsService tournamentBracketsService)
	{
		this.tournamentBracketsService = tournamentBracketsService;
	}

	[HttpGet("{bracketType}/{bracketId}")]
	public async Task<IActionResult> GetBracket([FromRoute] Guid bracketId, [FromRoute] BracketType bracketType)
	{
		var result = await tournamentBracketsService.GetBracket(bracketId, bracketType);
		return ToActionResult(result);
	}
}