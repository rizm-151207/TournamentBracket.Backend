using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Competitions;
using TournamentBracket.Domain.Divisions;

namespace TournamentBracket.Domain.Tests.Competitions;

[TestFixture]
public class CompetitionSpec
{
    private Competition competition;

    [SetUp]
    public void Setup()
    {
        competition = new Competition
        {
            Id = Guid.NewGuid(),
            Name = "Test Competition",
            Location = "Test Location",
            StartDateTime = DateTime.UtcNow.AddDays(30),
            Status = CompetitionStatus.Planned,
            OwnerEmail = "owner@example.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    [Test]
    public void TryAddDivision_WithValidDivision_Should_ReturnTrue()
    {
        // Arrange
        var division = new Division(competition.Id, true, minAge: 12, maxAge: 15);

        // Act
        var result = competition.TryAddDivision(division);

        // Assert
        result.Should().BeTrue();
        division.CompetitionId.Should().Be(competition.Id);
    }

    [Test]
    public void TryAddDivision_Should_SetCompetitionId()
    {
        // Arrange
        var division = new Division(Guid.NewGuid(), true, minAge: 12, maxAge: 15);

        // Act
        competition.TryAddDivision(division);

        // Assert
        division.CompetitionId.Should().Be(competition.Id);
    }
}
