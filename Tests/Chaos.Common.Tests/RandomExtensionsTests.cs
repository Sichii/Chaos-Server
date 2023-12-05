using System.Numerics;
using Chaos.Extensions.Common;
using FluentAssertions;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class RandomExtensionsTests
{
    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenMinAndMax_ForByte()
    {
        const byte MIN_VALUE = 10;
        const byte MAX_VALUE = 20;
        RunRangeTest(MIN_VALUE, MAX_VALUE);
    }

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenMinAndMax_ForDecimal()
    {
        const decimal MIN_VALUE = -1.5m;
        const decimal MAX_VALUE = 1.5m;
        RunRangeTest(MIN_VALUE, MAX_VALUE);
    }

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenMinAndMax_ForDouble()
    {
        const double MIN_VALUE = -1.5;
        const double MAX_VALUE = 1.5;
        RunRangeTest(MIN_VALUE, MAX_VALUE);
    }

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenMinAndMax_ForFloat()
    {
        const float MIN_VALUE = -1.5f;
        const float MAX_VALUE = 1.5f;
        RunRangeTest(MIN_VALUE, MAX_VALUE);
    }

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenMinAndMax_ForInt()
    {
        const int MIN_VALUE = -1000;
        const int MAX_VALUE = 1000;
        RunRangeTest(MIN_VALUE, MAX_VALUE);
    }

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenMinAndMax_ForLong()
    {
        const long MIN_VALUE = -10000;
        const long MAX_VALUE = 10000;
        RunRangeTest(MIN_VALUE, MAX_VALUE);
    }

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenMinAndMax_ForSByte()
    {
        const sbyte MIN_VALUE = -10;
        const sbyte MAX_VALUE = 10;
        RunRangeTest(MIN_VALUE, MAX_VALUE);
    }

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenMinAndMax_ForShort()
    {
        const short MIN_VALUE = -100;
        const short MAX_VALUE = 100;
        RunRangeTest(MIN_VALUE, MAX_VALUE);
    }

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenMinAndMax_ForUInt()
    {
        const uint MIN_VALUE = 1000;
        const uint MAX_VALUE = 2000;
        RunRangeTest(MIN_VALUE, MAX_VALUE);
    }

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenMinAndMax_ForULong()
    {
        const ulong MIN_VALUE = 10000;
        const ulong MAX_VALUE = 20000;
        RunRangeTest(MIN_VALUE, MAX_VALUE);
    }

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenMinAndMax_ForUShort()
    {
        const ushort MIN_VALUE = 100;
        const ushort MAX_VALUE = 200;
        RunRangeTest(MIN_VALUE, MAX_VALUE);
    }

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenZeroAndMaxValue_ForByte() => RunTest(byte.MinValue, byte.MaxValue);

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenZeroAndMaxValue_ForInt() => RunTest(int.MinValue, int.MaxValue);

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenZeroAndMaxValue_ForLong() => RunTest(long.MinValue, long.MaxValue);

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenZeroAndMaxValue_ForSByte() => RunTest(sbyte.MinValue, sbyte.MaxValue);

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenZeroAndMaxValue_ForShort() => RunTest(short.MinValue, short.MaxValue);

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenZeroAndMaxValue_ForUInt() => RunTest(uint.MinValue, uint.MaxValue);

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenZeroAndMaxValue_ForULong() => RunTest(ulong.MinValue, ulong.MaxValue);

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenZeroAndMaxValue_ForUShort() => RunTest(ushort.MinValue, ushort.MaxValue);

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenZeroAndOne_ForDecimal() => RunTest(decimal.Zero, decimal.One);

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenZeroAndOne_ForDouble() => RunTest(0d, 1d);

    [Fact]
    public void Next_ShouldReturnRandomNumberBetweenZeroAndOne_ForFloat() => RunTest(0f, 1f);

    private void RunRangeTest<T>(T minValue, T maxValue) where T: INumber<T>
    {
        const int ITERATIONS = 10000;

        for (var i = 0; i < ITERATIONS; i++)
        {
            // Act
            var result = Random.Shared.Next(minValue, maxValue);

            // Assert
            result.Should()
                  .BeInRange(minValue, maxValue);
        }
    }

    private void RunTest<T>(T minValue, T maxValue) where T: INumber<T>
    {
        const int ITERATIONS = 10000;

        for (var i = 0; i < ITERATIONS; i++)
        {
            // Act
            var result = Random.Shared.Next<T>();

            // Assert
            result.Should()
                  .BeInRange(minValue, maxValue);
        }
    }
}