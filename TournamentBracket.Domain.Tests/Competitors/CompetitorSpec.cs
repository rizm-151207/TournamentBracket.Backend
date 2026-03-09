using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Competitors;

namespace TournamentBracket.Domain.Tests.Competitors;

[TestFixture]
public class CompetitorSpec
{
    private Competitor competitor;

    [SetUp]
    public void Setup()
    {
        competitor = new Competitor
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Gender = true,
            DateOfBirth = DateTime.Now.AddYears(-25),
            Weight = 75.5f,
            Subject = "MMA",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    [Test]
    public void Age_Property_Should_Calculate_Correctly()
    {
        // Arrange
        var dateOfBirth = DateTime.Now.AddYears(-25);
        competitor.DateOfBirth = dateOfBirth;

        // Act & Assert
        competitor.Age.Should().Be(25);
    }
}