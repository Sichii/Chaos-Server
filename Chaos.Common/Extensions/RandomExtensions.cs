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
    ///     Returns a random number between 0 and the number's max value (1 for floating point types and decimals)
    /// </summary>
    public static T Next<T>(this Random random) where T: INumber<T>
    {
        var zero = T.Zero;

        return zero switch
        {
            byte    => T.CreateChecked(random.Next(byte.MaxValue)),
            sbyte   => T.CreateChecked(random.Next(sbyte.MaxValue)),
            short   => T.CreateChecked(random.Next(short.MaxValue)),
            ushort  => T.CreateChecked(random.Next(ushort.MaxValue)),
            int     => T.CreateChecked(random.Next(int.MaxValue)),
            uint    => T.CreateChecked((uint)random.Next(int.MaxValue)),
            long    => T.CreateChecked(random.NextInt64(long.MaxValue)),
            ulong   => T.CreateChecked((ulong)random.NextInt64(long.MinValue, long.MaxValue)),
            decimal => T.CreateChecked(Convert.ToDecimal(random.NextDouble())),
            float   => T.CreateChecked(random.NextSingle()),
            double  => T.CreateChecked(random.NextDouble()),
            _       => throw new ArgumentOutOfRangeException(nameof(T))
        };
    }

    /// <summary>
    ///     Returns a random number between <paramref name="min" /> and <paramref name="max" />
    /// </summary>
    public static T Next<T>(this Random random, T min, T max) where T: INumber<T>
    {
        var zero = T.Zero;

        switch (zero)
        {
            case byte:
            {
                var minByte = byte.CreateChecked(min);
                var maxByte = byte.CreateChecked(max);

                return T.CreateChecked(random.Next(minByte, maxByte));
            }
            case sbyte:
            {
                var minSByte = sbyte.CreateChecked(min);
                var maxSByte = sbyte.CreateChecked(max);

                return T.CreateChecked(random.Next(minSByte, maxSByte));
            }
            case short:
            {
                var minShort = short.CreateChecked(min);
                var maxShort = short.CreateChecked(max);

                return T.CreateChecked(random.Next(minShort, maxShort));
            }
            case ushort:
            {
                var minUShort = ushort.CreateChecked(min);
                var maxUShort = ushort.CreateChecked(max);

                return T.CreateChecked(random.Next(minUShort, maxUShort));
            }
            case int:
            {
                var minInt = int.CreateChecked(min);
                var maxInt = int.CreateChecked(max);

                return T.CreateChecked(random.Next(minInt, maxInt));
            }
            case uint:
            {
                var minUInt = uint.CreateChecked(min);
                var maxUInt = uint.CreateChecked(max);

                return T.CreateChecked(random.NextInt64(minUInt, maxUInt));
            }
            case long:
            {
                var minLong = long.CreateChecked(min);
                var maxLong = long.CreateChecked(max);

                return T.CreateChecked(random.NextInt64(minLong, maxLong));
            }
            case ulong:
            {
                //there is no "bigger datatype" so we gotta do some tricks
                //move the random values down into the range of long
                var minLong = (long)(ulong.CreateChecked(min) - long.MaxValue);
                var maxLong = (long)(ulong.CreateChecked(max) - long.MaxValue);

                //generate random long
                var randomLong = random.NextInt64(minLong, maxLong);

                //if the random long is negative, return long.MaxValue - Math.Abs(randomLong)
                //this is the equivalent of "randomLong + long.MaxValue" which is not otherwise possible due to long/ulong overflow
                if (randomLong < 0)
                    return T.CreateChecked(long.MaxValue - (ulong)Math.Abs(randomLong));

                return T.CreateChecked(long.MaxValue + (ulong)randomLong);
            }
            case decimal:
            {
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
            }
            case float:
            {
                var minFloat = float.CreateChecked(min);
                var maxFloat = float.CreateChecked(max);

                return T.CreateChecked(
                    MathEx.ScaleRange(
                        random.NextSingle(),
                        0.0f,
                        1.0f,
                        minFloat,
                        maxFloat));
            }
            case double:
            {
                var minDouble = double.CreateChecked(min);
                var maxDouble = double.CreateChecked(max);

                return T.CreateChecked(
                    MathEx.ScaleRange(
                        random.NextDouble(),
                        0.0d,
                        1.0d,
                        minDouble,
                        maxDouble));
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(T));
        }
    }
}