using System.Collections.ObjectModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TournamentBracket.Application.Brackets.Interfaces;
using TournamentBracket.Application.Common.Authorization.Interfaces;
using TournamentBracket.Application.Common.Helpers;
using TournamentBracket.Application.Competitions.Commands;
using TournamentBracket.Application.Competitions.DTO;
using TournamentBracket.Application.Competitions.Interfaces;
using TournamentBracket.Application.Competitions.Queries;
using TournamentBracket.Application.Competitions.Responses;
using TournamentBracket.Application.Competitors.Interfaces;
using TournamentBracket.Application.Divisions.Interfaces;
using TournamentBracket.Application.Matches.Commands;
using TournamentBracket.Application.Matches.Interface;
using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Brackets.RoundRobinBracket;
using TournamentBracket.Domain.Competitions;
using TournamentBracket.Domain.Matches;
using TournamentBracket.Domain.Users;
using TournamentBracket.Infrastructure.Common;
using TournamentBracket.Infrastructure.Common.Results;

namespace TournamentBracket.Application.Competitions.Services;

public class CompetitionsService : ICompetitionsService
{
	private readonly AppDbContext dbContext;
	private readonly IResourceAuthorizationService authorizationService;
	private readonly ICompetitorService competitorService;
	private readonly IDivisionsService divisionsService;
	private readonly ITournamentBracketsService tournamentBracketsService;
	private readonly IMatchesService matchesService;
	private readonly ICompetitionPlanner competitionPlanner;
	private readonly IHttpContextAccessor httpContextAccessor;


	public CompetitionsService(AppDbContext dbContext,
		IResourceAuthorizationService authorizationService,
		ICompetitorService competitorService,
		IDivisionsService divisionsService,
		ITournamentBracketsService tournamentBracketsService,
		IMatchesService matchesService,
		ICompetitionPlanner competitionPlanner,
		IHttpContextAccessor httpContextAccessor)
	{
		this.dbContext = dbContext;
		this.authorizationService = authorizationService;
		this.competitorService = competitorService;
		this.divisionsService = divisionsService;
		this.tournamentBracketsService = tournamentBracketsService;
		this.matchesService = matchesService;
		this.competitionPlanner = competitionPlanner;
		this.httpContextAccessor = httpContextAccessor;
	}

	public async Task<Result<CreateCompetitionResponse>> CreateCompetition(
		CreateCompetitionCommand command,
		string userEmail,
		CancellationToken ct = default)
	{
		var competition = new Competition
		{
			Name = command.Name,
			Location = command.Location,
			StartDateTime = command.StartDateTime,
			TatamiCount = command.TatamiCount,
			Status = CompetitionStatus.Planned,
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow,
			OwnerEmail = userEmail
		};

		dbContext.Competitions.Add(competition);
		await dbContext.SaveChangesAsync(ct);
		return Result<CreateCompetitionResponse>.Success(new CreateCompetitionResponse(competition.Id));
	}

	public async Task<Result<IReadOnlyCollection<Competition>>> GetCompetitions(CompetitionsPageQuery query,
		CancellationToken ct = default)
	{
		var userRole = httpContextAccessor.HttpContext?.User.Claims
			.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
		var userEmail = httpContextAccessor.HttpContext?.User.Claims
			.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

		var competitionsQueryable =
			ApplyFilter(dbContext.Competitions.AsQueryable(), query.CreateFilter(), userRole, userEmail);
		var competitions = await competitionsQueryable
			.SelectPage(query)
			.Include(c => c.Divisions)
			.ToListAsync(ct);
		return Result<IReadOnlyCollection<Competition>>.Success(competitions);
	}

