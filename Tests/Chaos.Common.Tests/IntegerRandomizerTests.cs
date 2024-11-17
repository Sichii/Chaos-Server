#region
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using FluentAssertions;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Common.Tests;

public sealed class IntegerRandomizerTests
{
    [Test]
    public void PickRandomWeightedSingleOrDefault_ReturnsValidResult()
    {
        var weightedChoices = new List<KeyValuePair<string, int>>
        {
            new("Choice1", 10),
            new("Choice2", 90),
            new("Choice3", 20)
        };

        var choice1Count = 0;
        var choice2Count = 0;
        var choice3Count = 0;
        var nullCount = 0;

        for (var i = 0; i < 1000; i++)
        {
            var result = weightedChoices.PickRandomWeightedSingleOrDefault();

            switch (result)
            {
                case "Choice1":
                    choice1Count++;

                    break;
                case "Choice2":
                    choice2Count++;

                    break;
                case "Choice3":
                    choice3Count++;

                    break;
                default:
                    nullCount++;

                    break;
            }
        }

        // VerifySimpleLog that the result distribution roughly matches the weights
        choice1Count.Should()
                    .BeInRange(10, 100);

        choice2Count.Should()
                    .BeInRange(10, 900);

        choice3Count.Should()
                    .BeInRange(10, 200);

        nullCount.Should()
                 .BeGreaterThan(0);
    }

    [Test]
    [Arguments(100, 50)]
    [Arguments(50, 1000)]
    public void RollChance_ReturnsBoolean(int successChance, int testCount)
    {
        var successCount = 0;

        for (var i = 0; i < testCount; i++)
        {
            var result = IntegerRandomizer.RollChance(successChance);

            if (result)
                successCount++;
        }

        // The success rate should be close to the successChance. Allow a 10% error margin.
        (successCount * 100m / testCount).Should()
                                         .BeApproximately(successChance, 10);
    }

    [Test]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(500)]
    public void RollDouble_ReturnsNumberInRange(int maxPer)
    {
        for (var i = 0; i < 1000; i++)
        {
            var result = IntegerRandomizer.RollDouble(maxPer);

            result.Should()
                  .BeGreaterThan(0)
                  .And
                  .BeLessOrEqualTo(maxPer * 2);
        }
    }

    [Test]
    [Arguments(100, 10, 90)]
    [Arguments(100, 50, 50)]
    [Arguments(100, 100, 0)]
    public void RollRange_NegativeRandomization_ReturnsWithinExpectedRange(int baseValue, int variancePct, int expectedMin)
    {
        // Repeat the test 1000 times to make sure we cover as many random values as possible
        for (var i = 0; i < 1000; i++)
        {
            var result = IntegerRandomizer.RollRange(baseValue, variancePct, RandomizationType.Negative);

            result.Should()
                  .BeGreaterOrEqualTo(expectedMin)
                  .And
                  .BeLessOrEqualTo(baseValue);
        }
    }

    [Test]
    [Arguments(100, 10, 110)]
    [Arguments(100, 50, 150)]
    [Arguments(100, 100, 200)]
    public void RollRange_PositiveRandomization_ReturnsWithinExpectedRange(int baseValue, int variancePct, int expectedMax)
    {
        // Repeat the test 1000 times to make sure we cover as many random values as possible
        for (var i = 0; i < 1000; i++)
        {
            var result = IntegerRandomizer.RollRange(baseValue, variancePct, RandomizationType.Positive);

            result.Should()
                  .BeGreaterOrEqualTo(baseValue)
                  .And
                  .BeLessOrEqualTo(expectedMax);
        }
    }

    [Test]
    [Arguments(100, 10, 90)]
    [Arguments(100, 50, 50)]
    [Arguments(100, 100, 0)]
    public void RollRangeLong_NegativeRandomization_ReturnsWithinExpectedRange(long baseValue, int variancePct, long expectedMin)
    {
        // Repeat the test 1000 times to make sure we cover as many random values as possible
        for (var i = 0; i < 1000; i++)
        {
            var result = IntegerRandomizer.RollRange(baseValue, variancePct, RandomizationType.Negative);

            result.Should()
                  .BeGreaterOrEqualTo(expectedMin)
                  .And
                  .BeLessOrEqualTo(baseValue);
        }
    }

    [Test]
    [Arguments(100, 10, 110)]
    [Arguments(100, 50, 150)]
    [Arguments(100, 100, 200)]
    public void RollRangeLong_PositiveRandomization_ReturnsWithinExpectedRange(long baseValue, int variancePct, long expectedMax)
    {
        // Repeat the test 1000 times to make sure we cover as many random values as possible
        for (var i = 0; i < 1000; i++)
        {
            var result = IntegerRandomizer.RollRange(baseValue, variancePct, RandomizationType.Positive);

            result.Should()
                  .BeGreaterOrEqualTo(baseValue)
                  .And
                  .BeLessOrEqualTo(expectedMax);
        }
    }

    [Test]
    [Arguments(100)]
    [Arguments(200)]
    [Arguments(500)]
    public void RollSingle_ReturnsNumberInRange(int max)
    {
        for (var i = 0; i < 1000; i++)
        {
            var result = IntegerRandomizer.RollSingle(max);

            result.Should()
                  .BeGreaterThan(0)
                  .And
                  .BeLessOrEqualTo(max);
        }
    }
}