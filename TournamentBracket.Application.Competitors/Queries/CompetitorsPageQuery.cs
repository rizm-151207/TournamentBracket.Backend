using TournamentBracket.Application.Common.Queries;
using TournamentBracket.Application.Competitors.DTO;

namespace TournamentBracket.Application.Competitors.Queries;

public class CompetitorsPageQuery : PageQuery
{
    public CompetitorsPageQuery(string? fio, int? page, int? count)
        : base(page, count)
    {
        Filter = new CompetitorsFilter(fio);
        Page = page;
        Count = count ?? 20;
    }

    public CompetitorsFilter Filter { get; set; }
}