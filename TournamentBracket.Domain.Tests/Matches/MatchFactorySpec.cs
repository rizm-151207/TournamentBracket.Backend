using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Tests.Matches;

[TestFixture]
public class MatchFactorySpec
{
	private MatchFactory matchFactory;

	[SetUp]
	public void Setup()
	{
		matchFactory = new MatchFactory();
	}

	[Test]
	public void CreateLeafMatch_WithTwoCompetitors_Should_CreateValidMatch()
	{
		// Arrange
		var competitor1 = CreateCompetitor();
		var competitor2 = CreateCompetitor();

		// Act
		var match = matchFactory.CreateMatchByCompetitors(competitor1, competitor2);

		// Assert
		match.Should().NotBeNull();
		match.FirstCompetitor.Should().Be(competitor1);
		match.SecondCompetitor.Should().Be(competitor2);
		match.Status.Should().Be(MatchStatus.Unplanned);
		match.MatchProcess.Should().NotBeNull();
	}

	[Test]
	public void CreateLeafMatch_WithOneCompetitor_Should_CreateByeMatch()
	{
		// Arrange
		var competitor1 = CreateCompetitor();

		// Act
		var match = matchFactory.CreateMatchByCompetitors(competitor1, null);

		// Assert
		match.Should().NotBeNull();
		match.FirstCompetitor.Should().Be(competitor1);
		match.Status.Should().Be(MatchStatus.Finished);
		match.IsByeMatch.Should().BeTrue();
		match.MatchProcess.Winner.Should().Be(MatchWinner.FirstCompetitor);
		match.MatchProcess.WinReason.Should().Be(WinReason.Bye);
	}

	[Test]
	public void CreateLeafMatch_WithSecondCompetitorOnly_Should_CreateByeMatch()
	{
		// Arrange
		var competitor2 = CreateCompetitor();

		// Act
		var match = matchFactory.CreateMatchByCompetitors(null, competitor2);

		// Assert
		match.Should().NotBeNull();
		match.FirstCompetitor.Should().Be(competitor2);
		match.Status.Should().Be(MatchStatus.Finished);
		match.IsByeMatch.Should().BeTrue();
	}

	[Test]
	public void CreateLeafMatch_WithNoCompetitors_Should_CreateEmptyUnplannedMatch()
	{
		// Act
		var match = matchFactory.CreateMatchByCompetitors(null, null);

		// Assert
		match.Should().NotBeNull();
		match.FirstCompetitor.Should().BeNull();
		match.SecondCompetitor.Should().BeNull();
		match.Status.Should().Be(MatchStatus.Unplanned);
		match.MatchProcess.Should().NotBeNull();
	}

	[Test]
	public void CreateEmptyUnplannedMatch_Should_CreateValidEmptyMatch()
	{
		// Act
		var match = matchFactory.CreateEmptyUnplannedMatch();

		// Assert
		match.Should().NotBeNull();
		match.FirstCompetitor.Should().BeNull();
		match.SecondCompetitor.Should().BeNull();
		match.Status.Should().Be(MatchStatus.Unplanned);
		match.MatchProcess.Should().NotBeNull();
		match.PlannedDateTime.Should().BeNull();
	}

	[Test]
	public void CreateEmptyUnplannedMatch_Should_SetCreatedAtAndUpdatedAt()
	{
		// Arrange
		var beforeCreate = DateTime.UtcNow;

		// Act
		var match = matchFactory.CreateEmptyUnplannedMatch();

		// Assert
		match.CreatedAt.Should().BeOnOrAfter(beforeCreate);
		match.UpdatedAt.Should().BeOnOrAfter(beforeCreate);
	}

	[Test]
	public void CreateLeafMatch_WithTwoCompetitors_Should_SetCreatedAtAndUpdatedAt()
	{
		// Arrange
		var competitor1 = CreateCompetitor();
		var competitor2 = CreateCompetitor();
		var beforeCreate = DateTime.UtcNow;

		// Act
		var match = matchFactory.CreateMatchByCompetitors(competitor1, competitor2);

		// Assert
		match.CreatedAt.Should().BeOnOrAfter(beforeCreate);
		match.UpdatedAt.Should().BeOnOrAfter(beforeCreate);
	}

	[Test]
	public void CreateByeMatch_Should_SetCorrectWinner()
	{
		// Arrange
		var competitor = CreateCompetitor();

		// Act
		var match = matchFactory.CreateMatchByCompetitors(competitor, null);

		// Assert
		match.FirstCompetitor.Should().Be(competitor);
		match.MatchProcess.Winner.Should().Be(MatchWinner.FirstCompetitor);
	}

	[Test]
	public void CreateLeafMatch_WithBothCompetitorsSame_Should_CreateValidMatch()
	{
		// Arrange
		var competitor = CreateCompetitor();

		// Act
		var match = matchFactory.CreateMatchByCompetitors(competitor, competitor);

		// Assert
		match.Should().NotBeNull();
		match.FirstCompetitor.Should().Be(competitor);
		match.SecondCompetitor.Should().Be(competitor);
		match.Status.Should().Be(MatchStatus.Unplanned);
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
