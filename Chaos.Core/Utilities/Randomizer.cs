namespace Chaos.Core.Utilities;

public static class Randomizer
{
    public static int RollSingle(int max) => Random.Shared.Next(1, max + 1);
    public static int RollDouble(int maxPer) => RollSingle(maxPer) + RollSingle(maxPer);
    public static bool RollChance(int successChance) => RollSingle(100) <= successChance;
}