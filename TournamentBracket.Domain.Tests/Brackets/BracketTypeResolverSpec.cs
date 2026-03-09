using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Brackets;

namespace TournamentBracket.Domain.Tests.Brackets;

[TestFixture]
public class BracketTypeResolverSpec
{
    private BracketTypeResolver _resolver;

    [SetUp]
    public void Setup()
    {
        _resolver = new BracketTypeResolver();
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    [TestCase(8)]
    [TestCase(10)]
    [TestCase(16)]
    [TestCase(32)]
    [TestCase(64)]
    [TestCase(100)]
    public void Resolve_AnyCompetitorsCount_Should_ReturnSingleElimination(int competitorsCount)
    {
        // Act
        var result = _resolver.Resolve(competitorsCount);

        // Assert
        result.Should().Be(BracketType.SingleElimination);
    }

    [Test]
    public void Resolve_WithZeroCompetitors_Should_ReturnSingleElimination()
    {
        // Act
        var result = _resolver.Resolve(0);

        // Assert
        result.Should().Be(BracketType.SingleElimination);
    }

    [Test]
    public void Resolve_WithLargeNumberOfCompetitors_Should_ReturnSingleElimination()
    {
        // Act
        var result = _resolver.Resolve(1000);

        // Assert
        result.Should().Be(BracketType.SingleElimination);
    }
}
