using FluentAssertions;
using NUnit.Framework;
using TournamentBracket.Domain.Brackets.Helpers;

namespace TournamentBracket.Domain.Tests.HelpersSpecs;

[TestFixture]
public class BracketsHelpersSpec
{
	[Test]
	[TestCase(1, new[] { 1 })]
	[TestCase(2, new[] { 1, 2 })]
	[TestCase(4, new[] { 1, 4, 2, 3 })]
	[TestCase(8, new[] { 1, 8, 4, 5, 2, 7, 3, 6 })]
	public void GetSeed_PowerOfTwo_Should_ReturnCorrectSeed(int competitorsCount, int[] expected)
	{
		// Act
		var result = BracketsHelpers.GetSeed(competitorsCount);

		// Assert
		result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
	}

	[Test]
	public void GetSeed_NotPowerOfTwo_Should_RoundUpToNearestPowerOfTwo()
	{
		// Act
		var result = BracketsHelpers.GetSeed(3);

		// Assert
		result.Should().HaveCount(4);
	}

	[Test]
	[TestCase(3)]
	[TestCase(5)]
	[TestCase(6)]
	[TestCase(7)]
	[TestCase(9)]
	[TestCase(10)]
	public void GetSeed_NotPowerOfTwo_Should_ReturnPowerOfTwoSize(int competitorsCount)
	{
		// Act
		var result = BracketsHelpers.GetSeed(competitorsCount);

		// Assert
		var expectedSize = BracketsHelpers.NearestPowerOfTwo(competitorsCount);
		result.Should().HaveCount(expectedSize);
	}

	[Test]
	public void GetSeed_OneCompetitor_Should_ReturnSingleSeed()
	{
		// Act
		var result = BracketsHelpers.GetSeed(1);

		// Assert
		result.Should().BeEquivalentTo(new[] { 1 });
	}

	[Test]
	public void GetSeed_TwoCompetitors_Should_ReturnCorrectSeed()
	{
		// Act
		var result = BracketsHelpers.GetSeed(2);

		// Assert
		result.Should().BeEquivalentTo(new[] { 1, 2 });
	}

	[Test]
	public void GetSeed_FiveCompetitors_Should_ReturnSeedOfSizeEight()
	{
		// Act
		var result = BracketsHelpers.GetSeed(5);

		// Assert
		result.Should().HaveCount(8);
		result.Should().Contain(1);
		result.Should().Contain(8);
	}

	[Test]
	public void NearestPowerOfTwo_AlreadyPowerOfTwo_Should_ReturnSameValue()
	{
		// Act & Assert
		BracketsHelpers.NearestPowerOfTwo(1).Should().Be(1);
		BracketsHelpers.NearestPowerOfTwo(2).Should().Be(2);
		BracketsHelpers.NearestPowerOfTwo(4).Should().Be(4);
		BracketsHelpers.NearestPowerOfTwo(8).Should().Be(8);
		BracketsHelpers.NearestPowerOfTwo(16).Should().Be(16);
		BracketsHelpers.NearestPowerOfTwo(32).Should().Be(32);
	}

	[Test]
	[TestCase(3, 4)]
	[TestCase(5, 8)]
	[TestCase(6, 8)]
	[TestCase(7, 8)]
	[TestCase(9, 16)]
	[TestCase(10, 16)]
	[TestCase(15, 16)]
	[TestCase(17, 32)]
	public void NearestPowerOfTwo_NotPowerOfTwo_Should_RoundUp(int value, int expected)
	{
		// Act
		var result = BracketsHelpers.NearestPowerOfTwo(value);

		// Assert
		result.Should().Be(expected);
	}

	[Test]
	public void NearestPowerOfTwo_Zero_Should_ReturnOne()
	{
		// Act
		var result = BracketsHelpers.NearestPowerOfTwo(0);

		// Assert
		result.Should().Be(1);
	}

	[Test]
	public void NearestPowerOfTwo_One_Should_ReturnOne()
	{
		// Act
		var result = BracketsHelpers.NearestPowerOfTwo(1);

		// Assert
		result.Should().Be(1);
	}

	[Test]
	public void NearestPowerOfTwo_LargeValue_Should_ReturnCorrectPowerOfTwo()
	{
		// Act
		var result = BracketsHelpers.NearestPowerOfTwo(100);

		// Assert
		result.Should().Be(128);
	}

	[Test]
	public void NearestPowerOfTwo_ExactPowerOfTwo_Should_NotChange()
	{
		// Arrange
		var value = 64;

		// Act
		var result = BracketsHelpers.NearestPowerOfTwo(value);

		// Assert
		result.Should().Be(value);
	}

	[Test]
	public void IsPowerOfTwo_PowerOfTwoValues_Should_ReturnTrue()
	{
		// Note: IsPowerOfTwo is private, but we can test it indirectly through NearestPowerOfTwo
		BracketsHelpers.NearestPowerOfTwo(2).Should().Be(2);
		BracketsHelpers.NearestPowerOfTwo(4).Should().Be(4);
		BracketsHelpers.NearestPowerOfTwo(8).Should().Be(8);
		BracketsHelpers.NearestPowerOfTwo(16).Should().Be(16);
	}

	[Test]
	public void IsPowerOfTwo_NotPowerOfTwoValues_Should_RoundUp()
	{
		// Note: IsPowerOfTwo is private, but we can test it indirectly through NearestPowerOfTwo
		BracketsHelpers.NearestPowerOfTwo(3).Should().Be(4);
		BracketsHelpers.NearestPowerOfTwo(5).Should().Be(8);
		BracketsHelpers.NearestPowerOfTwo(6).Should().Be(8);
	}

	[Test]
	public void GetSeed_SeedValues_Should_BeValid()
	{
		// Arrange
		var competitorsCount = 8;

		// Act
		var result = BracketsHelpers.GetSeed(competitorsCount);

		// Assert
		result.Should().OnlyContain(x => x >= 1 && x <= competitorsCount);
	}

	[Test]
	public void GetSeed_SeedValues_Should_NotContainDuplicates()
	{
		// Arrange
		var competitorsCount = 8;

		// Act
		var result = BracketsHelpers.GetSeed(competitorsCount);

		// Assert
		result.Should().OnlyHaveUniqueItems();
	}

	[Test]
	public void GetSeed_LargeCompetitorsCount_Should_WorkCorrectly()
	{
		// Act
		var result = BracketsHelpers.GetSeed(16);

		// Assert
		result.Should().HaveCount(16);
		result.Should().OnlyContain(x => x >= 1 && x <= 16);
		result.Should().OnlyHaveUniqueItems();
	}

	[Test]
	public void NearestPowerOfTwo_VeryLargeValue_Should_WorkCorrectly()
	{
		// Act
		var result = BracketsHelpers.NearestPowerOfTwo(1000);

		// Assert
		result.Should().Be(1024);
	}

	[Test]
	public void GetSeed_SeedSum_Should_BeCorrect()
	{
		// Arrange
		var competitorsCount = 8;

		// Act
		var result = BracketsHelpers.GetSeed(competitorsCount);

		// Assert
		var sum = result.Sum();
		var expectedSum = (competitorsCount * (competitorsCount + 1)) / 2;
		sum.Should().Be(expectedSum);
	}
}
