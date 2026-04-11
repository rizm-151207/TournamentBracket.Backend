using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Tests.Brackets;

[TestFixture]
public class BracketNodeSpec
{
	private BracketNode node;

	[SetUp]
	public void Setup()
	{
		node = new BracketNode
		{
			Id = Guid.NewGuid(),
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow
		};
	}

	[Test]
	public void SetParent_WhenParentIsNull_Should_SetParent()
	{
		// Arrange
		var parent = new BracketNode();

		// Act
		node.SetParent(parent);

		// Assert
		node.Parent.Should().Be(parent);
	}

	[Test]
	public void SetParent_WhenParentIsNotNull_Should_Throw_InvalidOperationException()
	{
		// Arrange
		var parent = new BracketNode();
		node.SetParent(parent);

		// Act
		Action act = () => node.SetParent(new BracketNode());

		// Assert
		act.Should().Throw<InvalidOperationException>()
			.WithMessage($"Parent for node {node.Id} already setted");
	}

	[Test]
	public void SetMatch_Should_SetMatchAndMatchId()
	{
		// Arrange
		var match = new Match
		{
			Id = Guid.NewGuid(),
			MatchProcess = new MatchProcess()
		};

		// Act
		node.SetMatch(match);

		// Assert
		node.Match.Should().Be(match);
		node.MatchId.Should().Be(match.Id);
	}

	[Test]
	public void GetAllChildren_WhenChildrenAreNull_Should_Return_EmptyEnumerable()
	{
		// Arrange
		node.Children = null;

		// Act
		var result = node.GetAllChildren();

		// Assert
		result.Should().BeEmpty();
	}

	[Test]
	public void GetAllChildren_WhenHasChildren_Should_Return_AllChildrenRecursively()
	{
		// Arrange
		var child1 = new BracketNode { Children = new List<BracketNode>() };
		var child2 = new BracketNode { Children = new List<BracketNode>() };
		var grandchild = new BracketNode { Children = new List<BracketNode>() };

		child1.Children.Add(grandchild);
		node.Children = new List<BracketNode> { child1, child2 };

		// Act
		var result = node.GetAllChildren().ToList();

		// Assert
		result.Should().ContainInOrder(new List<BracketNode> { child1, grandchild, child2 });
	}
}
