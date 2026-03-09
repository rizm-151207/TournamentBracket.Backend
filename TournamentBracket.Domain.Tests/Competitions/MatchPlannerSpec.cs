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
    private MatchPlanner matchPlanner;
    private Competition competition;

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
    public void PlanMatchesForCompetitions_WithEmptyDivisions_Should_Succeed()
    {
        // Arrange
        var divisionsWithBrackets = new Dictionary<Division, Bracket>();

        // Act
        var result = matchPlanner.PlanMatchesForCompetitions(competition, divisionsWithBrackets);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void PlanMatchesForCompetitions_Should_PlanMatchesWithSequentialNumbers()
    {
        // Arrange
        var division = CreateDivision();
        var bracket = CreateFakeBracketWithMatches(3);
        var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };

        // Act
        var result = matchPlanner.PlanMatchesForCompetitions(competition, divisionsWithBrackets);

        // Assert
        result.Should().BeTrue();
        var matches = bracket.GetAllMatches();
        var plannedMatches = new List<Match>();
        foreach (var match in matches)
        {
            if (!match.IsByeMatch)
                plannedMatches.Add(match);
        }

        plannedMatches.Should().NotBeEmpty();
    }

    [Test]
    public void PlanMatchesForCompetitions_Should_SetPlannedDateTime()
    {
        // Arrange
        var division = CreateDivision();
        var bracket = CreateFakeBracketWithMatches(2);
        var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };
        var startDateTime = DateTime.UtcNow.AddDays(30);
        competition.StartDateTime = startDateTime;

        // Act
        matchPlanner.PlanMatchesForCompetitions(competition, divisionsWithBrackets);

        // Assert
        var nonByeMatches = bracket.GetAllMatches();
        foreach (var match in nonByeMatches)
        {
            if (!match.IsByeMatch)
            {
                match.PlannedDateTime.Should().NotBeNull();
                match.PlannedDateTime.Should().BeOnOrAfter(startDateTime);
            }
        }
    }

    [Test]
    public void PlanMatchesForCompetitions_Should_SetMatchStatusToPlanned()
    {
        // Arrange
        var division = CreateDivision();
        var bracket = CreateFakeBracketWithMatches(2);
        var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };

        // Act
        matchPlanner.PlanMatchesForCompetitions(competition, divisionsWithBrackets);

        // Assert
        var matches = bracket.GetAllMatches();
        foreach (var match in matches)
        {
            if (!match.IsByeMatch)
            {
                match.Status.Should().Be(MatchStatus.Planned);
            }
        }
    }

    [Test]
    public void PlanMatchesForCompetitions_ByeMatches_Should_BeUnplanned()
    {
        // Arrange
        var division = CreateDivision();
        var bracket = CreateFakeBracketWithByeMatches(2);
        var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };

        // Act
        matchPlanner.PlanMatchesForCompetitions(competition, divisionsWithBrackets);

        // Assert
        var byeMatches = bracket.GetAllMatches();
        foreach (var match in byeMatches)
        {
            if (match.IsByeMatch)
            {
                match.PlannedDateTime.Should().BeNull();
                match.Index.Should().BeNull();
            }
        }
    }

    [Test]
    public void PlanMatchesForCompetitions_MultipleDivisions_Should_PlanAllMatches()
    {
        // Arrange
        var division1 = CreateDivision(minAge: 12);
        var division2 = CreateDivision(minAge: 15);
        var bracket1 = CreateFakeBracketWithMatches(2);
        var bracket2 = CreateFakeBracketWithMatches(2);
        var divisionsWithBrackets = new Dictionary<Division, Bracket>
        {
            { division1, bracket1 },
            { division2, bracket2 }
        };

        // Act
        var result = matchPlanner.PlanMatchesForCompetitions(competition, divisionsWithBrackets);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void PlanMatchesForCompetitions_Should_IncrementMatchIndex()
    {
        // Arrange
        var division = CreateDivision();
        var bracket = CreateFakeBracketWithMatches(5);
        var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };

        // Act
        matchPlanner.PlanMatchesForCompetitions(competition, divisionsWithBrackets);

        // Assert
        var nonByeMatches = bracket.GetAllMatches();
        var plannedMatches = new List<Match>();
        foreach (var match in nonByeMatches)
        {
            if (!match.IsByeMatch)
                plannedMatches.Add(match);
        }

        plannedMatches.Should().NotBeEmpty();
    }

    [Test]
    public void PlanMatchesForCompetitions_Should_IncrementDateTimeByDefaultTime()
    {
        // Arrange
        var division = CreateDivision();
        var bracket = CreateFakeBracketWithMatches(3);
        var divisionsWithBrackets = new Dictionary<Division, Bracket> { { division, bracket } };
        var startDateTime = DateTime.UtcNow.AddDays(30);
        competition.StartDateTime = startDateTime;

        // Act
        matchPlanner.PlanMatchesForCompetitions(competition, divisionsWithBrackets);

        // Assert
        var nonByeMatches = bracket.GetAllMatches();
        var plannedMatches = new List<Match>();
        foreach (var match in nonByeMatches)
        {
            if (!match.IsByeMatch)
                plannedMatches.Add(match);
        }

        if (plannedMatches.Count >= 2)
        {
            var firstMatch = plannedMatches[0];
            var secondMatch = plannedMatches[1];
            var timeDifference = (secondMatch.PlannedDateTime!.Value - firstMatch.PlannedDateTime!.Value).TotalMinutes;
            timeDifference.Should().Be(3);
        }
    }

    [Test]
    public void PlanMatchesForCompetitions_WithNullDivisionsDictionary_Should_ThrowArgumentNullException()
    {
        // Act
        Action act = () => matchPlanner.PlanMatchesForCompetitions(competition, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    private Division CreateDivision(int? minAge = 12, int? maxAge = 18)
    {
        return new Division(Guid.NewGuid(), true, minAge: minAge, maxAge: maxAge);
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
