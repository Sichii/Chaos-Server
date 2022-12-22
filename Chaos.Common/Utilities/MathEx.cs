using System.Numerics;

namespace Chaos.Common.Utilities;

public static class MathEx
{
    public static TNumber CalculatePercent<TNumber>(int current, int max) where TNumber: INumber<TNumber> =>
        (TNumber)Convert.ChangeType(current / (decimal)max * 100.0m, typeof(TNumber));

    public static TNumber GetPercentOf<TNumber>(int max, decimal percent) where TNumber: INumber<TNumber> =>
        (TNumber)Convert.ChangeType(max / 100.0m * percent, typeof(TNumber));

    public static double ScaleRange(
        double num,
        double min,
        double max,
        double newMin,
        double newMax
    ) => (newMax - newMin) * (num - min) / (max - min) + newMin;
}