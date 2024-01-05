using System.Diagnostics;
using Chaos.Extensions.Common;

namespace Chaos.Common.Utilities;

/// <summary>
///     A Utility class for operations involving random numbers
/// </summary>
public static class Randomizer
{
    /// <summary>
    ///     Picks a random element from the specified collection.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the elements in the collection.
    /// </typeparam>
    /// <param name="objs">
    ///     The collection to pick a random element from.
    /// </param>
    /// <returns>
    ///     A random element from the specified collection.
    /// </returns>
    public static T PickRandom<T>(this ICollection<T> objs) => objs.ElementAt(Random.Shared.Next(objs.Count));

    /// <summary>
    ///     Picks a random choice based on the weights. The higher the weight, the more likely it is to be picked. Chances are
    ///     exhaustive.
    /// </summary>
    /// <param name="weightedChoices">
    ///     A collection of choices with their corresponding weights
    /// </param>
    /// <typeparam name="T">
    ///     The type of object to return
    /// </typeparam>
    /// <returns>
    ///     A random element from the specified collection
    /// </returns>
    public static T PickRandomWeighted<T>(this ICollection<KeyValuePair<T, int>> weightedChoices)
    {
        var totalWeight = weightedChoices.Sum(x => x.Value);
        var randomWeight = Random.Shared.Next(0, totalWeight);
        var accumulator = 0;

        foreach ((var choice, var weight) in weightedChoices)
        {
            accumulator += weight;

            if (accumulator > randomWeight)
                return choice;
        }

        throw new UnreachableException("The loop that picks a random number should be exhaustive");
    }

    /// <summary>
    ///     Picks a random choice based on the weights. The higher the weight, the more likely it is to be picked. Chances are
    ///     exhaustive.
    /// </summary>
    /// <param name="weightedChoices">
    ///     A collection of choices with their corresponding weights
    /// </param>
    /// <typeparam name="T">
    ///     The type of object to return
    /// </typeparam>
    /// <returns>
    ///     A random element from the specified collection
    /// </returns>
    public static T PickRandomWeighted<T>(this ICollection<KeyValuePair<T, decimal>> weightedChoices)
    {
        var totalWeight = weightedChoices.Sum(x => x.Value);
        var randomWeight = Random.Shared.Next(0, totalWeight);
        var accumulator = 0.0m;

        foreach ((var choice, var weight) in weightedChoices)
        {
            accumulator += weight;

            if (accumulator > randomWeight)
                return choice;
        }

        throw new UnreachableException("The loop that picks a random number should be exhaustive");
    }

    /// <summary>
    ///     Picks a random choice based on the weights. The higher the weight, the more likely it is to be picked. Chances are
    ///     exhaustive.
    /// </summary>
    /// <param name="choices">
    ///     The choices to choose from
    /// </param>
    /// <param name="weights">
    ///     The weights of those choices
    /// </param>
    /// <typeparam name="T">
    ///     The type of object to return
    /// </typeparam>
    /// <returns>
    ///     A random element from the given choices
    /// </returns>
    public static T PickRandomWeighted<T>(this IEnumerable<T> choices, IEnumerable<int> weights)
        => choices.Zip(weights, (choice, weight) => new KeyValuePair<T, int>(choice, weight))
                  .ToList()
                  .PickRandomWeighted();
}