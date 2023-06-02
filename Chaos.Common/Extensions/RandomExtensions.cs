using System.Numerics;
using Chaos.Common.Utilities;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="System.Random" />.
/// </summary>
public static class RandomExtensions
{
    /// <summary>
    /// Returns a random number between 0 and the number's max value (1 for floating point types and decimals)
    /// </summary>
    public static T Next<T>(this Random random) where T: INumber<T>
    {
        var zero = T.Zero;

        switch (zero)
        {
            case byte:
                return T.CreateChecked(random.Next(byte.MaxValue));
            case sbyte:
                return T.CreateChecked(random.Next(sbyte.MaxValue));
            case short:
                return T.CreateChecked(random.Next(short.MaxValue));
            case ushort:
                return T.CreateChecked(random.Next(ushort.MaxValue));
            case int:
                return T.CreateChecked(random.Next(int.MaxValue));
            case uint:
                return T.CreateChecked(random.NextInt64(uint.MaxValue));
            case long:
                return T.CreateChecked(random.NextInt64(long.MaxValue));
            case ulong:
                var randomNum = (ulong)random.NextInt64(long.MinValue, long.MaxValue);

                return T.CreateChecked(randomNum);
            case decimal:
                var dbl = random.NextDouble();

                return T.CreateChecked(
                    Convert.ToDecimal(
                        MathEx.ScaleRange(
                            dbl,
                            0.0d,
                            1.0d,
                            0.0d,
                            Convert.ToDouble(decimal.MaxValue))));
            case float:
                return T.CreateChecked(random.NextSingle());
            case double:
                return T.CreateChecked(random.NextDouble());
            default:
                throw new ArgumentOutOfRangeException(nameof(T));
        }
    }

    /// <summary>
    /// Returns a random number between <paramref name="min"/> and <paramref name="max"/>
    /// </summary>
    public static T Next<T>(this Random random, T min, T max) where T: INumber<T>
    {
        var zero = T.Zero;

        switch (zero)
        {
            case byte:
                var minByte = byte.CreateChecked(min);
                var maxByte = byte.CreateChecked(max);

                return T.CreateChecked(random.Next(minByte, maxByte));
            case sbyte:
                var minSByte = sbyte.CreateChecked(min);
                var maxSByte = sbyte.CreateChecked(max);

                return T.CreateChecked(random.Next(minSByte, maxSByte));
            case short:
                var minShort = short.CreateChecked(min);
                var maxShort = short.CreateChecked(max);

                return T.CreateChecked(random.Next(minShort, maxShort));
            case ushort:
                var minUShort = ushort.CreateChecked(min);
                var maxUShort = ushort.CreateChecked(max);

                return T.CreateChecked(random.Next(minUShort, maxUShort));
            case int:
                var minInt = int.CreateChecked(min);
                var maxInt = int.CreateChecked(max);

                return T.CreateChecked(random.Next(minInt, maxInt));
            case uint:
                var minUInt = uint.CreateChecked(min);
                var maxUInt = uint.CreateChecked(max);

                return T.CreateChecked(random.NextInt64(minUInt, maxUInt));
            case long:
                var minLong = long.CreateChecked(min);
                var maxLong = long.CreateChecked(max);

                return T.CreateChecked(random.NextInt64(minLong, maxLong));
            case ulong:
                var minULong = (long)(decimal.CreateChecked(min) - long.MaxValue);
                var maxULong = (long)(decimal.CreateChecked(max) - long.MaxValue);

                var randomNum = (decimal)random.NextInt64(minULong, maxULong) + long.MaxValue;

                return T.CreateChecked(randomNum);
            case decimal:
                var minDecimal = decimal.CreateChecked(min);
                var maxDecimal = decimal.CreateChecked(max);
                var dbl = random.NextDouble();

                return T.CreateChecked(
                    Convert.ToDecimal(
                        MathEx.ScaleRange(
                            dbl,
                            0.0d,
                            1.0d,
                            Convert.ToDouble(minDecimal),
                            Convert.ToDouble(maxDecimal))));
            case float:
                var minFloat = float.CreateChecked(min);
                var maxFloat = float.CreateChecked(max);

                return T.CreateChecked(
                    MathEx.ScaleRange(
                        random.NextSingle(),
                        0.0f,
                        1.0f,
                        minFloat,
                        maxFloat));
            case double:
                var minDouble = double.CreateChecked(min);
                var maxDouble = double.CreateChecked(max);

                return T.CreateChecked(
                    MathEx.ScaleRange(
                        random.NextDouble(),
                        0.0d,
                        1.0d,
                        minDouble,
                        maxDouble));
            default:
                throw new ArgumentOutOfRangeException(nameof(T));
        }
    }
}