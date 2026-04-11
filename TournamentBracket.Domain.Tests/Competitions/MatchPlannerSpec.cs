using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Competitions;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Divisions;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Tests.Competitions;

[TestFixture]
public class MatchPlannerSpec
{
	private MatchPlanner matchPlanner = null!;
	private Competition competition = null!;
	private const int DefaultTatamiNum = 1;
	private const int MatchDurationMinutes = 3;

	[SetUp]
	public void Setup()
	{
		matchPlanner = new MatchPlanner();
		competition = new Competition
		{
			Id = Guid.NewGuid(),
			StartDateTime = DateTime.UtcNow.AddDays(30)
		};
	}

	[Test]
	public void PlanMatchesForDivisionsOnTatami_WithEmptyDivisionsList_Should_Succeed()
	{
		// Arrange
		var divisions = new List<Division>();
		var divisionsWithBrackets = new Dictionary<Division, Bracket>();

		// Act
		var result = matchPlanner.PlanMatchesForDivisionsOnTatami(
			competition, divisions, DefaultTatamiNum, divisionsWithBrackets);

		// Assert
		result.Should().BeTrue();
	}

	[Test]
	public void PlanMatchesForDivisionsOnTatami_Should_PlanMatchesWithSequentialIndexes()
	{
		// Arrange
		var division = CreateDivision();
		var bracket = CreateFakeBracketWithMatches(5);
		var divisions = new List<Division> { division };
		var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };

		// Act
		matchPlanner.PlanMatchesForDivisionsOnTatami(
			competition, divisions, DefaultTatamiNum, divisionsWithBrackets);

		// Assert
		var nonByeMatches = bracket.GetAllMatches().Where(m => !m.IsByeMatch).ToList();
		nonByeMatches.Should().NotBeEmpty();

