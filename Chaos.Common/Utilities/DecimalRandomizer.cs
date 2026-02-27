#region
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
#endregion

namespace Chaos.Common.Utilities;

/// <summary>
///     A Utility class for operations involving random DECIMALS
/// </summary>
public static class DecimalRandomizer
{
    /// <summary>
    ///     Picks a random choice based on the weights. The higher the weight, the more likely it is to be picked. Chances are
    ///     NOT exhaustive. Only picks at most 1 item.
    /// </summary>
    /// <param name="weightedChoices">
    ///     A collection of choices with their corresponding weights
    /// </param>
    /// <typeparam name="T">
    ///     The type of object to return
    /// </typeparam>
    /// <returns>
    ///     A random element from the specified collection if a choice is taken, otherwise
    ///     <c>
    ///         default
    ///     </c>
    /// </returns>
    public static T? PickRandomWeightedSingleOrDefault<T>(this ICollection<KeyValuePair<T, decimal>> weightedChoices)
    {
        // Calculate the chance that any choice is taken
        var chanceOfNoSelection = weightedChoices.Aggregate(1.0m, (acc, item) => acc * (1.0m - item.Value));
        var chanceOfSelection = 1.0m - chanceOfNoSelection;

        //no choice
        if (!RollChance(chanceOfSelection))
            return default;

        return weightedChoices.PickRandomWeighted();
    }

    /// <summary>
    ///     Randomly determins if a roll is successful or not.
    /// </summary>
    public static bool RollChance(decimal successChance) => Random.Shared.Next<decimal>() < successChance;

    /// <summary>
    ///     Generates a random number within the specified range, applying the given randomization type.
    /// </summary>
    /// <param name="baseValue">
    ///     The base value of the range.
    /// </param>
    /// <param name="variancePct">
    ///     The percentage of variance allowed.
    /// </param>
    /// <param name="randomizationType">
    ///     The type of randomization to apply.
    /// </param>
    /// <returns>
    ///     A random number within the specified range, according to the randomization type.
    /// </returns>
    public static decimal RollRange(decimal baseValue, decimal variancePct, RandomizationType randomizationType)
    {
        var randomPct = Random.Shared.Next(0, variancePct);
        decimal applicablePct;

        switch (randomizationType)
        {
            case RandomizationType.Balanced:
            {
                var half = variancePct / 2;

                applicablePct = randomPct - half;

                break;
            }
            case RandomizationType.Positive:
            {
                applicablePct = randomPct;

                break;
            }
            case RandomizationType.Negative:
            {
                applicablePct = -randomPct;

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        var amountToAdd = baseValue * applicablePct;

        return baseValue + amountToAdd;
    }

    /// <param name="choices">
    ///     A collection of choiced
    /// </param>
    /// <typeparam name="T">
    ///     The type of object to return
    /// </typeparam>
    extension<T>(IEnumerable<T> choices)
    {
        /// <summary>
        ///     Picks a random choice based on the common weight. Chances are NOT exhaustive. Only picks at most 1 item. Each item
        ///     has the same chance to be picked.
        /// </summary>
        /// <param name="commonWeight">
        ///     The weight of each choice
        /// </param>
        /// <returns>
        ///     A random element from the specified collection if a choice is taken, otherwise
        ///     <c>
        ///         default
        ///     </c>
        /// </returns>
        public T? PickRandomWeightedSingleOrDefault(decimal commonWeight)
            => choices.Select(x => new KeyValuePair<T, decimal>(x, commonWeight))
                      .ToArray()
                      .PickRandomWeightedSingleOrDefault();

        /// <summary>
        ///     Picks a random choice based on the weights. The higher the weight, the more likely it is to be picked. Chances are
        ///     NOT exhaustive. Only picks at most 1 item.
        /// </summary>
        /// <param name="weights">
        ///     The weights of those choices
        /// </param>
        /// <returns>
        ///     A random element from the specified collection if a choice is taken, otherwise
        ///     <c>
        ///         default
        ///     </c>
        /// </returns>
        public T? PickRandomWeightedSingleOrDefault(IEnumerable<decimal> weights)
            => choices.Zip(weights, (choice, weight) => new KeyValuePair<T, decimal>(choice, weight))
                      .ToArray()
                      .PickRandomWeightedSingleOrDefault();
    }
}