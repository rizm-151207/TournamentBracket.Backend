using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Tests.Brackets;

[TestFixture]
public class BracketNodeFactorySpec
{
    private BracketNodeFactory bracketNodeFactory;

    [SetUp]
    public void Setup()
    {
        bracketNodeFactory = new BracketNodeFactory();
    }

    [Test]
    public void Create_WithMatchAndRoundInfo_Should_CreateValidBracketNode()
    {
        // Arrange
        var match = CreateMatch();
        const int roundFromFinal = 2;
        const int indexInRound = 1;

        // Act
        var node = bracketNodeFactory.Create(match, roundFromFinal, indexInRound);

        // Assert
        node.Should().NotBeNull();
        node.Match.Should().Be(match);
        node.MatchId.Should().Be(match.Id);
        node.RoundFromFinal.Should().Be(roundFromFinal);
        node.IndexInRound.Should().Be(indexInRound);
    }

    [Test]
    public void Create_WithParentNode_Should_SetParent()
    {
        // Arrange
        var match = CreateMatch();
        var parent = new BracketNode();

        // Act
        var node = bracketNodeFactory.Create(match, 1, 0, parent);

        // Assert
        node.Parent.Should().Be(parent);
    }

    [Test]
    public void Create_WithChildrenNodes_Should_SetChildren()
    {
        // Arrange
        var match = CreateMatch();
        var child1 = new BracketNode();
        var child2 = new BracketNode();
        var children = new List<BracketNode> { child1, child2 };

        // Act
        var node = bracketNodeFactory.Create(match, 1, 0, children: children);

        // Assert
        node.Children.Should().BeEquivalentTo(children);
    }

    [Test]
    public void Create_WithParentAndChildren_Should_SetBoth()
    {
        // Arrange
        var match = CreateMatch();
        var parent = new BracketNode();
        var child = new BracketNode();
        var children = new List<BracketNode> { child };

        // Act
        var node = bracketNodeFactory.Create(match, 1, 0, parent, children);

        // Assert
        node.Parent.Should().Be(parent);
        node.Children.Should().BeEquivalentTo(children);
    }

    [Test]
    public void Create_Should_SetCreatedAt()
    {
        // Arrange
        var match = CreateMatch();
        var beforeCreate = DateTime.UtcNow;

        // Act
        var node = bracketNodeFactory.Create(match, 1, 0);

        // Assert
        node.CreatedAt.Should().BeOnOrAfter(beforeCreate);
    }

    [Test]
    public void Create_Should_SetUpdatedAt()
    {
        // Arrange
        var match = CreateMatch();
        var beforeCreate = DateTime.UtcNow;

        // Act
        var node = bracketNodeFactory.Create(match, 1, 0);

        // Assert
        node.UpdatedAt.Should().BeOnOrAfter(beforeCreate);
    }

    [Test]
    public void Create_Should_SetMatchId()
    {
        // Arrange
        var match = CreateMatch();

        // Act
        var node = bracketNodeFactory.Create(match, 1, 0);

        // Assert
        node.MatchId.Should().Be(match.Id);
    }

    [Test]
    public void Create_WithNullParent_Should_NotSetParent()
    {
        // Arrange
        var match = CreateMatch();

        // Act
        var node = bracketNodeFactory.Create(match, 1, 0, parent: null);

        // Assert
        node.Parent.Should().BeNull();
    }

    [Test]
    public void Create_WithNullChildren_Should_NotSetChildren()
    {
        // Arrange
        var match = CreateMatch();

        // Act
        var node = bracketNodeFactory.Create(match, 1, 0, children: null);

        // Assert
        node.Children.Should().BeNull();
    }

    [Test]
    public void Create_WithEmptyChildrenList_Should_SetEmptyChildren()
    {
        // Arrange
        var match = CreateMatch();
        var emptyChildren = new List<BracketNode>();

        // Act
        var node = bracketNodeFactory.Create(match, 1, 0, children: emptyChildren);

        // Assert
        node.Children.Should().BeEmpty();
    }

    [Test]
    public void Create_NodeWithAllParameters_Should_CreateValidNode()
    {
        // Arrange
        var match = CreateMatch();
        var parent = new BracketNode();
        var child1 = new BracketNode();
        var child2 = new BracketNode();
        var children = new List<BracketNode> { child1, child2 };

        // Act
        var node = bracketNodeFactory.Create(match, 2, 1, parent, children);

        // Assert
        node.Match.Should().Be(match);
        node.RoundFromFinal.Should().Be(2);
        node.IndexInRound.Should().Be(1);
        node.Parent.Should().Be(parent);
        node.Children.Should().HaveCount(2);
        node.CreatedAt.Should().NotBe(default);
        node.UpdatedAt.Should().NotBe(default);
    }

    [Test]
    public void Create_NodeWithZeroRoundFromFinal_Should_WorkCorrectly()
    {
        // Arrange
        var match = CreateMatch();

        // Act
        var node = bracketNodeFactory.Create(match, 0, 0);

        // Assert
        node.RoundFromFinal.Should().Be(0);
    }

    [Test]
    public void Create_NodeWithZeroIndexInRound_Should_WorkCorrectly()
    {
        // Arrange
        var match = CreateMatch();

        // Act
        var node = bracketNodeFactory.Create(match, 1, 0);

        // Assert
        node.IndexInRound.Should().Be(0);
    }

    [Test]
    public void Create_NodeWithLargeRoundFromFinal_Should_WorkCorrectly()
    {
        // Arrange
        var match = CreateMatch();

        // Act
        var node = bracketNodeFactory.Create(match, 10, 5);

        // Assert
        node.RoundFromFinal.Should().Be(10);
        node.IndexInRound.Should().Be(5);
    }

    [Test]
    public void Create_CreatedAt_And_UpdatedAt_Should_BeClose()
    {
        // Arrange
        var match = CreateMatch();

        // Act
        var node = bracketNodeFactory.Create(match, 1, 0);

        // Assert
        node.CreatedAt.Should().BeCloseTo(node.UpdatedAt, TimeSpan.FromSeconds(1));
    }

    private Match CreateMatch()
    {
        return new Match
        {
            Id = Guid.NewGuid(),
            Status = MatchStatus.Unplanned,
            MatchProcess = new MatchProcess(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
