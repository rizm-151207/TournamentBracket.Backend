using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Tests.Matches;

[TestFixture]
public class MatchSpec
{
    private Match match;
    private Competitor competitor1;
    private Competitor competitor2;

    [SetUp]
    public void Setup()
    {
        match = new Match
        {
            Id = Guid.NewGuid(),
            Status = MatchStatus.Unplanned,
            MatchProcess = new MatchProcess(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        competitor1 = new Competitor
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Gender = true,
            DateOfBirth = DateTime.Now.AddYears(-25),
            Weight = 75.5f,
            Subject = "Judo"
        };

        competitor2 = new Competitor
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Smith",
            Gender = false,
            DateOfBirth = DateTime.Now.AddYears(-28),
            Weight = 65.0f,
            Subject = "Karate"
        };
    }

    [Test]
    public void AddCompetitor_When_FirstCompetitor_IsNull_Should_AddAs_FirstCompetitor()
    {
        // Act
        match.AddCompetitor(competitor1);

        // Assert
        match.FirstCompetitor.Should().Be(competitor1);
        match.SecondCompetitor.Should().BeNull();
        match.Status.Should().Be(MatchStatus.WaitingOtherCompetitor);
    }

    [Test]
    public void AddCompetitor_When_SecondCompetitor_IsNull_Should_AddAs_SecondCompetitor()
    {
        // Arrange
        match.AddCompetitor(competitor1);

        // Act
        match.AddCompetitor(competitor2);

        // Assert
        match.FirstCompetitor.Should().Be(competitor1);
        match.SecondCompetitor.Should().Be(competitor2);
        match.Status.Should().Be(MatchStatus.Unplanned);
    }

    [Test]
    public void AddCompetitor_When_MatchIsFull_Should_Throw_InvalidOperationException()
    {
        // Arrange
        match.AddCompetitor(competitor1);
        match.AddCompetitor(competitor2);

        // Act
        Action act = () => match.AddCompetitor(new Competitor());

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Can't add competitor to match. Match * already full");
    }

    [Test]
    public void RemoveCompetitor_With_ExistingCompetitor_Should_Remove()
    {
        // Arrange
        match.AddCompetitor(competitor1);
        match.AddCompetitor(competitor2);

        // Act
        match.RemoveCompetitor(competitor1);

        // Assert
        match.FirstCompetitor.Should().BeNull();
        match.SecondCompetitor.Should().Be(competitor2);
        match.Status.Should().Be(MatchStatus.Finished);
    }

    [Test]
    public void RemoveCompetitor_With_NonExistingCompetitor_Should_Throw_InvalidOperationException()
    {
        // Arrange
        var nonExistingCompetitor = new Competitor { Id = Guid.NewGuid() };

        // Act
        Action act = () => match.RemoveCompetitor(nonExistingCompetitor);

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Can't find competitor in competition to remove");
    }

    [Test]
    public void TryGetWinner_With_FinishedMatchAnd_Winner_Should_Return_Winner()
    {
        // Arrange
        match.AddCompetitor(competitor1);
        match.AddCompetitor(competitor2);
        match.MatchProcess.SetWinner(true, WinReason.Ippon); // First competitor wins
        match.Status = MatchStatus.Finished;

        // Act
        var result = match.TryGetWinner(out var winner);

        // Assert
        result.Should().BeTrue();
        winner.Should().Be(competitor1);
    }

    [Test]
    public void TryGetWinner_With_NonFinishedMatch_Should_Return_False()
    {
        // Arrange
        match.AddCompetitor(competitor1);
        match.AddCompetitor(competitor2);
        // Status remains Unplanned

        // Act
        var result = match.TryGetWinner(out var winner);

        // Assert
        result.Should().BeFalse();
        winner.Should().BeNull();
    }

    [Test]
    public void TryGetLoser_With_FinishedMatchAnd_Loser_Should_Return_Loser()
    {
        // Arrange
        match.AddCompetitor(competitor1);
        match.AddCompetitor(competitor2);
        match.MatchProcess.SetWinner(true, WinReason.Ippon); // First competitor wins
        match.Status = MatchStatus.Finished;

        // Act
        var result = match.TryGetLoser(out var loser);

        // Assert
        result.Should().BeTrue();
        loser.Should().Be(competitor2);
    }

