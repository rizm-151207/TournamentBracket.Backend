using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Divisions;

namespace TournamentBracket.Domain.Tests.Divisions;

[TestFixture]
public class DivisionsFactorySpec
{
    private DivisionsFactory factory;
    private Guid competitionId;

    [SetUp]
    public void Setup()
    {
        factory = new DivisionsFactory();
        competitionId = Guid.NewGuid();
    }

    [Test]
    public void CreateDefaultDivisions_Should_ReturnNonEmptyList()
    {
        // Act
        var divisions = factory.CreateDefaultDivisions(competitionId);

        // Assert
        divisions.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void CreateDefaultDivisions_Should_CreateDivisionsForAllAgeGroups()
    {
        // Act
        var divisions = factory.CreateDefaultDivisions(competitionId);

        // Assert
        divisions.Should().HaveCountGreaterThan(10);
    }

    [Test]
    public void CreateDefaultDivisions_Should_CreateDivisionsForBothGenders()
    {
        // Act
        var divisions = factory.CreateDefaultDivisions(competitionId);

        // Assert
        divisions.Should().Contain(d => d.Gender == true);
        divisions.Should().Contain(d => d.Gender == false);
    }

    [Test]
    public void CreateDefaultDivisions_Should_SetCorrectCompetitionId()
    {
        // Act
        var divisions = factory.CreateDefaultDivisions(competitionId);

        // Assert
        divisions.Should().OnlyContain(d => d.CompetitionId == competitionId);
    }

    [Test]
    public void CreateDefaultDivisions_Should_CreateDivisionForGirlsUnderMinAge()
    {
        // Act
        var divisions = factory.CreateDefaultDivisions(competitionId);

        // Assert
        divisions.Should().Contain(d => !d.Gender && d.MaxAge == Division.MinDivisionAge - 1);
    }

    [Test]
    public void CreateDefaultDivisions_Should_CreateDivisionForBoysUnderMinAge()
    {
        // Act
        var divisions = factory.CreateDefaultDivisions(competitionId);

        // Assert
        divisions.Should().Contain(d => d.Gender && d.MaxAge == Division.MinDivisionAge - 1);
    }

    [Test]
    public void CreateDefaultDivisions_Should_CreateKidDivisionsWithCorrectAgeSteps()
    {
        // Act
        var divisions = factory.CreateDefaultDivisions(competitionId);

        // Assert
        for (var min = Division.MinDivisionAge; min < Division.MaxKidAge; min += Division.KidAgeStep)
        {
            var max = min + Division.KidAgeStep - 1;
            divisions.Should().Contain(d => d.MinAge == min && d.MaxAge == max);
        }
    }

    [Test]
    public void CreateDefaultDivisions_Should_CreateAdultDivisions()
    {
        // Act
        var divisions = factory.CreateDefaultDivisions(competitionId);

        // Assert
        divisions.Should().Contain(d => d.MinAge == Division.MaxKidAge && d.MaxAge == Division.VeteranAge - 1);
    }

    [Test]
    public void CreateDefaultDivisions_Should_CreateVeteranDivisions()
    {
        // Act
        var divisions = factory.CreateDefaultDivisions(competitionId);

        // Assert
        divisions.Should().Contain(d => d.MinAge == Division.VeteranAge);
    }

    [Test]
    public void CreateDefaultDivisions_Should_CreateEqualNumberOfDivisionsForMenAndWomen()
    {
        // Act
        var divisions = factory.CreateDefaultDivisions(competitionId);

        // Assert
        var menDivisions = divisions.Count(d => d.Gender);
        var womenDivisions = divisions.Count(d => !d.Gender);
        menDivisions.Should().Be(womenDivisions);
    }

    [Test]
    public void CreateDefaultDivisions_Should_CreateDivisionsWithNames()
    {
        // Act
        var divisions = factory.CreateDefaultDivisions(competitionId);

        // Assert
        divisions.Should().OnlyContain(d => !string.IsNullOrEmpty(d.Name));
    }

    [Test]
    public void CreateDefaultDivisions_MultipleCalls_Should_CreateIndependentInstances()
    {
        // Arrange
        var competitionId2 = Guid.NewGuid();

        // Act
        var divisions1 = factory.CreateDefaultDivisions(competitionId);
        var divisions2 = factory.CreateDefaultDivisions(competitionId2);

        // Assert
        divisions1.Should().HaveSameCount(divisions2);
        divisions1.Should().OnlyContain(d => d.CompetitionId == competitionId);
        divisions2.Should().OnlyContain(d => d.CompetitionId == competitionId2);
    }

    [Test]
    public void CreateDefaultDivisions_Should_CreateDivisionsWithValidAgeRanges()
    {
        // Act
        var divisions = factory.CreateDefaultDivisions(competitionId);

        // Assert
        foreach (var division in divisions)
        {
            if (division.MinAge.HasValue && division.MaxAge.HasValue)
            {
                division.MinAge.Value.Should().BeLessThanOrEqualTo(division.MaxAge.Value);
            }
        }
    }

    [Test]
    public void CreateDefaultDivisions_Should_CoverAllAgeGroupsFromZeroToVeteran()
    {
        // Act
        var divisions = factory.CreateDefaultDivisions(competitionId);

        // Assert
        divisions.Should().Contain(d => d.MaxAge < Division.MinDivisionAge);
        divisions.Should().Contain(d => d.MinAge >= Division.MinDivisionAge && d.MaxAge < Division.MaxKidAge);
        divisions.Should().Contain(d => d.MinAge >= Division.MaxKidAge && d.MaxAge < Division.VeteranAge);
        divisions.Should().Contain(d => d.MinAge >= Division.VeteranAge);
    }

    [Test]
    public void CreateDefaultDivisions_Should_CreateDivisionsWithInitializedCompetitorsList()
    {
        // Act
        var divisions = factory.CreateDefaultDivisions(competitionId);

        // Assert
        divisions.Should().OnlyContain(d => d.Competitors != null);
        divisions.Should().OnlyContain(d => d.Competitors.Count == 0);
    }
}