	public async Task<Result<IReadOnlyCollection<Match>>> GetMatches(Guid competitionId, MatchesQuery query, CancellationToken ct = default)
	{
		var divisionsResult = await divisionsService.GetDivisionsByCompetitionId(competitionId, ct);
		if (!divisionsResult.IsSuccess)
			return Result<IReadOnlyCollection<Match>>.FailedWith(divisionsResult.Error!);

		var filteredDivisions = divisionsResult.Item!.AsEnumerable();
		if (query.Tatami.HasValue)
			filteredDivisions = filteredDivisions.Where(d => d.Tatami == query.Tatami);

		var bracketsResult = await tournamentBracketsService.GetBracketsByIds(filteredDivisions.Select(d => d.TournamentBracketId).ToList(), ct);
		if (!bracketsResult.IsSuccess)
			return Result<IReadOnlyCollection<Match>>.FailedWith(bracketsResult.Error!);
		var brackets = bracketsResult.Item!;
		var matches = new ReadOnlyCollection<Match>(brackets.SelectMany(b => b.GetAllMatches()).ToList());

		return Result<IReadOnlyCollection<Match>>.Success(matches);
	}

	public async Task<Result<int>> GetCount(CompetitionsFilter filter, CancellationToken ct = default)
	{
		var userRole = httpContextAccessor.HttpContext?.User.Claims
			.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
		var userEmail = httpContextAccessor.HttpContext?.User.Claims
			.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

		var competitionsCount = await ApplyFilter(dbContext.Competitions.AsQueryable(), filter, userRole, userEmail)
			.CountAsync(ct);
		return Result<int>.Success(competitionsCount);
	}

	public async Task<Result<Competition>> GetCompetitionWithoutCompetitors(Guid id, CancellationToken ct = default)
	{
		var competition = await dbContext.Competitions
			.Include(c => c.Divisions)
			.IgnoreAutoIncludes()
			.FirstOrDefaultAsync(e => e.Id == id, cancellationToken: ct);
		return competition == null
			? Result<Competition>.FailedWith(new Error("Not found", 404))
			: Result<Competition>.Success(competition);
	}

	public async Task<Result<Competition>> GetCompetitionFull(Guid id, CancellationToken ct = default)
	{
		var competition = await dbContext.Competitions
			.FirstOrDefaultAsync(e => e.Id == id, cancellationToken: ct);
		return competition == null
			? Result<Competition>.FailedWith(new Error("Not found", 404))
			: Result<Competition>.Success(competition);
	}

	public async Task<Result> UpdateCompetition(UpdateCompetitionCommand command, CancellationToken ct = default)
	{
		var competitionResult = await GetCompetitionWithoutCompetitors(command.Id, ct);
		if (!competitionResult.IsSuccess)
			return competitionResult;
		var competition = competitionResult.Item!;

		var authorizeResult = await authorizationService.Authorize(competition);
		if (!authorizeResult.IsSuccess)
			return authorizeResult;

		var oldTatamiCount = competition.TatamiCount;
		var oldStartDateTime = competition.StartDateTime;

		competition.Name = command.Name;
		competition.Location = command.Location;
		competition.TatamiCount = command.TatamiCount;
		competition.StartDateTime = command.StartDateTime;
		competition.Status = command.Status;
		competition.UpdatedAt = DateTime.UtcNow;

		if (oldTatamiCount != competition.TatamiCount || oldStartDateTime != competition.StartDateTime)
		{
			var replanResult = await competitionPlanner.PlanMatches(competition, ct);
			if (!replanResult.IsSuccess)
				return replanResult;
		}

		await dbContext.SaveChangesAsync(ct);
		return Result.Success();
	}

	public async Task<Result> DeleteCompetition(Guid id, CancellationToken ct = default)
	{
		var competitionResult = await GetCompetitionWithoutCompetitors(id, ct);
		if (!competitionResult.IsSuccess)
			return competitionResult;
		var competition = competitionResult.Item!;

		var authorizeResult = await authorizationService.Authorize(competition);
		if (!authorizeResult.IsSuccess)
			return authorizeResult;

		dbContext.Competitions.Remove(competition);
		var competitionsDeleted = await dbContext.SaveChangesAsync(ct);
		return competitionsDeleted > 0
			? Result.Success()
			: Result.Failed("Some error while deleting");
	}

