namespace TournamentBracket.Application.Common.Queries;

public class PageQuery
{
    public PageQuery(
        int? page,
        int? count)
    {
        Page = page;
        Count = count ?? 20;
    }

    public int? Page { get; set; }
    public int Count { get; set; }
}