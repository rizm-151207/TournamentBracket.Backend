using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Competitors;
using TournamentBracket.Domain.Divisions;

namespace TournamentBracket.Domain.Tests.Divisions;

[TestFixture]
public class DivisionSpec
{
    private Guid _competitionId;

    [SetUp]
    public void Setup()
    {
        _competitionId = Guid.NewGuid();
    }

    [Test]
    public void Constructor_WithMinAgeOnly_Should_CreateValidDivision()
    {
        // Act
        var division = new Division(_competitionId, true, minAge: 12);

        // Assert
        division.MinAge.Should().Be(12);
        division.MaxAge.Should().BeNull();
        division.Gender.Should().BeTrue();
        division.CompetitionId.Should().Be(_competitionId);
    }

    [Test]
    public void Constructor_WithMaxAgeOnly_Should_CreateValidDivision()
    {
        // Act
        var division = new Division(_competitionId, false, maxAge: 15);

        // Assert
        division.MinAge.Should().BeNull();
        division.MaxAge.Should().Be(15);
        division.Gender.Should().BeFalse();
    }

    [Test]
    public void Constructor_WithMinAndMaxAge_Should_CreateValidDivision()
    {
        // Act
        var division = new Division(_competitionId, true, minAge: 12, maxAge: 15);

        // Assert
        division.MinAge.Should().Be(12);
        division.MaxAge.Should().Be(15);
    }

    [Test]
    public void Constructor_WithWeightLimits_Should_CreateValidDivision()
    {
        // Act
        var division = new Division(_competitionId, true, minAge: 12, maxAge: 15, minWeight: 50, maxWeight: 60);

        // Assert
        division.MinWeight.Should().Be(50);
        division.MaxWeight.Should().Be(60);
    }

    [Test]
    public void Constructor_WithNullMinAndMaxAge_Should_Throw_ArgumentNullException()
    {
        // Act
        Action act = () => new Division(_competitionId, true, minAge: null, maxAge: null);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void TryAddCompetitor_WithValidCompetitor_Should_ReturnTrue()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 10, maxAge: 20);
        var competitor = CreateCompetitor(age: 15, gender: true, weight: 55);

        // Act
        var result = division.TryAddCompetitor(competitor);

