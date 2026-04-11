using System.Collections.ObjectModel;
using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Brackets.RoundRobinBracket;

public class RoundRobinBracket : Bracket
{
	public override BracketType Type => BracketType.RoundRobin;
	private IReadOnlyList<Match> matches;
	public IReadOnlyList<Match> Matches
	{
		get => matches;
		set
		{
			matches = value;
		}
	}

	public override List<Competitor> GetAllCompetitors()
	{
		return Matches.SelectMany(m => new List<Competitor?> { m.FirstCompetitor, m.SecondCompetitor })
			.Where(c => c is not null)
			.Distinct()
			.ToList() as List<Competitor>;
	}

	public override IReadOnlyCollection<Match> GetAllMatches()
	{
		return Matches;
	}

	public override Dictionary<int, IReadOnlyCollection<Match>> GetGroupedMatchesByRounds()
	{
		return Matches.Count switch
		{
			1 => new Dictionary<int, IReadOnlyCollection<Match>>() { { 0, new ReadOnlyCollection<Match>([.. Matches]) } },
			3 => new Dictionary<int, IReadOnlyCollection<Match>>() { { 0, new ReadOnlyCollection<Match>([Matches[0]]) },
																	 { 1, new ReadOnlyCollection<Match>([Matches[1]]) },
																	 { 2, new ReadOnlyCollection<Match>([Matches[2]]) } },
			_ => throw new ArgumentException($"Can't be {Matches.Count} in round robin bracket")
		};
	}

	public override bool HasFreeMatch()
	{
		return Matches.Any(m => m.IsByeMatch);
	}

	public override void RefreshBracketAfterMatchUpdate(Match match) { }

	public override bool TryAddCompetitorAuto(Competitor competitor)
	{
		if (Matches.Count == 0)
			return false;

		if (Matches.Count == 1 && Matches[0].IsByeMatch)// Добавлен уже 1 участник
		{
			Matches[0].AddCompetitor(competitor);
			return true;
		}

		// Добавлено уже 2 участника
		if (Matches.Count != 3)
			return false;

		foreach(var byeMatch in Matches.Where(m => m.IsByeMatch))
		{
			byeMatch.AddCompetitor(competitor);
		}


		return true;
	}

	public override bool TryRemoveCompetitorAuto(Competitor competitor, out bool hasEmptyMatch)
	{
		var matchesWithCompetitor = Matches
			.Where(m => m.FirstCompetitor == competitor || m.SecondCompetitor == competitor)
			.ToList();

		hasEmptyMatch = false;

		if (matchesWithCompetitor.Count == 0)
			return false;

		if (matchesWithCompetitor.Count != 1 && matchesWithCompetitor.Count != 2)
			throw new Exception($"Expected 1 or 2 mathces with competitor, but got {matchesWithCompetitor.Count}");

		if (matchesWithCompetitor.Count == 1)
			matchesWithCompetitor.First().RemoveCompetitor(competitor);

		hasEmptyMatch = true;
		return true;
	}
}
