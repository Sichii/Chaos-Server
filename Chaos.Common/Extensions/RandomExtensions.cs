using Chaos.Common.Utilities;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="System.Random" />.
/// </summary>
public static class RandomExtensions
{
    /// <summary>
    ///     Returns a random decimal number that is greater than or equal to <paramref name="min" />, and less than <paramref name="max" />.
    /// </summary>
    /// <param name="random"></param>
    /// <param name="min">The inclusive lower bound of the random number returned</param>
    /// <param name="max">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue</param>
    /// <returns>A decimal number that is greater than or equal to <paramref name="min" />, and less than <paramref name="max" /></returns>
    public static decimal Next(this Random random, decimal min, decimal max)
    {
        if (min > max)
            throw new ArgumentException($"{nameof(max)} must be greater than or equal to {nameof(min)}");

        var dbl = random.NextDouble();

        return Convert.ToDecimal(
            MathEx.ScaleRange(
                dbl,
                0.0d,
                1.0d,
                Convert.ToDouble(min),
                Convert.ToDouble(max)));
    }

    /// <summary>
    ///     Returns a random decimal number that is greater than or equal to 0.0, and less than <paramref name="max" />.
    /// </summary>
    /// <param name="random"></param>
    /// <param name="max">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to 0.0</param>
    /// <returns>A decimal number that is greater than or equal to 0.0, and less than <paramref name="max" /></returns>
    public static decimal Next(this Random random, decimal max)
    {
        if (0.0m > max)
            throw new ArgumentException($"{nameof(max)} must be greater than or equal to 0.0");

        var dbl = random.NextDouble();

        return Convert.ToDecimal(
            MathEx.ScaleRange(
                dbl,
                0.0d,
                1.0d,
                0.0d,
                Convert.ToDouble(max)));
    }
}