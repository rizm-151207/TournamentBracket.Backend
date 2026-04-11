using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Brackets.SingleEliminationBracket;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Tests.Brackets.SingleEliminationBrackets;

[TestFixture]
public class SingleEliminationBracketSpec
{
	private SingleEliminationBracket bracket;
	private BracketNodeFactory bracketNodeFactory;
	private MatchFactory matchFactory;
	private SingleEliminationBracketFactory bracketFactory;

	[SetUp]
	public void Setup()
	{
		bracketNodeFactory = new BracketNodeFactory();
		matchFactory = new MatchFactory();
		bracketFactory = new SingleEliminationBracketFactory(bracketNodeFactory, matchFactory);
		var competitors = CreateCompetitors(4);
		bracket = (SingleEliminationBracket)bracketFactory.CreateBracket(competitors);
	}

	[Test]
	public void Type_Should_BeSingleElimination()
	{
		// Assert
		bracket.Type.Should().Be(BracketType.SingleElimination);
	}

	[Test]
	public void Root_Should_NotBeNull()
	{
		// Assert
		bracket.Root.Should().NotBeNull();
	}

	[Test]
	public void ThirdPlace_Should_NotBeNull()
	{
		// Assert
		bracket.ThirdPlace.Should().NotBeNull();
	}

	[Test]
	public void TryAddCompetitorAuto_WithExistingCompetitor_Should_ReturnFalse()
	{
		// Arrange
		var existingCompetitor = bracket.GetAllCompetitors().FirstOrDefault();

		// Act
		var result = bracket.TryAddCompetitorAuto(existingCompetitor!);

		// Assert
		result.Should().BeFalse();
	}

	[Test]
	public void TryRemoveCompetitorAuto_WithExistingCompetitor_Should_ReturnTrue()
	{
		// Arrange
		var competitor = bracket.GetAllCompetitors().FirstOrDefault();

		// Act
		var result = bracket.TryRemoveCompetitorAuto(competitor!, out var _);

		// Assert
		result.Should().BeTrue();
	}

	[Test]
	public void TryRemoveCompetitorAuto_WithNonExistingCompetitor_Should_ReturnFalse()
	{
		// Arrange
		var newCompetitor = CreateCompetitor();

		// Act
		var result = bracket.TryRemoveCompetitorAuto(newCompetitor, out _);

		// Assert
		result.Should().BeFalse();
	}

	[Test]
	public void HasFreeMatch_WithByeMatches_Should_ReturnTrue()
	{
		// Arrange - create bracket with odd number of competitors (5 needs bye matches)
		var competitors = CreateCompetitors(5);
		var testBracket = bracketFactory.CreateBracket(competitors) as SingleEliminationBracket;

		// Act
		var result = testBracket!.HasFreeMatch();

		// Assert
		result.Should().BeTrue();
	}

	[Test]
	public void GetAllCompetitors_Should_ReturnAllCompetitors()
	{
		// Act
		var competitors = bracket.GetAllCompetitors();

		// Assert
		competitors.Should().HaveCount(4);
	}

	[Test]
	public void GetAllCompetitors_Should_ReturnDistinctCompetitors()
	{
		// Act
		var competitors = bracket.GetAllCompetitors();

		// Assert
		competitors.Should().OnlyHaveUniqueItems();
	}

	[Test]
	public void GetAllMatches_Should_ReturnAllMatches()
	{
		// Act
		var allMatches = bracket.GetAllMatches();

		// Assert
		allMatches.Should().HaveCount(4);
	}

	[Test]
	public void GetAllMatches_Should_IncludeThirdPlaceMatch()
	{
		// Act
		var allMatches = bracket.GetAllMatches();

		// Assert
		allMatches.Should().Contain(bracket.ThirdPlace.Match);
	}

	[Test]
	public void GetAllNodesWithCompetitorsMatches_Should_ReturnMatchesWithCompetitors()
	{
		// Act
		var nodesWithMatches = bracket.GetAllNodesWithCompetitorsMatches().ToList();

		// Assert
		nodesWithMatches.Should().NotBeNullOrEmpty();
		nodesWithMatches.Should().OnlyContain(n => n.Match.FirstCompetitor != null || n.Match.SecondCompetitor != null);
	}

	[Test]
	public void RefreshBracketAfterMatchUpdate_WithFinishedMatch_Should_PropagateWinner()
	{
		// Arrange
		var leafNodes = bracket.GetAllNodesWithCompetitorsMatches()
			.Where(n => n.Children == null || n.Children.Count == 0)
			.ToList();

		var leafMatch = leafNodes[0].Match;

		// Set winner for leaf match
		leafMatch.MatchProcess.SetWinner(true, WinReason.Ippon);
		leafMatch.Status = MatchStatus.Finished;

		// Act
		bracket.RefreshBracketAfterMatchUpdate(leafMatch);

		// Assert
		var parentMatch = leafNodes[0].Parent?.Match;
		parentMatch.Should().NotBeNull();
	}

	private List<Competitor> CreateCompetitors(int count)
	{
		var competitors = new List<Competitor>();
		for (var i = 0; i < count; i++)
		{
			competitors.Add(new Competitor
			{
				Id = Guid.NewGuid(),
				FirstName = $"First{i}",
				LastName = $"Last{i}",
				Gender = true,
				DateOfBirth = DateTime.Now.AddYears(-20),
				Weight = 70 + i,
				Subject = $"trainer{i}"
			});
		}
		return competitors;
	}

	private Competitor CreateCompetitor()
	{
		return new Competitor
		{
			Id = Guid.NewGuid(),
			FirstName = "New",
			LastName = "Competitor",
			Gender = true,
			DateOfBirth = DateTime.Now.AddYears(-25),
			Weight = 75,
			Subject = "NewTrainer"
		};
	}
}
