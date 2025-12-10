using TournamentBracket.Application.Common.Queries;

namespace TournamentBracket.Application.Common.Helpers;

public static class QueryableExtensions
{
    public static IQueryable<T> SelectPage<T>(this IQueryable<T> queryable, PageQuery pageQuery)
    {
        if (!pageQuery.Page.HasValue)
            return queryable;

        return queryable
            .Skip((pageQuery.Page.Value - 1) * pageQuery.Count)
            .Take(pageQuery.Count);
    }
}