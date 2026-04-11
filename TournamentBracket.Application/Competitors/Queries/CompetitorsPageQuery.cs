using TournamentBracket.Application.Common.Queries;
using TournamentBracket.Application.Competitors.DTO;

namespace TournamentBracket.Application.Competitors.Queries;

public class CompetitorsPageQuery : PageQuery
{
	public string? FIO { get; set; }

	public CompetitorsFilter CreateFilter() => new(FIO);
}