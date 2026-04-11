namespace TournamentBracket.Domain.Brackets.Helpers;

public class BracketsHelpers
{
	public static int[] GetSeed(int competitorsCount)
	{
		if (!IsPowerOfTwo(competitorsCount))
			competitorsCount = NearestPowerOfTwo(competitorsCount);

		if (competitorsCount == 1)
			return [1];
		var halfSeed = GetSeed(competitorsCount / 2);

		var result = new List<int>(competitorsCount);
		foreach (var x in halfSeed)
		{
			result.Add(x);
			result.Add(competitorsCount - x + 1);
		}

		return result.ToArray();
	}

	public static int NearestPowerOfTwo(int value)
	{
		while (!IsPowerOfTwo(value))
			value++;
		return value;
	}

	private static bool IsPowerOfTwo(int x)
	{
		return x > 0 && (x & (x - 1)) == 0;
	}
}