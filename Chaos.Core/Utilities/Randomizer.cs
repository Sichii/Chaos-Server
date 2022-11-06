using Chaos.Core.Definitions;

namespace Chaos.Core.Utilities;

/// <summary>
///     A Utility class for generating random numbers
/// </summary>
public static class Randomizer
{
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