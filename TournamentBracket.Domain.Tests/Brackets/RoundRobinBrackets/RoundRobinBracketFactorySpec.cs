using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Brackets.RoundRobinBracket;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Tests.Brackets.RoundRobinBrackets;

[TestFixture]
public class RoundRobinBracketFactorySpec
{
    private RoundRobinBracketFactory factory;
    private MatchFactory matchFactory;

    [SetUp]
    public void Setup()
    {
        matchFactory = new MatchFactory();
        factory = new RoundRobinBracketFactory(matchFactory);
    }

    [Test]
    public void CreateBracket_WithOneCompetitor_Should_CreateBracketWithByeMatch()
    {
        // Arrange
        var competitors = CreateCompetitors(1);

        // Act
        var bracket = factory.CreateBracket(competitors) as RoundRobinBracket;

        // Assert
        bracket.Should().NotBeNull();
        bracket!.Matches.Should().HaveCount(1);
        bracket.Matches[0].IsByeMatch.Should().BeTrue();
        bracket.Matches[0].FirstCompetitor.Should().Be(competitors[0]);
        bracket.Type.Should().Be(BracketType.RoundRobin);
    }

    [Test]
    public void CreateBracket_WithTwoCompetitors_Should_CreateBracketWithOneMatch()
    {
        // Arrange
        var competitors = CreateCompetitors(2);

        // Act
        var bracket = factory.CreateBracket(competitors) as RoundRobinBracket;

        // Assert
        bracket.Should().NotBeNull();
        bracket!.Matches.Should().HaveCount(1);
        bracket.Matches[0].FirstCompetitor.Should().Be(competitors[0]);
        bracket.Matches[0].SecondCompetitor.Should().Be(competitors[1]);
        bracket.Matches[0].IsByeMatch.Should().BeFalse();
    }

    [Test]
    public void CreateBracket_WithThreeCompetitors_Should_CreateBracketWithThreeMatches()
    {
        // Arrange
        var competitors = CreateCompetitors(3);

        // Act
        var bracket = factory.CreateBracket(competitors) as RoundRobinBracket;

        // Assert
        bracket.Should().NotBeNull();
        bracket!.Matches.Should().HaveCount(3);
        bracket.Matches[0].FirstCompetitor.Should().Be(competitors[0]);
        bracket.Matches[0].SecondCompetitor.Should().Be(competitors[1]);
        bracket.Matches[1].FirstCompetitor.Should().Be(competitors[1]);
        bracket.Matches[1].SecondCompetitor.Should().Be(competitors[2]);
        bracket.Matches[2].FirstCompetitor.Should().Be(competitors[2]);
        bracket.Matches[2].SecondCompetitor.Should().Be(competitors[0]);
    }

    [Test]
    public void CreateBracket_WithEmptyList_Should_ThrowArgumentException()
    {
        // Arrange
        var competitors = new List<Competitor>();

        // Act
        Action act = () => factory.CreateBracket(competitors);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Empty competitor list");
    }

    [Test]
    public void CreateBracket_WithMoreThanThreeCompetitors_Should_ThrowArgumentException()
    {
        // Arrange
        var competitors = CreateCompetitors(4);

        // Act
        Action act = () => factory.CreateBracket(competitors);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Competitors count must be less than 4. Current count: 4");
    }

    [Test]
    public void CreateBracket_Should_SetCreatedAtAndUpdatedAt()
    {
        // Arrange
        var competitors = CreateCompetitors(2);
        var beforeCreate = DateTime.UtcNow;

        // Act
        var bracket = factory.CreateBracket(competitors) as RoundRobinBracket;

        // Assert
        bracket!.CreatedAt.Should().BeOnOrAfter(beforeCreate);
        bracket.UpdatedAt.Should().BeOnOrAfter(beforeCreate);
    }

    [Test]
    public void ExtendBracket_WithOneMatch_Should_ExtendToThreeMatches()
    {
        // Arrange
        var competitors = CreateCompetitors(2);
        var bracket = factory.CreateBracket(competitors) as RoundRobinBracket;

        // Act
        factory.ExtendBracket(bracket!);

        // Assert
        bracket!.Matches.Should().HaveCount(3);
        bracket.UpdatedAt.Should().BeOnOrAfter(bracket.CreatedAt);
    }

    [Test]
    public void ExtendBracket_WithByeMatch_Should_ThrowInvalidOperationException()
    {
        // Arrange
        var competitors = CreateCompetitors(1);
        var bracket = factory.CreateBracket(competitors) as RoundRobinBracket;

        // Act
        Action act = () => factory.ExtendBracket(bracket!);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Can't extend round robin bracket. There is free match");
    }

    [Test]
    public void ExtendBracket_WithThreeMatches_Should_ThrowInvalidOperationException()
    {
        // Arrange
        var competitors = CreateCompetitors(3);
        var bracket = factory.CreateBracket(competitors) as RoundRobinBracket;

        // Act
        Action act = () => factory.ExtendBracket(bracket!);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Can extend round robin bracket only with 1 match. Current matches count: 3");
    }

