using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Brackets;
using TournamentBracket.Domain.Brackets.RoundRobinBracket;
using TournamentBracket.Domain.Brackets.SingleEliminationBracket;
using TournamentBracket.Domain.Matches;

namespace TournamentBracket.Domain.Tests.Brackets;

[TestFixture]
public class BracketFactoryResolverSpec
{
	private BracketFactoryResolver bracketFactoryResolver;
	private BracketTypeResolver bracketTypeResolver;
	private SingleEliminationBracketFactory singleEliminationBracketFactory;
	private RoundRobinBracketFactory roundRobinBracketFactory;
	private BracketNodeFactory bracketNodeFactory;
	private MatchFactory matchFactory;

	[SetUp]
	public void Setup()
	{
		bracketTypeResolver = new BracketTypeResolver();
		bracketNodeFactory = new BracketNodeFactory();
		matchFactory = new MatchFactory();
		singleEliminationBracketFactory = new SingleEliminationBracketFactory(bracketNodeFactory, matchFactory);
		roundRobinBracketFactory = new RoundRobinBracketFactory(matchFactory);
		bracketFactoryResolver = new BracketFactoryResolver(bracketTypeResolver, singleEliminationBracketFactory, roundRobinBracketFactory);
	}

	[Test]
	[TestCase(1)]
	[TestCase(2)]
	[TestCase(3)]
	public void ResolveByCompetitorsCount_WithOneToThreeCompetitors_Should_ReturnRoundRobinBracketFactory(int competitorsCount)
	{
		// Act
		var result = bracketFactoryResolver.ResolveByCompetitorsCount(competitorsCount);

		// Assert
		result.Should().BeOfType<RoundRobinBracketFactory>();
	}

	[Test]
	[TestCase(4)]
	[TestCase(5)]
	[TestCase(8)]
	[TestCase(10)]
	[TestCase(16)]
	public void ResolveByCompetitorsCount_WithFourOrMoreCompetitors_Should_ReturnSingleEliminationBracketFactory(int competitorsCount)
	{
		// Act
		var result = bracketFactoryResolver.ResolveByCompetitorsCount(competitorsCount);

		// Assert
		result.Should().BeOfType<SingleEliminationBracketFactory>();
	}

	[Test]
	[TestCase(BracketType.SingleElimination)]
	public void ResolveByBracketType_Should_ReturnSingleEliminationBracketFactory(BracketType bracketType)
	{
		// Act
		var result = bracketFactoryResolver.ResolveByBracketType(bracketType);

		// Assert
		result.Should().BeOfType<SingleEliminationBracketFactory>();
	}

	[Test]
	[TestCase(BracketType.RoundRobin)]
	public void ResolveByBracketType_Should_ReturnRoundRobinBracketFactory(BracketType bracketType)
	{
		// Act
		var result = bracketFactoryResolver.ResolveByBracketType(bracketType);

		// Assert
		result.Should().BeOfType<RoundRobinBracketFactory>();
	}

	[Test]
	public void ResolveByBracketType_WithNotImplementedType_Should_ThrowNotImplementedException()
	{
		// Arrange
		var notImplementedType = (BracketType)999;

		// Act
		Action act = () => bracketFactoryResolver.ResolveByBracketType(notImplementedType);

		// Assert
		act.Should().Throw<NotImplementedException>();
	}

	[Test]
	public void ResolveByCompetitorsCount_MultipleCallsWithSameCount_Should_ReturnSameFactoryType()
	{
		// Arrange
		const int competitorsCount = 8;

		// Act
		var result1 = bracketFactoryResolver.ResolveByCompetitorsCount(competitorsCount);
		var result2 = bracketFactoryResolver.ResolveByCompetitorsCount(competitorsCount);

		// Assert
		result1.GetType().Should().Be(result2.GetType());
		result1.Should().BeOfType<SingleEliminationBracketFactory>();
	}

	[Test]
	public void ResolveByCompetitorsCount_MultipleCallsWithDifferentCounts_Should_ReturnCorrectFactoryTypes()
	{
		// Act
		var result1 = bracketFactoryResolver.ResolveByCompetitorsCount(2);
		var result2 = bracketFactoryResolver.ResolveByCompetitorsCount(8);
		var result3 = bracketFactoryResolver.ResolveByCompetitorsCount(16);

		// Assert
		result1.Should().BeOfType<RoundRobinBracketFactory>();
		result2.Should().BeOfType<SingleEliminationBracketFactory>();
		result3.Should().BeOfType<SingleEliminationBracketFactory>();
	}
}
