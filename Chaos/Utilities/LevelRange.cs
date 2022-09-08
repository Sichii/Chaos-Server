namespace Chaos.Utilities;

public static class LevelRange
{
    public static int GetUpperBound(int level) => (int)Math.Floor(level + level / 5m + 3);

    public static int GetLowerBound(int level) => (int)Math.Ceiling(Math.Max(0, level * 5m - 15) / 6);

    public static int GetRangeDifference(int level) => GetUpperBound(level) - GetLowerBound(level);

    public static bool WithinLevelRange(int left, int right)
    {
        var min = Math.Min(left, right);
        var max = Math.Max(left, right);
        
        return (min >= GetLowerBound(max)) && (max <= GetUpperBound(min));
    }

    public static double Scale(
        double num,
        double min,
        double max,
        double newMin,
        double newMax
    ) => (newMax - newMin) * (num - min) / (max - min) + newMin;
}