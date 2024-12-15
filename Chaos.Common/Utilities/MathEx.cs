#region
using System.Numerics;
#endregion

namespace Chaos.Common.Utilities;

/// <summary>
///     A static utility for performing math operations
/// </summary>
public static class MathEx
{
    /// <summary>
    ///     Calculates a percentile between 2 numbers
    /// </summary>
    /// <param name="current">
    ///     The current value
    /// </param>
    /// <param name="max">
    ///     The maximum value
    /// </param>
    /// <typeparam name="TNumber">
    ///     The type of numeric to return
    /// </typeparam>
    /// <returns>
    ///     The percentile <paramref name="current" /> is of <paramref name="max" />
    /// </returns>
    public static TNumber CalculatePercent<TNumber>(int current, int max) where TNumber: INumber<TNumber>
        => (TNumber)Convert.ChangeType(current / (decimal)max * 100m, typeof(TNumber));

    /// <summary>
    ///     Calculates the percentage of a number
    /// </summary>
    /// <param name="num">
    ///     The number to get the percentile of
    /// </param>
    /// <param name="percent">
    ///     The percent to calculate
    /// </param>
    /// <typeparam name="TNumber">
    ///     The type of numeric to return
    /// </typeparam>
    /// <returns>
    ///     A number that is "<paramref name="percent" />" percent of <paramref name="num" />
    /// </returns>
    public static TNumber GetPercentOf<TNumber>(int num, decimal percent) where TNumber: INumber<TNumber>
        => (TNumber)Convert.ChangeType(num / 100m * percent, typeof(TNumber));

    /// <summary>
    ///     Whether the type is an integer type
    /// </summary>
    private static bool IsIntegerType<T>()
        => (typeof(T) == typeof(sbyte))
           || (typeof(T) == typeof(byte))
           || (typeof(T) == typeof(short))
           || (typeof(T) == typeof(ushort))
           || (typeof(T) == typeof(int))
           || (typeof(T) == typeof(uint))
           || (typeof(T) == typeof(long))
           || (typeof(T) == typeof(ulong))
           || (typeof(T) == typeof(nint))
           || (typeof(T) == typeof(nuint));

    /// <summary>
    ///     Scales a number from one range to another range.
    /// </summary>
    /// <param name="num">
    ///     The input number to be scaled.
    /// </param>
    /// <param name="min">
    ///     The lower bound of the original range.
    /// </param>
    /// <param name="max">
    ///     The upper bound of the original range.
    /// </param>
    /// <param name="newMin">
    ///     The lower bound of the new range.
    /// </param>
    /// <param name="newMax">
    ///     The upper bound of the new range.
    /// </param>
    /// <returns>
    ///     The scaled number in the new range.
    /// </returns>
    /// <remarks>
    ///     This method assumes that the input number is within the original range. No clamping or checking is performed.
    /// </remarks>
    public static double ScaleRange(
        double num,
        double min,
        double max,
        double newMin,
        double newMax)
    {
        if (min.Equals(max))
            throw new ArgumentOutOfRangeException(nameof(min), "Min and max cannot be the same value");

        return (newMax - newMin) * (num - min) / (max - min) + newMin;
    }

    /// <summary>
    ///     Scales a number from one range to another range.
    /// </summary>
    /// <param name="num">
    ///     The input number to be scaled.
    /// </param>
    /// <param name="min">
    ///     The lower bound of the original range.
    /// </param>
    /// <param name="max">
    ///     The upper bound of the original range.
    /// </param>
    /// <param name="newMin">
    ///     The lower bound of the new range.
    /// </param>
    /// <param name="newMax">
    ///     The upper bound of the new range.
    /// </param>
    /// <returns>
    ///     The scaled number in the new range.
    /// </returns>
    /// <remarks>
    ///     This method assumes that the input number is within the original range. No clamping or checking is performed.
    /// </remarks>
    public static T2 ScaleRange<T1, T2>(
        T1 num,
        T1 min,
        T1 max,
        T2 newMin,
        T2 newMax) where T1: INumber<T1>
                   where T2: INumber<T2>
    {
        if (min.Equals(max))
            throw new ArgumentOutOfRangeException(nameof(min), "Min and max cannot be the same value");

        // Compute the ratio as double for higher precision
        var ratio = double.CreateChecked(num - min) / double.CreateChecked(max - min);

        // Compute the scaled value
        var scaledValue = ratio * double.CreateChecked(newMax - newMin) + double.CreateChecked(newMin);

        // Determine if T2 is an integer type
        if (IsIntegerType<T2>())
        {
            // Round the scaled value to the nearest integer
            var roundedValue = Math.Round(scaledValue, MidpointRounding.AwayFromZero);

            return T2.CreateChecked(roundedValue);
        }

        // For floating-point types, return the scaled value directly
        return T2.CreateChecked(scaledValue);
    }
}