	public async Task<Result> AddCompetitorAuto(Guid competitionId, AddCompetitorCommand command,
		CancellationToken ct = default)
	{
		await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
		try
		{
			var competitionResult = await GetCompetitionWithoutCompetitors(competitionId, ct);
			if (!competitionResult.IsSuccess)
				return competitionResult;
			var competition = competitionResult.Item!;

			var authorizeResult = await authorizationService.Authorize(competition);
			if (!authorizeResult.IsSuccess)
				return authorizeResult;

			if (competition.Status is CompetitionStatus.Started)
				return Result.Failed(new Error("Competition already started", 400));

			var competitorResult = await competitorService.GetCompetitor(command.CompetitorId, ct);
			if (!competitorResult.IsSuccess)
				return competitorResult;
			var competitor = competitorResult.Item!;

			var suitableDivisionResult = await divisionsService.GetSuitableDivision(competitionId, competitor, ct);
			if (!suitableDivisionResult.IsSuccess)
				return suitableDivisionResult;

			var suitableDivision = suitableDivisionResult.Item;
			if (suitableDivision is null)
			{
				var defaultSuitableDivisionResult =
					await divisionsService.ConstructSuitableDivision(competitionId, competitor, ct);
				if (!defaultSuitableDivisionResult.IsSuccess)
					return defaultSuitableDivisionResult;
				suitableDivision = defaultSuitableDivisionResult.Item!;
			}

			var successAddCompetitor = suitableDivision.TryAddCompetitor(competitor);
			if (!successAddCompetitor)
				return Result.Failed($"Some error while adding competitor {competitor.Id}");
			var successAddDivision = competition.TryAddDivision(suitableDivision);
			if (!successAddDivision)
				return Result.Failed($"Some error while adding division to competition {competitionId}");

			if (suitableDivision.TournamentBracketId == Guid.Empty)
			{
				var createBracketResult =
					await tournamentBracketsService.CreateBracket(suitableDivision.Competitors, ct);
				if (!createBracketResult.IsSuccess)
					return createBracketResult;

				var newBracket = createBracketResult.Item!;
				suitableDivision.TournamentBracketId = newBracket.Id;
				suitableDivision.BracketType = newBracket.Type;
			}
			else
			{
				var addCompetitorResult = await tournamentBracketsService.AddCompetitorToBracketAuto(
					suitableDivision.TournamentBracketId,
					suitableDivision.BracketType, competitor, ct);
				if (!addCompetitorResult.IsSuccess)
					return addCompetitorResult;

				var newBracket = addCompetitorResult.Item!;
				suitableDivision.TournamentBracketId = newBracket.Id;
				suitableDivision.BracketType = newBracket.Type;
			}

			//dbContext.Divisions.Add(suitableDivision);
			await transaction.CommitAsync(ct);

			var planResult = await competitionPlanner.PlanMatches(competition, ct);
			if (!planResult.IsSuccess)
				return planResult;

			await dbContext.SaveChangesAsync(ct);
			return Result.Success();
		}
		catch (Exception e)
		{
			await transaction.RollbackAsync(ct);
			return e.ToResult();
		}
	}

