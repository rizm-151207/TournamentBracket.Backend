using TournamentBracket.Application.Common.Queries;
using TournamentBracket.Application.Competitions.DTO;

namespace TournamentBracket.Application.Competitions.Queries;

public class CompetitionsPageQuery : PageQuery
{
	public string? Name { get; set; }

	public CompetitionsFilter CreateFilter() => new(Name);
}