        // Assert
        result.Should().BeTrue();
        division.Competitors.Should().Contain(competitor);
        competitor.Divisions.Should().Contain(division);
    }

    [Test]
    public void TryAddCompetitor_WithAlreadyAddedCompetitor_Should_ReturnFalse()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 10, maxAge: 20);
        var competitor = CreateCompetitor(age: 15, gender: true, weight: 55);
        division.TryAddCompetitor(competitor);

        // Act
        var result = division.TryAddCompetitor(competitor);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void TryAddCompetitor_WithWrongGender_Should_ReturnFalse()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 10, maxAge: 20);
        var competitor = CreateCompetitor(age: 15, gender: false, weight: 55);

        // Act
        var result = division.TryAddCompetitor(competitor);

        // Assert
        result.Should().BeFalse();
        division.Competitors.Should().BeEmpty();
    }

    [Test]
    public void TryAddCompetitor_WithTooYoungCompetitor_Should_ReturnFalse()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 15, maxAge: 20);
        var competitor = CreateCompetitor(age: 12, gender: true, weight: 55);

        // Act
        var result = division.TryAddCompetitor(competitor);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void TryAddCompetitor_WithTooOldCompetitor_Should_ReturnFalse()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 15, maxAge: 20);
        var competitor = CreateCompetitor(age: 25, gender: true, weight: 55);

        // Act
        var result = division.TryAddCompetitor(competitor);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void TryAddCompetitor_WithTooLightCompetitor_Should_ReturnFalse()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 15, maxAge: 20, minWeight: 60);
        var competitor = CreateCompetitor(age: 17, gender: true, weight: 55);

        // Act
        var result = division.TryAddCompetitor(competitor);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void TryAddCompetitor_WithTooHeavyCompetitor_Should_ReturnFalse()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 15, maxAge: 20, maxWeight: 60);
        var competitor = CreateCompetitor(age: 17, gender: true, weight: 70);

        // Act
        var result = division.TryAddCompetitor(competitor);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void TryRemoveCompetitor_WithExistingCompetitor_Should_ReturnTrue()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 10, maxAge: 20);
        var competitor = CreateCompetitor(age: 15, gender: true, weight: 55);
        division.TryAddCompetitor(competitor);

        // Act
        var result = division.TryRemoveCompetitor(competitor);

        // Assert
        result.Should().BeTrue();
        division.Competitors.Should().BeEmpty();
        competitor.Divisions.Should().BeEmpty();
    }

    [Test]
    public void TryRemoveCompetitor_WithNonExistingCompetitor_Should_ReturnFalse()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 10, maxAge: 20);
        var competitor = CreateCompetitor(age: 15, gender: true, weight: 55);

        // Act
        var result = division.TryRemoveCompetitor(competitor);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsCompetitorFits_WithValidCompetitor_Should_ReturnTrue()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 12, maxAge: 18, minWeight: 50, maxWeight: 70);
        var competitor = CreateCompetitor(age: 15, gender: true, weight: 60);

        // Act
        var result = division.IsCompetitorFits(competitor);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void IsCompetitorFits_WithWrongGender_Should_ReturnFalse()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 12, maxAge: 18);
        var competitor = CreateCompetitor(age: 15, gender: false, weight: 60);

        // Act
        var result = division.IsCompetitorFits(competitor);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsCompetitorFits_WithOnlyMinAge_Should_CheckMinAgeOnly()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 15);
        var youngCompetitor = CreateCompetitor(age: 12, gender: true, weight: 60);
        var oldCompetitor = CreateCompetitor(age: 50, gender: true, weight: 60);

        // Act & Assert
        division.IsCompetitorFits(youngCompetitor).Should().BeFalse();
        division.IsCompetitorFits(oldCompetitor).Should().BeTrue();
    }

    [Test]
    public void IsCompetitorFits_WithOnlyMaxAge_Should_CheckMaxAgeOnly()
    {
        // Arrange
        var division = new Division(_competitionId, true, maxAge: 18);
        var youngCompetitor = CreateCompetitor(age: 12, gender: true, weight: 60);
        var oldCompetitor = CreateCompetitor(age: 25, gender: true, weight: 60);

        // Act & Assert
        division.IsCompetitorFits(youngCompetitor).Should().BeTrue();
        division.IsCompetitorFits(oldCompetitor).Should().BeFalse();
    }

    [Test]
    public void IsCompetitorFits_WithOnlyMinWeight_Should_CheckMinWeightOnly()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 15, minWeight: 60);
        var lightCompetitor = CreateCompetitor(age: 17, gender: true, weight: 55);
        var heavyCompetitor = CreateCompetitor(age: 17, gender: true, weight: 70);

        // Act & Assert
        division.IsCompetitorFits(lightCompetitor).Should().BeFalse();
        division.IsCompetitorFits(heavyCompetitor).Should().BeTrue();
    }

    [Test]
    public void IsCompetitorFits_WithOnlyMaxWeight_Should_CheckMaxWeightOnly()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 15, maxWeight: 60);
        var lightCompetitor = CreateCompetitor(age: 17, gender: true, weight: 55);
        var heavyCompetitor = CreateCompetitor(age: 17, gender: true, weight: 70);

        // Act & Assert
        division.IsCompetitorFits(lightCompetitor).Should().BeTrue();
        division.IsCompetitorFits(heavyCompetitor).Should().BeFalse();
    }

    [Test]
    public void Name_Should_BeGeneratedCorrectly_ForGirls()
    {
        // Arrange
        var division = new Division(_competitionId, false, maxAge: 9);

        // Assert
        division.Name.Should().StartWith("Девочки");
    }

    [Test]
    public void Name_Should_BeGeneratedCorrectly_ForWomen()
    {
        // Arrange
        var division = new Division(_competitionId, false, minAge: 18);

        // Assert
        division.Name.Should().StartWith("Женщины");
    }

    [Test]
    public void Name_Should_BeGeneratedCorrectly_ForBoys()
    {
        // Arrange
        var division = new Division(_competitionId, true, maxAge: 9);

        // Assert
        division.Name.Should().StartWith("Мальчики");
    }

    [Test]
    public void Name_Should_BeGeneratedCorrectly_ForMen()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 18);

        // Assert
        division.Name.Should().StartWith("Мужчины");
    }

    [Test]
    public void Name_Should_IncludeAgeRange()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 12, maxAge: 14);

        // Assert
        division.Name.Should().Contain("12 - 14");
    }

    [Test]
    public void Name_Should_IncludeWeightRange()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 12, minWeight: 50, maxWeight: 60);

        // Assert
        division.Name.Should().Contain("50 - 60");
    }

    [Test]
    public void Properties_Should_HaveCorrectInitialValues()
    {
        // Act
        var division = new Division(_competitionId, true, minAge: 12, maxAge: 15);

        // Assert
        division.Id.Should().BeEmpty();
        division.TournamentBracketId.Should().BeEmpty();
        division.BracketType.Should().Be(BracketType.RoundRobin); // Default enum value
        division.Competitors.Should().NotBeNull();
        division.Competitors.Should().BeEmpty();
        division.CreatedAt.Should().NotBe(default);
        division.UpdatedAt.Should().NotBe(default);
    }

    [Test]
    public void TryAddCompetitor_Should_AddMultipleCompetitors()
    {
        // Arrange
        var division = new Division(_competitionId, true, minAge: 10, maxAge: 20);
        var competitor1 = CreateCompetitor(age: 12, gender: true, weight: 50);
        var competitor2 = CreateCompetitor(age: 15, gender: true, weight: 55);
        var competitor3 = CreateCompetitor(age: 18, gender: true, weight: 60);

        // Act
        division.TryAddCompetitor(competitor1);
        division.TryAddCompetitor(competitor2);
        division.TryAddCompetitor(competitor3);

        // Assert
        division.Competitors.Should().HaveCount(3);
    }

    private Competitor CreateCompetitor(int age, bool gender, float weight)
    {
        return new Competitor
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Competitor",
            Gender = gender,
            DateOfBirth = DateTime.Now.AddYears(-age),
            Weight = weight,
            Subject = "TestSubject"
        };
    }
}
