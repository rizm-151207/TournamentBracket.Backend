using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Brackets.RoundRobinBracket;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Tests.Brackets.RoundRobinBrackets;

[TestFixture]
public class RoundRobinBracketSpec
{
	private MatchFactory matchFactory;

	[SetUp]
	public void Setup()
	{
		matchFactory = new MatchFactory();
	}

	[Test]
	public void Type_Should_ReturnRoundRobin()
	{
		// Arrange
		var competitor = CreateCompetitor();
		var match = matchFactory.CreateMatchByCompetitors(competitor, null);
		var bracket = new RoundRobinBracket { Matches = new List<Match> { match } };

		// Act
		var result = bracket.Type;

		// Assert
		result.Should().Be(BracketType.RoundRobin);
	}

	[Test]
	public void GetAllCompetitors_WithSingleMatch_Should_ReturnCompetitors()
	{
		// Arrange
		var competitor1 = CreateCompetitor();
		var competitor2 = CreateCompetitor();
		var match = matchFactory.CreateMatchByCompetitors(competitor1, competitor2);
		var bracket = new RoundRobinBracket { Matches = new List<Match> { match } };

		// Act
		var result = bracket.GetAllCompetitors();

		// Assert
		result.Should().HaveCount(2);
		result.Should().Contain(competitor1);
		result.Should().Contain(competitor2);
	}

	[Test]
	public void GetAllCompetitors_WithMultipleMatches_Should_ReturnDistinctCompetitors()
	{
		// Arrange
		var competitor1 = CreateCompetitor();
		var competitor2 = CreateCompetitor();
		var competitor3 = CreateCompetitor();
		var matches = new List<Match>
		{
			matchFactory.CreateMatchByCompetitors(competitor1, competitor2),
			matchFactory.CreateMatchByCompetitors(competitor2, competitor3),
			matchFactory.CreateMatchByCompetitors(competitor3, competitor1)
		};
		var bracket = new RoundRobinBracket { Matches = matches };

		// Act
		var result = bracket.GetAllCompetitors();

		// Assert
		result.Should().HaveCount(3);
	}

	[Test]
	public void GetAllMatches_Should_ReturnAllMatches()
	{
		// Arrange
		var competitor1 = CreateCompetitor();
		var competitor2 = CreateCompetitor();
		var matches = new List<Match>
		{
			matchFactory.CreateMatchByCompetitors(competitor1, competitor2)
		};
		var bracket = new RoundRobinBracket { Matches = matches };

		// Act
		var result = bracket.GetAllMatches();

		// Assert
		result.Should().HaveCount(1);
		result.Should().Contain(matches[0]);
	}

	[Test]
	public void GetGroupedMatchesByRounds_WithOneMatch_Should_ReturnSingleRound()
	{
		// Arrange
		var competitor = CreateCompetitor();
		var match = matchFactory.CreateMatchByCompetitors(competitor, null);
		var bracket = new RoundRobinBracket { Matches = new List<Match> { match } };

		// Act
		var result = bracket.GetGroupedMatchesByRounds();

		// Assert
		result.Should().HaveCount(1);
		result.Should().ContainKey(0);
		result[0].Should().HaveCount(1);
	}

	[Test]
	public void GetGroupedMatchesByRounds_WithThreeMatches_Should_ReturnThreeRounds()
	{
		// Arrange
		var competitor1 = CreateCompetitor();
		var competitor2 = CreateCompetitor();
		var competitor3 = CreateCompetitor();
		var matches = new List<Match>
		{
			matchFactory.CreateMatchByCompetitors(competitor1, competitor2),
			matchFactory.CreateMatchByCompetitors(competitor2, competitor3),
			matchFactory.CreateMatchByCompetitors(competitor3, competitor1)
		};
		var bracket = new RoundRobinBracket { Matches = matches };

		// Act
		var result = bracket.GetGroupedMatchesByRounds();

		// Assert
		result.Should().HaveCount(3);
		result.Should().ContainKey(0);
		result.Should().ContainKey(1);
		result.Should().ContainKey(2);
	}

	[Test]
	public void GetGroupedMatchesByRounds_WithInvalidMatchCount_Should_ThrowArgumentException()
	{
		// Arrange
		var competitor1 = CreateCompetitor();
		var competitor2 = CreateCompetitor();
		var competitor3 = CreateCompetitor();
		var competitor4 = CreateCompetitor();
		var matches = new List<Match>
		{
			matchFactory.CreateMatchByCompetitors(competitor1, competitor2),
			matchFactory.CreateMatchByCompetitors(competitor3, competitor4)
		};
		var bracket = new RoundRobinBracket { Matches = matches };

		// Act
		Action act = () => bracket.GetGroupedMatchesByRounds();

		// Assert
		act.Should().Throw<ArgumentException>()
			.WithMessage("Can't be 2 in round robin bracket");
	}

	[Test]
	public void HasFreeMatch_WithByeMatch_Should_ReturnTrue()
	{
		// Arrange
		var competitor = CreateCompetitor();
		var match = matchFactory.CreateMatchByCompetitors(competitor, null);
		var bracket = new RoundRobinBracket { Matches = new List<Match> { match } };

		// Act
		var result = bracket.HasFreeMatch();

		// Assert
		result.Should().BeTrue();
	}

