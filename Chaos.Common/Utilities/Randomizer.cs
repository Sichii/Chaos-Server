using System.Diagnostics;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;

namespace Chaos.Common.Utilities;

/// <summary>
///     A Utility class for generating random numbers
/// </summary>
public static class Randomizer
{
    public static T PickRandom<T>(this IReadOnlyCollection<T> objs) => objs.ElementAt(Random.Shared.Next(objs.Count));

    public static T PickRandom<T>(this IEnumerable<T> objs, IEnumerable<decimal> weights)
    {
        var localObjs = objs.ToArray();
        var localWeights = weights.ToArray();

        if (localObjs.Length != localWeights.Length)
            throw new ArgumentException($"{nameof(objs)} and {nameof(weights)} must have the same count");

        if (localObjs.Length == 0)
            throw new ArgumentException("Arguments must contains more than 0 elements");

        Array.Sort(localWeights, localObjs);

        var accumulator = 0m;

        foreach (ref var weight in localWeights.AsSpan())
        {
            accumulator += weight;
            weight = accumulator;
        }

        var rand = Random.Shared.Next(accumulator);

        for (var i = 0; i < localWeights.Length; i++)
        {
            var weight = localWeights[i];

            if (rand < weight)
                return localObjs[i];
        }

        throw new UnreachableException("The loop that picks a random number should be exhaustive");
    }

    /// <summary>
    ///     Randomly determins if a roll is successful or not.
    /// </summary>
    public static bool RollChance(int successChance) => RollSingle(100) <= successChance;

    /// <summary>
    ///     Generates 2 random numbers between 1 and <paramref name="maxPer" /> and adds them together. Inclusive on both ends.
    /// </summary>
    public static int RollDouble(int maxPer) => RollSingle(maxPer) + RollSingle(maxPer);

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
    public static int RollSingle(int max) => Random.Shared.Next(1, max + 1);
}