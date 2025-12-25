namespace TournamentBracket.Application.Common.Queries;

public class PageQuery
{
    public int? Page { get; set; }
    public int Count { get; set; } = 20;
}