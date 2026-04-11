using Microsoft.AspNetCore.Mvc;
using TournamentBracket.Application.Divisions.Interfaces;
using TournamentBracket.Application.Divisions.Queries;

namespace TournamentBracket.Api.Controllers;

[ApiController]
[Route("api/divisions")]
public class DivisionsController : ExtendedControllerBase
{
	private readonly IDivisionsService divisionsService;

	public DivisionsController(IDivisionsService divisionsService)
	{
		this.divisionsService = divisionsService;
	}

	[HttpGet]
	public async Task<IActionResult> GetDivisions([FromQuery] DivisionsQuery query)
	{
		var result = await divisionsService.GetDivisionsByCompetitionId(query.CompetitionId);
		return ToActionResult(result);
	}
}