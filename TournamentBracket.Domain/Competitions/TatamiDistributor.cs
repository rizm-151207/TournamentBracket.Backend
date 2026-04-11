using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Divisions;

namespace TournamentBracket.Domain.Competitions;

public class TatamiDistributor
{
	public bool DistributeTatamisByDivisions(Dictionary<Division, Bracket> divisionsWithBrackets, int tatamisCount)
	{
		if (tatamisCount == 1)
		{
			foreach (var d in divisionsWithBrackets.Keys)
				d.Tatami = 1;
			return true;
		}

		if (divisionsWithBrackets.Count >= tatamisCount)
		{
			var tatamiIndex = 1;
			foreach (var d in divisionsWithBrackets.Keys)
			{
				d.Tatami = tatamiIndex;
				tatamiIndex++;
			}
			return true;
		}

		var divisionsMatchesCount = divisionsWithBrackets.ToDictionary(
			kvp => kvp.Key,
			kvp => kvp.Value.GetAllMatches().Count(m => !m.IsByeMatch));

		var orderedDivisions = divisionsMatchesCount.Keys.OrderBy(d => (d.MinAge, !d.Gender));

		var matchesCountOnTatami = Enumerable.Range(0, tatamisCount).ToDictionary(_ => new List<Division>(), _ => 0);
		var tatamis = matchesCountOnTatami.Keys.ToList();

		foreach (var d in orderedDivisions)
		{
			var minMatchesTatami = tatamis.MinBy(t => matchesCountOnTatami[t])!;
			d.Tatami = tatamis.IndexOf(minMatchesTatami) + 1;
			matchesCountOnTatami[minMatchesTatami] += divisionsMatchesCount[d];
		}

		return true;
	}
}
