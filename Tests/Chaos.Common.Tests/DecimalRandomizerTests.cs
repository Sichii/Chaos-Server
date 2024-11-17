#region
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using FluentAssertions;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Common.Tests;

public sealed class DecimalRandomizerTests
{
    [Test]
    public void PickRandomWeightedSingleOrDefault_ChoicesAndWeights_ShouldOccasionallyReturnNull()
    {
        var choices = new List<string>
        {
            "A",
            "B"
        };

        var weights = new List<decimal>
        {
            0.5m,
            0.5m
        };

        var nullCount = 0;

        for (var i = 0; i < 10000; i++)
        {
            var choice = choices.PickRandomWeightedSingleOrDefault(weights);

            if (choice == null)
                nullCount++;
        }

        // Again, the exact number isn't too important.
        nullCount.Should()
                 .BeInRange(1, 5000);
    }

    [Test]
    public void PickRandomWeightedSingleOrDefault_WeightedChoices_ShouldOccasionallyReturnNull()
    {
        var weightedChoices = new List<KeyValuePair<string, decimal>>
        {
            new("A", 0.5m),
            new("B", 0.5m)
        };

        var nullCount = 0;

        for (var i = 0; i < 10000; i++)
        {
            var choice = weightedChoices.PickRandomWeightedSingleOrDefault();

            if (choice == null)
                nullCount++;
        }

        // The exact number here isn't too important; the point is that we expect 
        // some null values given enough iterations, but not too many.
        nullCount.Should()
                 .BeInRange(1, 5000);
    }

    [Test]
    public void RollChance_GivenChance_ShouldReturnTrueAtLeastOnce()
    {
        var success = false;

        for (var i = 0; i < 1000; i++)
        {
            success = DecimalRandomizer.RollChance(0.1m);

            if (success)
                break;
        }

        success.Should()
               .BeTrue();
    }

    [Test]
    public void RollRange_GivenBaseAndVariance_ShouldReturnNumberWithinRange()
    {
        const decimal BASE_VALUE = 100m;
        const decimal VARIANCE_PCT = 0.2m;

        for (var i = 0; i < 1000; i++)
        {
            var roll = DecimalRandomizer.RollRange(BASE_VALUE, VARIANCE_PCT, RandomizationType.Balanced);

            roll.Should()
                .BeInRange(BASE_VALUE - VARIANCE_PCT * BASE_VALUE / 2, BASE_VALUE + VARIANCE_PCT * BASE_VALUE / 2);
        }
    }

    [Test]
    [Arguments(100, 0.1, 90)]
    [Arguments(100, 0.5, 50)]
    [Arguments(100, 1, 0)]
    public void RollRange_NegativeRandomization_ReturnsWithinExpectedRange(decimal baseValue, decimal variancePct, decimal expectedMin)
    {
        // Repeat the test 1000 times to make sure we cover as many random values as possible
        for (var i = 0; i < 1000; i++)
        {
            var result = DecimalRandomizer.RollRange(baseValue, variancePct, RandomizationType.Negative);

            result.Should()
                  .BeInRange(expectedMin, baseValue);
        }
    }

    [Test]
    [Arguments(100, 0.1, 110)]
    [Arguments(100, 0.5, 150)]
    [Arguments(100, 1, 200)]
    public void RollRange_PositiveRandomization_ReturnsWithinExpectedRange(decimal baseValue, decimal variancePct, decimal expectedMax)
    {
        // Repeat the test 1000 times to make sure we cover as many random values as possible
        for (var i = 0; i < 1000; i++)
        {
            var result = DecimalRandomizer.RollRange(baseValue, variancePct, RandomizationType.Positive);

            result.Should()
                  .BeInRange(baseValue, expectedMax);
        }
    }
}