	public async Task<Result> RemoveCompetitor(Guid competitionId, RemoveCompetitorCommand command,
		CancellationToken ct = default)
	{
		await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
		try
		{
			var competitionResult = await GetCompetitionWithoutCompetitors(competitionId, ct);
			if (!competitionResult.IsSuccess)
				return competitionResult;
			var competition = competitionResult.Item!;

			var authorizeResult = await authorizationService.Authorize(competition);
			if (!authorizeResult.IsSuccess)
				return authorizeResult;

			if (competition.Status is CompetitionStatus.Started)
				return Result.Failed(new Error("Competition already started", 400));

			var competitorResult = await competitorService.GetCompetitor(command.CompetitorId, ct);
			if (!competitorResult.IsSuccess)
				return competitorResult;
			var competitor = competitorResult.Item!;

			var divisionsResult = await divisionsService.GetDivisionsByCompetitionId(competitionId, ct);
			if (!divisionsResult.IsSuccess)
				return divisionsResult;
			var divisions = divisionsResult.Item!;

			var division = divisions.FirstOrDefault(d => d.Competitors.Contains(competitor));
			if (division is null)
				return Result.Failed(
					$"Competition {competition.Id} does not contains division with competitor {competitor.Id}");

			var successRemoveFromDivision = division.TryRemoveCompetitor(competitor);
			if (!successRemoveFromDivision)
				return Result.Failed($"Some error while removing from division {competitor.Id}");

			var removeResult = await tournamentBracketsService.RemoveCompetitorFromBracketAuto(
				division.TournamentBracketId,
				division.BracketType, competitor, ct);
			if (!removeResult.IsSuccess)
				return removeResult;

			if (division.Competitors.Count == 0)
				dbContext.Divisions.Remove(division);
			else
			{
				var newBracket = removeResult.Item!;
				division.TournamentBracketId = newBracket.Id;
				division.BracketType = newBracket.Type;
			}

			await transaction.CommitAsync(ct);

			var planResult = await competitionPlanner.PlanMatches(competition, ct);
			if (!planResult.IsSuccess)
				return planResult;

			await dbContext.SaveChangesAsync(ct);
			return Result.Success();
		}
		catch (Exception e)
		{
			return e.ToResult();
		}
	}

	public async Task<Result> AddMatchEvent(Guid competitionId, UpdateMatchCommand command,
		CancellationToken ct = default)
	{
		var competitionResult = await GetCompetitionWithoutCompetitors(competitionId, ct);
		if (!competitionResult.IsSuccess)
			return competitionResult;
		var competition = competitionResult.Item!;

		if (competition.Divisions?.All(d => d.TournamentBracketId != command.BracketId) ?? true)
			return Result.Failed($"Can't find bracket {command.BracketId} in competition {competition.Id}");

		var authorizeResult = await authorizationService.Authorize(competition);
		if (!authorizeResult.IsSuccess)
			return authorizeResult;

		competition.Status = CompetitionStatus.Started;

		return await matchesService.AddMatchEvent(command, ct);
	}

	public async Task<Result> SetWinnerForRoundRobin(Guid competitionId, SetWinnerCommand command, CancellationToken ct = default)
	{
		var competitionResult = await GetCompetitionWithoutCompetitors(competitionId, ct);
		if (!competitionResult.IsSuccess)
			return competitionResult;
		var competition = competitionResult.Item!;

		if (competition.Divisions?.All(d => d.TournamentBracketId != command.BracketId) ?? true)
			return Result.Failed($"Can't find bracket {command.BracketId} in competition {competition.Id}");

		var authorizeResult = await authorizationService.Authorize(competition);
		if (!authorizeResult.IsSuccess)
			return authorizeResult;

		var bracketResult = await tournamentBracketsService.GetBracket(command.BracketId, BracketType.RoundRobin, ct);
		if (!bracketResult.IsSuccess)
			return Result.Failed(bracketResult.Error!);
		var bracket = bracketResult.Item!;

		var competitorResult = await competitorService.GetCompetitor(command.CompetitorId, ct);
		if (!competitorResult.IsSuccess)
			return Result.Failed(competitorResult.Error!);

		var successSetWinner = (bracket as RoundRobinBracket)!.SetWinner(competitorResult.Item!);
		if (!successSetWinner)
			return Result.Failed($"Something goes wrong when set winner in {bracket.Id}");

		await dbContext.SaveChangesAsync(ct);
		return Result.Success();
	}

	private IQueryable<Competition> ApplyFilter(IQueryable<Competition> queryable, CompetitionsFilter filter,
		string? userRole, string? userEmail)
	{
		if (!string.IsNullOrWhiteSpace(filter.Name))
			queryable = queryable.Where(c => c.Name.Contains(filter.Name));

		if (!string.IsNullOrWhiteSpace(userEmail) && userRole != nameof(UserRole.Administrator))
			queryable = queryable.Where(c => c.OwnerEmail == userEmail);

		return queryable;
	}
}