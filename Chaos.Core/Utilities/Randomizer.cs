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

    /// <summary>
    ///     Generates a random number between 1 and <paramref name="max" />. Inclusive on both ends.
    /// </summary>
    public static int RollSingle(int max) => Random.Shared.Next(1, max + 1);
}