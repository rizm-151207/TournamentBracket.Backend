using Microsoft.EntityFrameworkCore;
using TournamentBracket.Application.Divisions.Interfaces;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Divisions;
using TournamentBracket.Infrastructure.Common;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Divisions.Services;

public class DivisionsService : IDivisionsService
{
    private readonly AppDbContext dbContext;
    private readonly DivisionsFactory divisionsFactory;

    public DivisionsService(AppDbContext dbContext, DivisionsFactory divisionsFactory)
    {
        this.dbContext = dbContext;
        this.divisionsFactory = divisionsFactory;
    }

    public async Task<Result<IReadOnlyCollection<Division>>> GetDivisionByCompetitionId(Guid competitionId,
        CancellationToken ct = default)
    {
        var divisions = await dbContext.Divisions
            .Include(d => d.Competitors)
            .Where(d => d.CompetitionId == competitionId)
            .ToListAsync(ct);
        return Result<IReadOnlyCollection<Division>>.Success(divisions);
    }

    public async Task<Result<Division?>> GetSuitableDivision(Guid competitionId, Competitor competitor,
        CancellationToken ct = default)
    {
        var competitionsResult = await GetDivisionByCompetitionId(competitionId, ct);
        if (!competitionsResult.IsSuccess)
            return Result<Division?>.FailedWith(competitionsResult.Error!);

        var suitableDivision = competitionsResult.Item!.FirstOrDefault(d => d.IsCompetitorFits(competitor));
        return Result<Division?>.Success(suitableDivision);
    }

    public async Task<Result<Division>> ConstructSuitableDivision(Guid competitionId, Competitor competitor,
        CancellationToken ct = default)
    {
        var defaultDivisions = divisionsFactory.CreateDefaultDivisions(competitionId);
        var suitableDivision = defaultDivisions.SingleOrDefault(d => d.IsCompetitorFits(competitor))
            ?? throw new NullReferenceException($"Can't find division for competitor {competitor.FirstName} {competitor.LastName} {competitor.MiddleName}");

        return Result<Division>.Success(suitableDivision);
    }
}