		for (int i = 0; i < nonByeMatches.Count; i++)
		{
			nonByeMatches[i].Index.Should().Be($"Поединок {i + 1} (татами {DefaultTatamiNum})");
		}
	}

	[Test]
	public void PlanMatchesForDivisionsOnTatami_Should_SetPlannedDateTime()
	{
		// Arrange
		var division = CreateDivision();
		var bracket = CreateFakeBracketWithMatches(3);
		var divisions = new List<Division> { division };
		var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };
		var startDateTime = DateTime.UtcNow.AddDays(30);
		competition.StartDateTime = startDateTime;

		// Act
		matchPlanner.PlanMatchesForDivisionsOnTatami(
			competition, divisions, DefaultTatamiNum, divisionsWithBrackets);

		// Assert
		var nonByeMatches = bracket.GetAllMatches().Where(m => !m.IsByeMatch).ToList();
		foreach (var match in nonByeMatches)
		{
			match.PlannedDateTime.Should().NotBeNull();
			match.PlannedDateTime.Should().BeOnOrAfter(startDateTime);
		}
	}

	[Test]
	public void PlanMatchesForDivisionsOnTatami_Should_SetMatchStatusToPlanned()
	{
		// Arrange
		var division = CreateDivision();
		var bracket = CreateFakeBracketWithMatches(4);
		var divisions = new List<Division> { division };
		var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };

		// Act
		matchPlanner.PlanMatchesForDivisionsOnTatami(
			competition, divisions, DefaultTatamiNum, divisionsWithBrackets);

		// Assert
		var nonByeMatches = bracket.GetAllMatches().Where(m => !m.IsByeMatch).ToList();
		foreach (var match in nonByeMatches)
		{
			match.Status.Should().Be(MatchStatus.Planned);
		}
	}

	[Test]
	public void PlanMatchesForDivisionsOnTatami_ByeMatches_Should_RemainUnplanned()
	{
		// Arrange
		var division = CreateDivision();
		var bracket = CreateFakeBracketWithByeMatches(3);
		var divisions = new List<Division> { division };
		var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };

		// Act
		matchPlanner.PlanMatchesForDivisionsOnTatami(
			competition, divisions, DefaultTatamiNum, divisionsWithBrackets);

		// Assert
		var byeMatches = bracket.GetAllMatches().Where(m => m.IsByeMatch).ToList();
		byeMatches.Should().NotBeEmpty();
		foreach (var match in byeMatches)
		{
			match.PlannedDateTime.Should().BeNull();
			match.Index.Should().BeNull();
			match.Status.Should().NotBe(MatchStatus.Planned);
		}
	}

	[Test]
	public void PlanMatchesForDivisionsOnTatami_MultipleDivisions_Should_PlanAllMatches()
	{
		// Arrange
		var division1 = CreateDivision(minAge: 12, maxAge: 14);
		var division2 = CreateDivision(minAge: 15, maxAge: 17);
		var bracket1 = CreateFakeBracketWithMatches(3);
		var bracket2 = CreateFakeBracketWithMatches(3);
		var divisions = new List<Division> { division1, division2 };
		var divisionsWithBrackets = new Dictionary<Division, Bracket>
		{
			{ division1, bracket1 },
			{ division2, bracket2 }
		};

		// Act
		var result = matchPlanner.PlanMatchesForDivisionsOnTatami(
			competition, divisions, DefaultTatamiNum, divisionsWithBrackets);

		// Assert
		result.Should().BeTrue();
		var allMatches = bracket1.GetAllMatches()
			.Concat(bracket2.GetAllMatches())
			.Where(m => !m.IsByeMatch)
			.ToList();
		allMatches.Should().AllSatisfy(m =>
		{
			m.Status.Should().Be(MatchStatus.Planned);
			m.PlannedDateTime.Should().NotBeNull();
		});
	}

	[Test]
	public void PlanMatchesForDivisionsOnTatami_Should_IncrementDateTimeByMatchDuration()
	{
		// Arrange
		var division = CreateDivision();
		var bracket = CreateFakeBracketWithMatches(4);
		var divisions = new List<Division> { division };
		var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };
		var startDateTime = DateTime.UtcNow.AddDays(30);
		competition.StartDateTime = startDateTime;

		// Act
		matchPlanner.PlanMatchesForDivisionsOnTatami(
			competition, divisions, DefaultTatamiNum, divisionsWithBrackets);

		// Assert
		var nonByeMatches = bracket.GetAllMatches().Where(m => !m.IsByeMatch).ToList();
		nonByeMatches.Should().HaveCountGreaterOrEqualTo(2);

		for (int i = 1; i < nonByeMatches.Count; i++)
		{
			var previousMatch = nonByeMatches[i - 1];
			var currentMatch = nonByeMatches[i];
			var timeDifference = (currentMatch.PlannedDateTime!.Value - previousMatch.PlannedDateTime!.Value).TotalMinutes;
			timeDifference.Should().Be(MatchDurationMinutes);
		}
	}

	[Test]
	public void PlanMatchesForDivisionsOnTatami_WithNullDivisionsList_Should_ThrowArgumentNullException()
	{
		// Arrange
		var divisionsWithBrackets = new Dictionary<Division, Bracket>();

		// Act
		Action act = () => matchPlanner.PlanMatchesForDivisionsOnTatami(
			competition, null!, DefaultTatamiNum, divisionsWithBrackets);

		// Assert
		act.Should().Throw<ArgumentNullException>();
	}

	[Test]
	public void PlanMatchesForDivisionsOnTatami_Should_UseCorrectTatamiNumberInIndex()
	{
		// Arrange
		var division = CreateDivision();
		var bracket = CreateFakeBracketWithMatches(2);
		var divisions = new List<Division> { division };
		var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };
		int tatamiNum = 5;

		// Act
		matchPlanner.PlanMatchesForDivisionsOnTatami(
			competition, divisions, tatamiNum, divisionsWithBrackets);

		// Assert
		var nonByeMatches = bracket.GetAllMatches().Where(m => !m.IsByeMatch).ToList();
		foreach (var match in nonByeMatches)
		{
			match.Index.Should().Contain($"татами {tatamiNum}");
		}
	}

	[Test]
	public void PlanMatchesForDivisionsOnTatami_MixedWithByeAndRegularMatches_Should_HandleCorrectly()
	{
		// Arrange
		var division = CreateDivision();
		var bracket = CreateFakeBracketWithMixedMatches(4, 2); // 4 regular, 2 bye
		var divisions = new List<Division> { division };
		var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };

		// Act
		matchPlanner.PlanMatchesForDivisionsOnTatami(
			competition, divisions, DefaultTatamiNum, divisionsWithBrackets);

		// Assert
		var allMatches = bracket.GetAllMatches().ToList();
		var regularMatches = allMatches.Where(m => !m.IsByeMatch).ToList();
		var byeMatches = allMatches.Where(m => m.IsByeMatch).ToList();

		regularMatches.Should().AllSatisfy(m =>
		{
			m.Status.Should().Be(MatchStatus.Planned);
			m.PlannedDateTime.Should().NotBeNull();
			m.Index.Should().NotBeNull();
		});

		byeMatches.Should().AllSatisfy(m =>
		{
			m.PlannedDateTime.Should().BeNull();
			m.Index.Should().BeNull();
		});
	}

	[Test]
	public void PlanMatchesForDivisionsOnTatami_WithDifferentAgeDivisions_Should_SortByAge()
	{
		// Arrange
		var youngerDivision = CreateDivision(minAge: 12, maxAge: 14);
		var olderDivision = CreateDivision(minAge: 15, maxAge: 17);
		var youngerBracket = CreateFakeBracketWithMatches(2);
		var olderBracket = CreateFakeBracketWithMatches(2);
		var divisions = new List<Division> { youngerDivision, olderDivision };
		var divisionsWithBrackets = new Dictionary<Division, Bracket>
		{
			{ youngerDivision, youngerBracket },
			{ olderDivision, olderBracket }
		};

		// Act
		matchPlanner.PlanMatchesForDivisionsOnTatami(
			competition, divisions, DefaultTatamiNum, divisionsWithBrackets);

		// Assert
		var allPlannedMatches = divisionsWithBrackets
			.SelectMany(kvp => kvp.Value.GetAllMatches())
			.Where(m => !m.IsByeMatch)
			.OrderBy(m => m.PlannedDateTime)
			.ToList();

		allPlannedMatches.Should().HaveCount(4);
	}

	[Test]
	public void PlanMatchesForDivisionsOnTatami_FirstMatchShouldStartAtCompetitionStartDateTime()
	{
		// Arrange
		var division = CreateDivision();
		var bracket = CreateFakeBracketWithMatches(3);
		var divisions = new List<Division> { division };
		var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };
		var expectedStartDateTime = new DateTime(2026, 3, 15, 10, 0, 0, DateTimeKind.Utc);
		competition.StartDateTime = expectedStartDateTime;

		// Act
		matchPlanner.PlanMatchesForDivisionsOnTatami(
			competition, divisions, DefaultTatamiNum, divisionsWithBrackets);

		// Assert
		var firstMatch = bracket.GetAllMatches()
			.Where(m => !m.IsByeMatch)
			.OrderBy(m => m.PlannedDateTime)
			.First();

		firstMatch.PlannedDateTime.Should().Be(expectedStartDateTime);
	}

	[Test]
	public void PlanMatchesForDivisionsOnTatami_UnplannedMatches_Should_BecomePlanned()
	{
		// Arrange
		var division = CreateDivision();
		var bracket = CreateFakeBracketWithUnplannedMatches(3);
		var divisions = new List<Division> { division };
		var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };

		// Act
		matchPlanner.PlanMatchesForDivisionsOnTatami(
			competition, divisions, DefaultTatamiNum, divisionsWithBrackets);

		// Assert
		var matches = bracket.GetAllMatches().Where(m => !m.IsByeMatch).ToList();
		matches.Should().AllSatisfy(m =>
		{
			m.Status.Should().Be(MatchStatus.Planned);
			m.PlannedDateTime.Should().NotBeNull();
			m.Index.Should().NotBeNull();
		});
	}

	private Division CreateDivision(int? minAge = 12, int? maxAge = 18)
	{
		return new Division(
			competition.Id,
			gender: true,
			minAge: minAge,
			maxAge: maxAge);
	}

	private FakeBracket CreateFakeBracketWithMatches(int matchCount)
	{
		var matches = new List<Match>();
		for (var i = 0; i < matchCount; i++)
		{
			matches.Add(new Match
			{
				Id = Guid.NewGuid(),
				Status = MatchStatus.Unplanned,
				MatchProcess = new MatchProcess(),
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			});
		}
		return new FakeBracket(matches);
	}

	private FakeBracket CreateFakeBracketWithByeMatches(int matchCount)
	{
		var matches = new List<Match>();
		for (var i = 0; i < matchCount; i++)
		{
			var match = new Match
			{
				Id = Guid.NewGuid(),
				Status = MatchStatus.Finished,
				MatchProcess = new MatchProcess { Winner = MatchWinner.FirstCompetitor, WinReason = WinReason.Bye },
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};
			matches.Add(match);
		}
		return new FakeBracket(matches);
	}

	private FakeBracket CreateFakeBracketWithMixedMatches(int regularCount, int byeCount)
	{
		var matches = new List<Match>();

		for (var i = 0; i < regularCount; i++)
		{
			matches.Add(new Match
			{
				Id = Guid.NewGuid(),
				Status = MatchStatus.Unplanned,
				MatchProcess = new MatchProcess(),
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			});
		}

		for (var i = 0; i < byeCount; i++)
		{
			matches.Add(new Match
			{
				Id = Guid.NewGuid(),
				Status = MatchStatus.Finished,
				MatchProcess = new MatchProcess { Winner = MatchWinner.FirstCompetitor, WinReason = WinReason.Bye },
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			});
		}

		return new FakeBracket(matches);
	}

	private FakeBracket CreateFakeBracketWithUnplannedMatches(int matchCount)
	{
		var matches = new List<Match>();
		for (var i = 0; i < matchCount; i++)
		{
			matches.Add(new Match
			{
				Id = Guid.NewGuid(),
				Status = MatchStatus.Unplanned,
				MatchProcess = new MatchProcess(),
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			});
		}
		return new FakeBracket(matches);
	}

	private class FakeBracket : Bracket
	{
		private readonly List<Match> _matches;

		public override BracketType Type => BracketType.SingleElimination;

		public FakeBracket(List<Match> matches)
		{
			_matches = matches;
		}

		public override bool TryAddCompetitorAuto(Competitor competitor) => false;

		public override bool TryRemoveCompetitorAuto(Competitor competitor, out bool hasEmptyMatch)
		{
			hasEmptyMatch = false;
			return false;
		}

		public override bool HasFreeMatch() => false;

		public override List<Competitor> GetAllCompetitors() => new();

		public override IReadOnlyCollection<Match> GetAllMatches() => _matches;

		public override Dictionary<int, IReadOnlyCollection<Match>> GetGroupedMatchesByRounds()
		{
			return new Dictionary<int, IReadOnlyCollection<Match>>
			{
				{ 1, _matches }
			};
		}

		public override void RefreshBracketAfterMatchUpdate(Match match) { }
	}
}
