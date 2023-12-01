using System.Diagnostics.CodeAnalysis;
using Chaos.Common.Definitions;

namespace Chaos.Common.Utilities;

/// <summary>
///     A Utility class for operations involving random INTEGERS
/// </summary>
public static class IntegerRandomizer
{
    /// <summary>
    ///     Picks a random choice based on the weights.
    ///     The higher the weight, the more likely it is to be picked.
    ///     Chances are NOT exhaustive.
    ///     Only picks at most 1 item.
    /// </summary>
    /// <param name="weightedChoices">A collection of choices with their corresponding weights</param>
    /// <typeparam name="T">The type of object to return</typeparam>
    /// <returns>A random element from the specified collection if a choice is taken, otherwise <c>default</c></returns>
    public static T? PickRandomWeightedSingleOrDefault<T>(this ICollection<KeyValuePair<T, int>> weightedChoices)
    {
        // Calculate the chance that any choice is taken
        var chanceOfNoSelection = weightedChoices.Aggregate(100, (acc, item) => Convert.ToInt32(acc * (100 - item.Value) / 100m));
        var chanceOfSelection = 100 - chanceOfNoSelection;

        //no choice
        if (!RollChance(chanceOfSelection))
            return default;

        return weightedChoices.PickRandomWeighted();
    }

    /// <summary>
    ///     Picks a random choice based on the weights.
    ///     The higher the weight, the more likely it is to be picked.
    ///     Chances are NOT exhaustive.
    ///     Only picks at most 1 item.
    /// </summary>
    /// <param name="choices">The choices to choose from</param>
    /// <param name="weights">The weights of those choices</param>
    /// <typeparam name="T">The type of object to return</typeparam>
    /// <returns>A random element from the specified collection if a choice is taken, otherwise <c>default</c></returns>
    [ExcludeFromCodeCoverage(Justification = "Tested by PickRandomWeightedSingleOrDefault<T>(ICollection<KeyValuePair<T, int>>)")]
    public static T? PickRandomWeightedSingleOrDefault<T>(this IEnumerable<T> choices, IEnumerable<int> weights) =>
        choices.Zip(weights, (choice, weight) => new KeyValuePair<T, int>(choice, weight))
               .ToList()
               .PickRandomWeightedSingleOrDefault();

    /// <summary>
    ///     Randomly determins if a roll is successful or not.
    /// </summary>
    public static bool RollChance(int successChance) => RollSingle(100) <= successChance;

    /// <summary>
    ///     Generates 2 random numbers between 1 and <paramref name="maxPer" /> and adds them together. Inclusive on both ends.
    /// </summary>
    public static int RollDouble(int maxPer) => RollSingle(maxPer) + RollSingle(maxPer);

    /// <summary>
    ///     Generates a random number within the specified range, applying the given randomization type.
    /// </summary>
    /// <param name="baseValue">The base value of the range.</param>
    /// <param name="variancePct">The percentage of variance allowed.</param>
    /// <param name="randomizationType">The type of randomization to apply.</param>
    /// <returns>A random number within the specified range, according to the randomization type.</returns>
    public static int RollRange(int baseValue, int variancePct, RandomizationType randomizationType)
    {
        var randomPct = Random.Shared.Next(0, variancePct);
        decimal applicablePct;

        switch (randomizationType)
        {
            case RandomizationType.Balanced:
            {
                var half = variancePct / 2;

                applicablePct = (randomPct - half) / 100m;

                break;
            }
            case RandomizationType.Positive:
            {
                applicablePct = randomPct / 100m;

                break;
            }
            case RandomizationType.Negative:
            {
                applicablePct = -(randomPct / 100m);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        var amountToAdd = Convert.ToInt32(baseValue * applicablePct);

        return baseValue + amountToAdd;
    }

    /// <summary>
    ///     Generates a random number within the specified range, applying the given randomization type.
    /// </summary>
    /// <param name="baseValue">The base value of the range.</param>
    /// <param name="variancePct">The percentage of variance allowed.</param>
    /// <param name="randomizationType">The type of randomization to apply.</param>
    /// <returns>A random number within the specified range, according to the randomization type.</returns>
    public static long RollRange(long baseValue, int variancePct, RandomizationType randomizationType)
    {
        var randomPct = Random.Shared.Next(0, variancePct);
        decimal applicablePct;

        switch (randomizationType)
        {
            case RandomizationType.Balanced:
            {
                var half = variancePct / 2;

                applicablePct = (randomPct - half) / 100m;

                break;
            }
            case RandomizationType.Positive:
            {
                applicablePct = randomPct / 100m;

                break;
            }
            case RandomizationType.Negative:
            {
                applicablePct = -(randomPct / 100m);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        var amountToAdd = Convert.ToInt64(baseValue * applicablePct);

        return baseValue + amountToAdd;
    }

    /// <summary>
    ///     Generates a random number between 1 and <paramref name="max" />. Inclusive on both ends.
    /// </summary>
    public static int RollSingle(int max)
    {
        if (max < 1)
            throw new InvalidOperationException("Max must be greater than 1. This method is like simulating dice rolls.");

        return Random.Shared.Next(1, max + 1);
    }
}