using Chaos.Common.Utilities;
using FluentAssertions;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class RandomizerTests
{
    [Fact]
    public void PickRandom_ShouldBeAbleToReturnAnyElementFromCollection()
    {
        var sampleList = new List<int>
        {
            1,
            2,
            3,
            4,
            5
        };
        var pickedItems = new HashSet<int>();

        // Pick random 1000 times
        for (var i = 0; i < 2000; i++)
        {
            var randomItem = sampleList.PickRandom();
            pickedItems.Add(randomItem);
        }

        pickedItems.Should()
                   .BeEquivalentTo(sampleList);
    }

    [Fact]
    public void PickRandom_ShouldReturnElementFromCollection()
    {
        var sampleList = new List<int>
        {
            1,
            2,
            3,
            4,
            5
        };

        // Pick random 50 times
        for (var i = 0; i < 50; i++)
        {
            var randomItem = sampleList.PickRandom();

            sampleList.Should()
                      .Contain(randomItem);
        }
    }

    [Fact]
    public void PickRandomWeighted_DecimalWeights_ReturnsElementsInProportionalDistribution()
    {
        // Arrange
        var collection = new List<KeyValuePair<string, decimal>>
        {
            new("A", 0.1m),
            new("B", 0.2m),
            new("C", 0.7m)
        };

        var results = new Dictionary<string, int>();
        const int TRIALS = 10000;

        // Act
        for (var i = 0; i < TRIALS; i++)
        {
            var result = collection.PickRandomWeighted();
            results.TryAdd(result, 0);

            results[result]++;
        }

        // Assert
        var expectedDistribution = new Dictionary<string, double>
        {
            {
                "A", 0.1
            },
            {
                "B", 0.2
            },
            {
                "C", 0.7
            }
        };

        foreach (var pair in results)
        {
            var actualRatio = pair.Value / (double)TRIALS;

            actualRatio.Should()
                       .BeApproximately(expectedDistribution[pair.Key], 0.1); // 10% tolerance
        }
    }

    [Fact]
    public void PickRandomWeighted_IEnumerableOfChoicesAndWeights_ReturnsElementsInProportionalDistribution()
    {
        // Arrange
        var choices = new List<char>
        {
            'A',
            'B',
            'C'
        };

        var weights = new List<int>
        {
            1,
            2,
            7
        };

        var results = new Dictionary<char, int>();
        const int TRIALS = 10000;

        // Act
        for (var i = 0; i < TRIALS; i++)
        {
            var result = choices.PickRandomWeighted(weights);
            results.TryAdd(result, 0);

            results[result]++;
        }

        // Assert
        var expectedDistribution = new Dictionary<char, double>
        {
            {
                'A', 0.1
            },
            {
                'B', 0.2
            },
            {
                'C', 0.7
            }
        };

        foreach (var pair in results)
        {
            var actualRatio = pair.Value / (double)TRIALS;

            actualRatio.Should()
                       .BeApproximately(expectedDistribution[pair.Key], 0.1); // 10% tolerance
        }
    }

    [Fact]
    public void PickRandomWeighted_IntWeights_ReturnsElementsInProportionalDistribution()
    {
        // Arrange
        var collection = new List<KeyValuePair<int, int>>
        {
            new(1, 1),
            new(2, 2),
            new(3, 7)
        };

        var results = new Dictionary<int, int>();
        const int TRIALS = 10000;

        // Act
        for (var i = 0; i < TRIALS; i++)
        {
            var result = collection.PickRandomWeighted();
            results.TryAdd(result, 0);

            results[result]++;
        }

        // Assert
        var expectedDistribution = new Dictionary<int, double>
        {
            {
                1, 0.1
            },
            {
                2, 0.2
            },
            {
                3, 0.7
            }
        };

        foreach (var pair in results)
        {
            var actualRatio = pair.Value / (double)TRIALS;

            actualRatio.Should()
                       .BeApproximately(expectedDistribution[pair.Key], 0.1); // 10% tolerance
        }
    }
}