    [Test]
    public void ExtendBracket_WithNonRoundRobinBracket_Should_ThrowArgumentException()
    {
        // Arrange
        var bracket = new FakeBracket();

        // Act
        Action act = () => factory.ExtendBracket(bracket);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void NeedToReduce_WithThreeMatchesAndByeMatch_Should_ReturnTrue()
    {
        // Arrange
        var competitors = CreateCompetitors(3);
        var matches = new List<Match>
        {
            matchFactory.CreateMatchByCompetitors(competitors[0], competitors[1]),
            matchFactory.CreateMatchByCompetitors(competitors[1], null), // bye match
            matchFactory.CreateMatchByCompetitors(competitors[2], competitors[0])
        };
        var bracket = new RoundRobinBracket { Matches = matches };

        // Act
        var result = factory.NeedToReduce(bracket);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void NeedToReduce_WithThreeMatchesNoBye_Should_ReturnFalse()
    {
        // Arrange
        var competitors = CreateCompetitors(3);
        var matches = new List<Match>
        {
            matchFactory.CreateMatchByCompetitors(competitors[0], competitors[1]),
            matchFactory.CreateMatchByCompetitors(competitors[1], competitors[2]),
            matchFactory.CreateMatchByCompetitors(competitors[2], competitors[0])
        };
        var bracket = new RoundRobinBracket { Matches = matches };

        // Act
        var result = factory.NeedToReduce(bracket);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void NeedToReduce_WithOneMatch_Should_ReturnFalse()
    {
        // Arrange
        var competitors = CreateCompetitors(2);
        var bracket = factory.CreateBracket(competitors) as RoundRobinBracket;

        // Act
        var result = factory.NeedToReduce(bracket!);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void NeedToReduce_WithNonRoundRobinBracket_Should_ThrowArgumentException()
    {
        // Arrange
        var bracket = new FakeBracket();

        // Act
        Action act = () => factory.NeedToReduce(bracket);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void RebalanceBracket_Should_NotThrowException()
    {
        // Arrange
        var competitors = CreateCompetitors(2);
        var bracket = factory.CreateBracket(competitors) as RoundRobinBracket;

        // Act & Assert
        Action act = () => factory.RebalanceBracket(bracket!);
        act.Should().NotThrow();
    }

    [Test]
    public void ReduceBracket_WithThreeMatches_Should_RemoveByeMatches()
    {
        // Arrange
        var competitors = CreateCompetitors(3);
        var matches = new List<Match>
        {
            matchFactory.CreateMatchByCompetitors(competitors[0], competitors[1]),
            matchFactory.CreateMatchByCompetitors(competitors[1], null), // bye match
            matchFactory.CreateMatchByCompetitors(competitors[2], null)  // bye match
        };
        var bracket = new RoundRobinBracket { Matches = matches };

        // Act
        factory.ReduceBracket(bracket);

        // Assert
        bracket.Matches.Should().HaveCount(1);
        bracket.Matches[0].FirstCompetitor.Should().Be(competitors[0]);
        bracket.Matches[0].SecondCompetitor.Should().Be(competitors[1]);
    }

    [Test]
    public void ReduceBracket_WithOneMatch_Should_ThrowInvalidOperationException()
    {
        // Arrange
        var competitors = CreateCompetitors(2);
        var bracket = factory.CreateBracket(competitors) as RoundRobinBracket;

        // Act
        Action act = () => factory.ReduceBracket(bracket!);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Can reduce bracket only with 3 matches. Current mathces is: 1");
    }

    [Test]
    public void ReduceBracket_WithNonRoundRobinBracket_Should_ThrowArgumentException()
    {
        // Arrange
        var bracket = new FakeBracket();

        // Act
        Action act = () => factory.ReduceBracket(bracket);

        // Assert
        act.Should().Throw<ArgumentException>();
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
                Subject = $"subject{i}"
            });
        }
        return competitors;
    }

    // Fake implementation for testing
    private class FakeBracket : Bracket
    {
        public override BracketType Type => BracketType.RoundRobin;

        public override bool TryAddCompetitorAuto(Competitor competitor) => false;
        public override bool TryRemoveCompetitorAuto(Competitor competitor, out bool hasEmptyMatch)
        {
            hasEmptyMatch = false;
            return false;
        }
        public override bool HasFreeMatch() => false;
        public override List<Competitor> GetAllCompetitors() => new();
        public override Dictionary<int, IReadOnlyCollection<Match>> GetGroupedMatchesByRounds() => new();
        public override IReadOnlyCollection<Match> GetAllMatches() => new List<Match>();
        public override void RefreshBracketAfterMatchUpdate(Match match) { }
    }
}
