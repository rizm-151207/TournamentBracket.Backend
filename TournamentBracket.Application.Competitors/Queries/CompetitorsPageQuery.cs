using TournamentBracket.Application.Competitors.DTO;

namespace TournamentBracket.Application.Competitors.Queries;

public record CompetitorsPageQuery
{
    public CompetitorsPageQuery(string? fio, int? page, int? count)
    {
        Filter = new CompetitorsFilter(fio);
        Page = page;
        Count = count ?? 20;
    }

    public CompetitorsFilter Filter { get; set; }
    public int? Page { get; set; }
    public int Count { get; set; }
}