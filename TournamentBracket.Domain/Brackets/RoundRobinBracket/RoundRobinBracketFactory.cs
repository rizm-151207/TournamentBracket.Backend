using System.Collections.ObjectModel;
using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Brackets.RoundRobinBracket;

public class RoundRobinBracketFactory : IBracketFactory
{
	private readonly MatchFactory matchFactory;

	public RoundRobinBracketFactory(MatchFactory matchFactory)
	{
		this.matchFactory = matchFactory;
	}

	public Bracket CreateBracket(IList<Competitor> competitors)
	{
		var competitorsCount = competitors.Count;
		if (competitorsCount == 0)
			throw new ArgumentException("Empty competitor list");

		if (competitorsCount > 3)
			throw new ArgumentException($"Competitors count must be less than 4. Current count: {competitors.Count}");

		List<Match> matches;
		if (competitorsCount == 1 || competitorsCount == 2)
		{
			var match = competitorsCount == 1
				? matchFactory.CreateMatchByCompetitors(competitors[0], null)
				: matchFactory.CreateMatchByCompetitors(competitors[0], competitors[1]);

			matches = [match];
		}
		else
		{
			matches = new List<Match> { matchFactory.CreateMatchByCompetitors(competitors[0], competitors[1]),
		 								matchFactory.CreateMatchByCompetitors(competitors[1], competitors[2]),
										matchFactory.CreateMatchByCompetitors(competitors[2], competitors[0])};
		}

		return new RoundRobinBracket
		{
			Matches = matches,
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow
		};
	}

	public void ExtendBracket(Bracket bracket)
	{
		var rrBracket = GetRRBracket(bracket);

		var matches = rrBracket.GetAllMatches();
		if (matches.Count != 1)
			throw new InvalidOperationException(
				$"Can extend round robin bracket only with 1 match. Current matches count: {matches.Count}");

		var match = matches.First();
		if (match.IsByeMatch)
			throw new InvalidOperationException($"Can't extend round robin bracket. There is free match");

		matches = new List<Match> { match,
		 								matchFactory.CreateMatchByCompetitors(match.FirstCompetitor, null),
										matchFactory.CreateMatchByCompetitors(match.SecondCompetitor, null)};

		rrBracket.Matches = new ReadOnlyCollection<Match>((matches as IList<Match>)!);
		rrBracket.UpdatedAt = DateTime.UtcNow;
	}

	public bool NeedToReduce(Bracket bracket)
	{
		var rrBracket = GetRRBracket(bracket);
		var matches = rrBracket.GetAllMatches();

		return matches.Count == 3 && matches.Any(m => m.IsByeMatch);
	}

	public void RebalanceBracket(Bracket bracket)
	{
	}

	public void ReduceBracket(Bracket bracket)
	{
		var rrBracket = GetRRBracket(bracket);
		var matches = rrBracket.GetAllMatches();
		if (matches.Count != 3)
			throw new InvalidOperationException($"Can reduce bracket only with 3 matches. Current mathces is: {matches.Count}");

		var matchesToRemove = matches
			.Where(m => m.IsByeMatch)
			.ToList();

		rrBracket.Matches = [.. rrBracket.Matches.Except(matchesToRemove)];
	}

	private RoundRobinBracket GetRRBracket(Bracket bracket)
	{
		return (bracket as RoundRobinBracket)
			?? throw new ArgumentException($"Bracket is not {typeof(RoundRobinBracket)}. Bracket is {bracket.GetType}");
	}
}
