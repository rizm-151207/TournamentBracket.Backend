using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Tests.Matches;

[TestFixture]
public class MatchProcessSpec
{
    private MatchProcess _matchProcess;

    [SetUp]
    public void Setup()
    {
        _matchProcess = new MatchProcess();
    }

    [Test]
    public void AddWazari_ForFirstCompetitor_Should_IncrementWazari()
    {
        // Act
        _matchProcess.AddWazari(true);

        // Assert
        _matchProcess.FirstCompetitorWazari.Should().Be(1);
        _matchProcess.SecondCompetitorWazari.Should().Be(0);
    }

    [Test]
    public void AddWazari_ForSecondCompetitor_Should_IncrementWazari()
    {
        // Act
        _matchProcess.AddWazari(false);

        // Assert
        _matchProcess.FirstCompetitorWazari.Should().Be(0);
        _matchProcess.SecondCompetitorWazari.Should().Be(1);
    }

    [Test]
    public void AddWazari_MultipleTimes_Should_Accumulate()
    {
        // Act
        _matchProcess.AddWazari(true);
        _matchProcess.AddWazari(true);
        _matchProcess.AddWazari(true);

        // Assert
        _matchProcess.FirstCompetitorWazari.Should().Be(3);
    }

    [Test]
    public void AddKeikoku_ForFirstCompetitor_Should_IncrementKeikoku()
    {
        // Act
        _matchProcess.AddKeikoku(true);

        // Assert
        _matchProcess.FirstCompetitorKeikoku.Should().Be(1);
        _matchProcess.FirstCompetitorChui.Should().Be(0);
    }

    [Test]
    public void AddKeikoku_ForSecondCompetitor_Should_IncrementKeikoku()
    {
        // Act
        _matchProcess.AddKeikoku(false);

        // Assert
        _matchProcess.SecondCompetitorKeikoku.Should().Be(1);
        _matchProcess.SecondCompetitorChui.Should().Be(0);
    }

    [Test]
    public void AddKeikoku_TwoTimesForFirstCompetitor_Should_ResetKeikokuAndAddChui()
    {
        // Act
        _matchProcess.AddKeikoku(true);
        _matchProcess.AddKeikoku(true);

        // Assert
        _matchProcess.FirstCompetitorKeikoku.Should().Be(0);
        _matchProcess.FirstCompetitorChui.Should().Be(1);
    }

    [Test]
    public void AddKeikoku_TwoTimesForSecondCompetitor_Should_ResetKeikokuAndAddChui()
    {
        // Act
        _matchProcess.AddKeikoku(false);
        _matchProcess.AddKeikoku(false);

        // Assert
        _matchProcess.SecondCompetitorKeikoku.Should().Be(0);
        _matchProcess.SecondCompetitorChui.Should().Be(1);
    }

    [Test]
    public void AddChui_ForFirstCompetitor_Should_IncrementChui()
    {
        // Act
        _matchProcess.AddChui(true);

        // Assert
        _matchProcess.FirstCompetitorChui.Should().Be(1);
    }

    [Test]
    public void AddChui_ForSecondCompetitor_Should_IncrementChui()
    {
        // Act
        _matchProcess.AddChui(false);

        // Assert
        _matchProcess.SecondCompetitorChui.Should().Be(1);
    }

    [Test]
    public void AddChui_MultipleTimes_Should_Accumulate()
    {
        // Act
        _matchProcess.AddChui(true);
        _matchProcess.AddChui(true);
        _matchProcess.AddChui(true);

        // Assert
        _matchProcess.FirstCompetitorChui.Should().Be(3);
    }

    [Test]
    public void SetWinner_ForFirstCompetitor_Should_SetCorrectValues()
    {
        // Act
        _matchProcess.SetWinner(true, WinReason.Ippon);

        // Assert
        _matchProcess.Winner.Should().Be(MatchWinner.FirstCompetitor);
        _matchProcess.WinReason.Should().Be(WinReason.Ippon);
    }

    [Test]
    public void SetWinner_ForSecondCompetitor_Should_SetCorrectValues()
    {
        // Act
        _matchProcess.SetWinner(false, WinReason.Wazari);

        // Assert
        _matchProcess.Winner.Should().Be(MatchWinner.SecondCompetitor);
        _matchProcess.WinReason.Should().Be(WinReason.Wazari);
    }

    [Test]
    public void SetWinner_WithDifferentWinReasons_Should_SetCorrectReason()
    {
        // Arrange
        var winReasons = Enum.GetValues(typeof(WinReason)).Cast<WinReason>().ToList();

        foreach (var reason in winReasons)
        {
            // Act
            _matchProcess.SetWinner(true, reason);

            // Assert
            _matchProcess.WinReason.Should().Be(reason);
            
            // Reset for next iteration
            _matchProcess.Clear();
        }
    }

    [Test]
    public void Clear_Should_ResetAllCounters()
    {
        // Arrange
        _matchProcess.AddWazari(true);
        _matchProcess.AddWazari(false);
        _matchProcess.AddKeikoku(true);
        _matchProcess.AddChui(false);
        _matchProcess.SetWinner(true, WinReason.Ippon);

        // Act
        _matchProcess.Clear();

        // Assert
        _matchProcess.FirstCompetitorWazari.Should().Be(0);
        _matchProcess.FirstCompetitorKeikoku.Should().Be(0);
        _matchProcess.FirstCompetitorChui.Should().Be(0);
        _matchProcess.SecondCompetitorWazari.Should().Be(0);
        _matchProcess.SecondCompetitorKeikoku.Should().Be(0);
        _matchProcess.SecondCompetitorChui.Should().Be(0);
        _matchProcess.Winner.Should().BeNull();
        _matchProcess.WinReason.Should().BeNull();
    }

    [Test]
    public void KeikokuConversion_ToChui_Should_WorkCorrectly()
    {
        // Arrange
        _matchProcess.AddKeikoku(true); // 1 Keikoku
        _matchProcess.AddKeikoku(true); // 2 Keikoku -> 1 Chui, Keikoku reset

        // Assert
        _matchProcess.FirstCompetitorKeikoku.Should().Be(0);
        _matchProcess.FirstCompetitorChui.Should().Be(1);
    }

    [Test]
    public void MultipleKeikokuConversions_Should_AccumulateChui()
    {
        // Arrange
        _matchProcess.AddKeikoku(true);
        _matchProcess.AddKeikoku(true); // 1 Chui
        _matchProcess.AddKeikoku(true);
        _matchProcess.AddKeikoku(true); // 2 Chui

        // Assert
        _matchProcess.FirstCompetitorChui.Should().Be(2);
    }
}