    [Test]
    public void Plan_Should_Set_IndexAndPlannedTime()
    {
        // Arrange
        var plannedTime = DateTime.UtcNow.AddHours(1);
        const int index = 5;
        const int tatamiNum = 2;

        // Act
        match.Plan(tatamiNum, index, plannedTime);

        // Assert
        match.Index.Should().Be("Поединок 5");
        match.PlannedDateTime.Should().Be(plannedTime);
        match.Status.Should().Be(MatchStatus.Planned);
    }

    [Test]
    public void UpdateMatch_With_Ippon_Should_FinishMatch()
    {
        // Arrange
        match.AddCompetitor(competitor1);
        match.AddCompetitor(competitor2);
        var updateEvent = new MatchUpdateEvent { Type = MatchUpdateType.Ippon, IsFirstCompetitor = true };

        // Act
        match.UpdateMatch(updateEvent);

        // Assert
        match.MatchProcess.Winner.Should().Be(MatchWinner.FirstCompetitor);
        match.MatchProcess.WinReason.Should().Be(WinReason.Ippon);
        match.Status.Should().Be(MatchStatus.Finished);
        match.EndDateTime.Should().NotBeNull();
    }

    [Test]
    public void UpdateMatch_With_Wazari_Should_IncrementWazari()
    {
        // Arrange
        match.AddCompetitor(competitor1);
        match.AddCompetitor(competitor2);
        var updateEvent = new MatchUpdateEvent { Type = MatchUpdateType.Wazari, IsFirstCompetitor = true };

        // Act
        match.UpdateMatch(updateEvent);

        // Assert
        match.MatchProcess.FirstCompetitorWazari.Should().Be(1);
        match.MatchProcess.SecondCompetitorWazari.Should().Be(0);
    }

    [Test]
    public void IsByeMatch_Property_Should_Identify_ByeMatches()
    {
        // Arrange - non-bye match
        match.Status = MatchStatus.Finished;
        match.MatchProcess.WinReason = WinReason.Ippon; // Not a Bye

        // Act & Assert
        match.IsByeMatch.Should().BeFalse();

        // Arrange - bye match
        match.Status = MatchStatus.Finished;
        match.MatchProcess.WinReason = WinReason.Bye;

        // Act & Assert
        match.IsByeMatch.Should().BeTrue();
    }

    [Test]
    public void UnplanBye_With_ByeMatch_Should_Clear_IndexAndPlannedTime()
    {
        // Arrange
        match.Status = MatchStatus.Finished;
        match.MatchProcess.WinReason = WinReason.Bye;
        match.Index = "Bye Match";
        match.PlannedDateTime = DateTime.UtcNow;

        // Act
        match.UnplanBye();

        // Assert
        match.Index.Should().BeNull();
        match.PlannedDateTime.Should().BeNull();
    }

    [Test]
    public void UnplanBye_With_NonByeMatch_Should_Throw_InvalidOperationException()
    {
        // Arrange
        match.Status = MatchStatus.Finished;
        match.MatchProcess.WinReason = WinReason.Ippon; // Not a bye match

        // Act
        Action act = () => match.UnplanBye();

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage($"{nameof(Match.UnplanBye)} method can be used only with bye match");
    }

    [Test]
    public void Clear_Should_Reset_MatchState()
    {
        // Arrange
        match.AddCompetitor(competitor1);
        match.AddCompetitor(competitor2);
        match.Index = "Test Match";
        match.PlannedDateTime = DateTime.UtcNow;
        match.StartDateTime = DateTime.UtcNow;
        match.MatchProcess.AddWazari(true);

        // Act
        match.Clear();

        // Assert
        match.FirstCompetitor.Should().BeNull();
        match.SecondCompetitor.Should().BeNull();
        match.PlannedDateTime.Should().BeNull();
        match.StartDateTime.Should().BeNull();
        match.EndDateTime.Should().BeNull();
        match.MatchProcess.Winner.Should().BeNull();
        match.MatchProcess.WinReason.Should().BeNull();
    }
}