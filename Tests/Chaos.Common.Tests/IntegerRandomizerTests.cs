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
    public void PickRandomWeightedSingleOrDefault_Empty_ReturnsDefault()
    {
        var empty = new List<KeyValuePair<string, int>>();
        var res = empty.PickRandomWeightedSingleOrDefault();

        res.Should()
           .BeNull();
    }

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

        // Smoke-check: results are plausible and at least one selection occurred
        (choice1Count + choice2Count + choice3Count + nullCount).Should()
                                                                .Be(1000);

        (choice1Count + choice2Count + choice3Count).Should()
                                                    .BeGreaterThan(0);
    }

    [Test]
    public void PickRandomWeightedSingleOrDefault_WithCommonWeight_ShouldWork()
    {
        // Arrange
        var choices = new[]
        {
            "A",
            "B",
            "C"
        };
        var commonWeight = 50;
        var results = new Dictionary<string, int>();
        var nullCount = 0;

        // Act
        for (var i = 0; i < 1000; i++)
        {
            var result = choices.PickRandomWeightedSingleOrDefault(commonWeight);

            if (result == null)
                nullCount++;
            else if (results.ContainsKey(result))
                results[result]++;
            else
                results[result] = 1;
        }

        // Assert
        results.Should()
               .ContainKey("A")
               .WhoseValue
               .Should()
               .BeGreaterThan(0);

        results.Should()
               .ContainKey("B")
               .WhoseValue
               .Should()
               .BeGreaterThan(0);

        results.Should()
               .ContainKey("C")
               .WhoseValue
               .Should()
               .BeGreaterThan(0);
    }

    [Test]
    public void PickRandomWeightedSingleOrDefault_WithHighWeights_ShouldAlmostAlwaysSelect()
    {
        // Arrange - use very high weights so selection is almost guaranteed
        var weightedChoices = new List<KeyValuePair<string, int>>
        {
            new("Choice1", 99),
            new("Choice2", 99)
        };

        var successCount = 0;

        // Act
        for (var i = 0; i < 100; i++)
        {
            var result = weightedChoices.PickRandomWeightedSingleOrDefault();

            if (result != null)
                successCount++;
        }

        // Assert - with 99% weights, we should get selections in almost all cases
        successCount.Should()
                    .BeGreaterThan(95);
    }

    [Test]
    public void PickRandomWeightedSingleOrDefault_WithLowWeights_ShouldOftenReturnNull()
    {
        // Arrange - use very low weights so selection is rare
        var weightedChoices = new List<KeyValuePair<string, int>>
        {
            new("Choice1", 1),
            new("Choice2", 1)
        };

        var nullCount = 0;

        // Act
        for (var i = 0; i < 1000; i++)
        {
            var result = weightedChoices.PickRandomWeightedSingleOrDefault();

            if (result == null)
                nullCount++;
        }

        // Assert - with 1% weights, we should get many nulls
        nullCount.Should()
                 .BeGreaterThan(800);
    }

    [Test]
    public void PickRandomWeightedSingleOrDefault_WithWeightsArray_ShouldWork()
    {
        // Arrange
        var choices = new[]
        {
            "A",
            "B",
            "C"
        };

        var weights = new[]
        {
            10,
            50,
            20
        };
        var results = new Dictionary<string, int>();
        var nullCount = 0;

        // Act
        for (var i = 0; i < 1000; i++)
        {
            var result = choices.PickRandomWeightedSingleOrDefault(weights);

            if (result == null)
                nullCount++;
            else if (results.ContainsKey(result))
                results[result]++;
            else
                results[result] = 1;
        }

        // Assert
        results.Should()
               .ContainKey("A")
               .WhoseValue
               .Should()
               .BeGreaterThan(0);

        results.Should()
               .ContainKey("B")
               .WhoseValue
               .Should()
               .BeGreaterThan(0);

        results.Should()
               .ContainKey("C")
               .WhoseValue
               .Should()
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
    public void RollChance_WithFullChance_ShouldAlwaysReturnTrue()
    {
        // Act & Assert
        for (var i = 0; i < 100; i++)
            IntegerRandomizer.RollChance(100)
                             .Should()
                             .BeTrue();
    }

    [Test]
    public void RollChance_WithZeroChance_ShouldAlwaysReturnFalse()
    {
        // Act & Assert
        for (var i = 0; i < 100; i++)
            IntegerRandomizer.RollChance(0)
                             .Should()
                             .BeFalse();
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
                  .BeLessThanOrEqualTo(maxPer * 2);
        }
    }

    [Test]
    public void RollDouble_WithMaxOne_ShouldReturnTwo()
    {
        // Act & Assert
        for (var i = 0; i < 100; i++)
            IntegerRandomizer.RollDouble(1)
                             .Should()
                             .Be(2);
    }

    [Test]
    [Arguments(
        100,
        10,
        90,
        110)]
    [Arguments(
        100,
        50,
        50,
        150)]
    [Arguments(
        100,
        100,
        0,
        200)]
    public void RollRange_BalancedRandomization_ReturnsWithinExpectedRange(
        int baseValue,
        int variancePct,
        int expectedMin,
        int expectedMax)
    {
        // Repeat the test 1000 times to make sure we cover as many random values as possible
        for (var i = 0; i < 1000; i++)
        {
            var result = IntegerRandomizer.RollRange(baseValue, variancePct, RandomizationType.Balanced);

            result.Should()
                  .BeGreaterThanOrEqualTo(expectedMin)
                  .And
                  .BeLessThanOrEqualTo(expectedMax);
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
                  .BeGreaterThanOrEqualTo(expectedMin)
                  .And
                  .BeLessThanOrEqualTo(baseValue);
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
                  .BeGreaterThanOrEqualTo(baseValue)
                  .And
                  .BeLessThanOrEqualTo(expectedMax);
        }
    }

    [Test]
    public void RollRange_WithInvalidRandomizationType_ShouldThrowException()
    {
        // Act & Assert
        var act = () => IntegerRandomizer.RollRange(100, 10, (RandomizationType)999);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void RollRange_WithZeroVariance_ShouldReturnBaseValue()
    {
        // Arrange
        var baseValue = 100;
        var variancePct = 0;

        // Act & Assert
        for (var i = 0; i < 100; i++)
        {
            IntegerRandomizer.RollRange(baseValue, variancePct, RandomizationType.Positive)
                             .Should()
                             .Be(baseValue);

            IntegerRandomizer.RollRange(baseValue, variancePct, RandomizationType.Negative)
                             .Should()
                             .Be(baseValue);

            IntegerRandomizer.RollRange(baseValue, variancePct, RandomizationType.Balanced)
                             .Should()
                             .Be(baseValue);
        }
    }

    [Test]
    [Arguments(
        100,
        10,
        90,
        110)]
    [Arguments(
        100,
        50,
        50,
        150)]
    [Arguments(
        100,
        100,
        0,
        200)]
    public void RollRangeLong_BalancedRandomization_ReturnsWithinExpectedRange(
        long baseValue,
        int variancePct,
        long expectedMin,
        long expectedMax)
    {
        // Repeat the test 1000 times to make sure we cover as many random values as possible
        for (var i = 0; i < 1000; i++)
        {
            var result = IntegerRandomizer.RollRange(baseValue, variancePct, RandomizationType.Balanced);

            result.Should()
                  .BeGreaterThanOrEqualTo(expectedMin)
                  .And
                  .BeLessThanOrEqualTo(expectedMax);
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
                  .BeGreaterThanOrEqualTo(expectedMin)
                  .And
                  .BeLessThanOrEqualTo(baseValue);
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
                  .BeGreaterThanOrEqualTo(baseValue)
                  .And
                  .BeLessThanOrEqualTo(expectedMax);
        }
    }

    [Test]
    public void RollRangeLong_WithInvalidRandomizationType_ShouldThrowException()
    {
        // Act & Assert
        var act = () => IntegerRandomizer.RollRange(100L, 10, (RandomizationType)999);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void RollRangeLong_WithZeroVariance_ShouldReturnBaseValue()
    {
        // Arrange
        var baseValue = 100L;
        var variancePct = 0;

        // Act & Assert
        for (var i = 0; i < 100; i++)
        {
            IntegerRandomizer.RollRange(baseValue, variancePct, RandomizationType.Positive)
                             .Should()
                             .Be(baseValue);

            IntegerRandomizer.RollRange(baseValue, variancePct, RandomizationType.Negative)
                             .Should()
                             .Be(baseValue);

            IntegerRandomizer.RollRange(baseValue, variancePct, RandomizationType.Balanced)
                             .Should()
                             .Be(baseValue);
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
                  .BeLessThanOrEqualTo(max);
        }
    }

    [Test]
    public void RollSingle_Throws_When_Max_Less_Than_One()
    {
        Action act = () => IntegerRandomizer.RollSingle(0);

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
    public void RollSingle_WithMaxOne_ShouldReturnOne()
    {
        // Act & Assert
        for (var i = 0; i < 100; i++)
            IntegerRandomizer.RollSingle(1)
                             .Should()
                             .Be(1);
    }
}