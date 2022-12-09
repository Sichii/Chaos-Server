namespace Chaos.Common.Utilities;

public static class MathEx
{
    public static double ScaleRange(
        double num,
        double min,
        double max,
        double newMin,
        double newMax
    ) => (newMax - newMin) * (num - min) / (max - min) + newMin;
}