	[Test]
	public void HasFreeMatch_WithoutByeMatch_Should_ReturnFalse()
	{
		// Arrange
		var competitor1 = CreateCompetitor();
		var competitor2 = CreateCompetitor();
		var match = matchFactory.CreateMatchByCompetitors(competitor1, competitor2);
		var bracket = new RoundRobinBracket { Matches = new List<Match> { match } };

		// Act
		var result = bracket.HasFreeMatch();

		// Assert
		result.Should().BeFalse();
	}

	[Test]
	public void RefreshBracketAfterMatchUpdate_Should_NotThrowException()
	{
		// Arrange
		var competitor1 = CreateCompetitor();
		var competitor2 = CreateCompetitor();
		var match = matchFactory.CreateMatchByCompetitors(competitor1, competitor2);
		var bracket = new RoundRobinBracket { Matches = new List<Match> { match } };

		// Act & Assert
		Action act = () => bracket.RefreshBracketAfterMatchUpdate(match);
		act.Should().NotThrow();
	}

	[Test]
	public void TryAddCompetitorAuto_WithEmptyMatches_Should_ReturnFalse()
	{
		// Arrange
		var bracket = new RoundRobinBracket { Matches = new List<Match>() };
		var competitor = CreateCompetitor();

		// Act
		var result = bracket.TryAddCompetitorAuto(competitor);

		// Assert
		result.Should().BeFalse();
	}

	[Test]
	public void TryAddCompetitorAuto_WithSingleByeMatch_Should_AddCompetitor()
	{
		// Arrange
		var competitor1 = CreateCompetitor();
		var competitor2 = CreateCompetitor();
		var match = matchFactory.CreateMatchByCompetitors(competitor1, null);
		var bracket = new RoundRobinBracket { Matches = new List<Match> { match } };

		// Act
		var result = bracket.TryAddCompetitorAuto(competitor2);

		// Assert
		result.Should().BeTrue();
		match.SecondCompetitor.Should().Be(competitor2);
	}

	[Test]
	public void TryAddCompetitorAuto_WithThreeMatchesAndByeMatches_Should_AddCompetitorToByeMatches()
	{
		// Arrange
		var competitor1 = CreateCompetitor();
		var competitor2 = CreateCompetitor();
		var competitor3 = CreateCompetitor();
		var newCompetitor = CreateCompetitor();
		var matches = new List<Match>
		{
			matchFactory.CreateMatchByCompetitors(competitor1, competitor2),
			matchFactory.CreateMatchByCompetitors(competitor2, null), // bye match
            matchFactory.CreateMatchByCompetitors(competitor3, competitor1)
		};
		var bracket = new RoundRobinBracket { Matches = matches };

		// Act
		var result = bracket.TryAddCompetitorAuto(newCompetitor);

		// Assert
		result.Should().BeTrue();
		var byeMatch = matches.Single(m => m.SecondCompetitor == null || m.SecondCompetitor == newCompetitor);
		byeMatch.FirstCompetitor.Should().Be(competitor2);
	}

	[Test]
	public void TryAddCompetitorAuto_WithThreeMatchesNoBye_Should_ReturnTrue()
	{
		// Arrange
		var competitor1 = CreateCompetitor();
		var competitor2 = CreateCompetitor();
		var competitor3 = CreateCompetitor();
		var matches = new List<Match>
		{
			matchFactory.CreateMatchByCompetitors(competitor1, competitor2),
			matchFactory.CreateMatchByCompetitors(competitor2, competitor3),
			matchFactory.CreateMatchByCompetitors(competitor3, competitor1)
		};
		var bracket = new RoundRobinBracket { Matches = matches };
		var newCompetitor = CreateCompetitor();

		// Act
		var result = bracket.TryAddCompetitorAuto(newCompetitor);

		// Assert - Returns true even though there are no bye matches to add to
		result.Should().BeTrue();
	}

	[Test]
	public void TryRemoveCompetitorAuto_WithCompetitorInMatch_Should_RemoveCompetitor()
	{
		// Arrange
		var competitor1 = CreateCompetitor();
		var competitor2 = CreateCompetitor();
		var match = matchFactory.CreateMatchByCompetitors(competitor1, competitor2);
		var bracket = new RoundRobinBracket { Matches = new List<Match> { match } };

		// Act
		var result = bracket.TryRemoveCompetitorAuto(competitor1, out var hasEmptyMatch);

		// Assert
		result.Should().BeTrue();
		hasEmptyMatch.Should().BeTrue();
		match.FirstCompetitor.Should().BeNull();
		match.SecondCompetitor.Should().Be(competitor2);
	}

	[Test]
	public void TryRemoveCompetitorAuto_WithNonExistentCompetitor_Should_ReturnFalse()
	{
		// Arrange
		var competitor1 = CreateCompetitor();
		var competitor2 = CreateCompetitor();
		var nonExistentCompetitor = CreateCompetitor();
		var match = matchFactory.CreateMatchByCompetitors(competitor1, competitor2);
		var bracket = new RoundRobinBracket { Matches = new List<Match> { match } };

		// Act
		var result = bracket.TryRemoveCompetitorAuto(nonExistentCompetitor, out var hasEmptyMatch);

		// Assert
		result.Should().BeFalse();
		hasEmptyMatch.Should().BeFalse();
	}

	private Competitor CreateCompetitor()
	{
		return new Competitor
		{
			Id = Guid.NewGuid(),
			FirstName = "Test",
			LastName = "Competitor",
			Gender = true,
			DateOfBirth = DateTime.Now.AddYears(-20),
			Weight = 70,
			Subject = "TestSubject"
		};
	}
}
