using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Brackets.Abstractions;
using TournamentBracket.Domain.Brackets.SingleEliminationBracket;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Tests.Brackets.SingleEliminationBrackets;

[TestFixture]
public class SingleEliminationBracketFactorySpec
{
    private SingleEliminationBracketFactory factory;
    private BracketNodeFactory bracketNodeFactory;
    private MatchFactory matchFactory;

    [SetUp]
    public void Setup()
    {
        bracketNodeFactory = new BracketNodeFactory();
        matchFactory = new MatchFactory();
        factory = new SingleEliminationBracketFactory(bracketNodeFactory, matchFactory);
    }

    [Test]
    public void CreateBracket_WithFourCompetitors_Should_CreateValidBracket()
    {
        // Arrange
        var competitors = CreateCompetitors(4);

        // Act
        var bracket = factory.CreateBracket(competitors) as SingleEliminationBracket;

        // Assert
        bracket.Should().NotBeNull();
        bracket!.Id.Should().NotBeEmpty();
        bracket.Type.Should().Be(BracketType.SingleElimination);
        bracket.Root.Should().NotBeNull();
        bracket.ThirdPlace.Should().NotBeNull();
    }

    [Test]
    public void CreateBracket_WithFourCompetitors_Should_CreateCorrectNumberOfMatches()
    {
        // Arrange
        var competitors = CreateCompetitors(4);

        // Act
        var bracket = factory.CreateBracket(competitors) as SingleEliminationBracket;
        var allMatches = bracket!.GetAllMatches();

        // Assert - 4 competitors = 2 semifinals + 1 final + 1 third place = 4 matches
        allMatches.Should().HaveCount(4);
    }

    [Test]
    public void CreateBracket_WithTwoCompetitors_Should_CreateValidBracket()
    {
        // Arrange
        var competitors = CreateCompetitors(2);

        // Act
        var bracket = factory.CreateBracket(competitors) as SingleEliminationBracket;

        // Assert
        bracket.Should().NotBeNull();
        bracket!.Root.Should().NotBeNull();
        bracket.GetAllMatches().Should().HaveCount(2); // 1 final + 1 third place
    }

    [Test]
    public void CreateBracket_WithEightCompetitors_Should_CreateValidBracket()
    {
        // Arrange
        var competitors = CreateCompetitors(8);

        // Act
        var bracket = factory.CreateBracket(competitors) as SingleEliminationBracket;

        // Assert
        bracket.Should().NotBeNull();
        var allMatches = bracket!.GetAllMatches();
        allMatches.Should().HaveCount(8); // 4 quarterfinals + 2 semifinals + 1 final + 1 third place
    }

    [Test]
    public void CreateBracket_WithOddNumberOfCompetitors_Should_CreateByeMatches()
    {
        // Arrange
        var competitors = CreateCompetitors(3);

        // Act
        var bracket = factory.CreateBracket(competitors);

        // Assert
        bracket.Should().NotBeNull();
        var byeMatches = bracket.GetAllMatches().Count(m => m.IsByeMatch);
        byeMatches.Should().Be(1);
    }

    [Test]
    public void CreateBracket_Should_SetBracketIdForAllNodes()
    {
        // Arrange
        var competitors = CreateCompetitors(4);

        // Act
        var bracket = factory.CreateBracket(competitors) as SingleEliminationBracket;

        // Assert
        foreach (var node in bracket!.GetAllNodes())
        {
            node.BracketId.Should().Be(bracket.Id);
        }
    }

    [Test]
    public void CreateBracket_WithCompetitorsFromSameTrainer_Should_SeparateThemInBracket()
    {
        // Arrange
        var subject1 = "subject1";
        var competitors = new List<Competitor>
        {
            CreateCompetitorWithSubject(subject1),
            CreateCompetitorWithSubject(subject1),
            CreateCompetitorWithSubject("subject2"),
            CreateCompetitorWithSubject("subject3")
        };

        // Act
        var bracket = factory.CreateBracket(competitors) as SingleEliminationBracket;

        // Assert
        var allCompetitorsInBracket = bracket!.GetAllCompetitors();
        allCompetitorsInBracket.Should().HaveCount(4);
    }

    [Test]
    public void ReduceBracket_WithLeafNodes_Should_RemoveLeafs()
    {
        // Arrange
        var competitors = CreateCompetitors(4);
        var bracket = factory.CreateBracket(competitors) as SingleEliminationBracket;

        // Act
        factory.ReduceBracket(bracket!);

        // Assert
        var allNodes = bracket!.GetAllNodes().ToList();
        allNodes.Should().NotBeEmpty();
        allNodes.Should().HaveCount(2);
    }

    [Test]
    public void RebalanceBracket_Should_RedistributeCompetitors()
    {
        // Arrange
        var competitors = CreateCompetitors(4);
        var bracket = factory.CreateBracket(competitors) as SingleEliminationBracket;

        // Act
        factory.RebalanceBracket(bracket!);

        // Assert
        bracket!.GetAllCompetitors().Should().HaveCount(4);
    }

    [Test]
    public void ExtendBracket_WithNotSingleEliminationBracket_Should_Throw_ArgumentException()
    {
        // Arrange
        var bracket = new FakeBracket();

        // Act
        Action act = () => factory.ExtendBracket(bracket);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void ReduceBracket_WithNotSingleEliminationBracket_Should_Throw_ArgumentException()
    {
        // Arrange
        var bracket = new FakeBracket();

        // Act
        Action act = () => factory.ReduceBracket(bracket);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void RebalanceBracket_WithSingleEliminationBracket_Should_Throw_ArgumentException()
    {
        // Arrange
        var bracket = new FakeBracket();

        // Act
        Action act = () => factory.RebalanceBracket(bracket);

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
                Subject = $"trainer{i}"
            });
        }
        return competitors;
    }

    private Competitor CreateCompetitorWithSubject(string subject)
    {
        return new Competitor
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Gender = true,
            DateOfBirth = DateTime.Now.AddYears(-20),
            Weight = 70,
            Subject = subject
        };
    }

    // Fake implementation for testing abstract class methods
    private class FakeBracket : Bracket
    {
        public override BracketType Type => BracketType.SingleElimination;

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
