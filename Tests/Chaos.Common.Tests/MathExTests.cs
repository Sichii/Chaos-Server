#region
using Chaos.Extensions.Common;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class MathExtensionsTests
{
    [Test]
    public void CalculatePercent_Should_Throw_When_Max_Is_Zero()
    {
        Action act = () => Math.CalculatePercent<decimal>(5, 0);

        act.Should()
           .Throw<DivideByZeroException>();
    }

    [Test]
    public void CalculatePercent_ShouldCalculatePercentBetweenTwoNumbers()
    {
        // Arrange
        const int CURRENT = 25;
        const int MAX = 100;

        // Act
        var result = Math.CalculatePercent<ulong>(CURRENT, MAX);

        // Assert
        result.Should()
              .Be(25);
    }

    [Test]
    public void CalculatePercent_ShouldCalculatePercentBetweenTwoNumbers_ForDifferentNumericTypes()
    {
        // Arrange
        const int CURRENT = 25;
        const int MAX = 100;

        // Act
        var result = Math.CalculatePercent<decimal>(CURRENT, MAX);

        // Assert
        result.Should()
              .Be(25.0m);
    }

    [Test]
    public void GetPercentOf_ShouldCalculatePercentageOfNumber()
    {
        // Arrange
        const int NUM = 50;
        const decimal PERCENT = 20;

        // Act
        var result = Math.GetPercentOf<ulong>(NUM, PERCENT);

        // Assert
        result.Should()
              .Be(10);
    }

    [Test]
    public void GetPercentOf_ShouldCalculatePercentageOfNumber_ForDifferentNumericTypes()
    {
        // Arrange
        const int NUM = 50;
        const decimal PERCENT = 20;

        // Act
        var result = Math.GetPercentOf<decimal>(NUM, PERCENT);

        // Assert
        result.Should()
              .Be(10.0m);
    }

    [Test]
    public void ScaleRange_Generic_IntegerTarget_Rounds_Away_From_Zero()
    {
        var result = Math.ScaleRange(
            2,
            0,
            3,
            0,
            1); // ratio=2/3 -> 0.666.. → rounds to 1

        result.Should()
              .Be(1);
    }

    [Test]
    public void ScaleRange_Generic_Should_Throw_When_Min_Equals_Max()
    {
        Action act = () => Math.ScaleRange(
            1,
            2,
            2,
            0.0,
            1.0);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void ScaleRange_Should_Throw_When_Min_Equals_Max_Double()
    {
        Action act = () => Math.ScaleRange(
            1.0,
            2.0,
            2.0,
            0.0,
            1.0);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void ScaleRange_ShouldScaleNumberFromOneRangeToAnother()
    {
        // Arrange
        const double NUM = 50;
        const double MIN = 0;
        const double MAX = 100;
        const double NEW_MIN = 0;
        const double NEW_MAX = 10;

        // Act
        var result = Math.ScaleRange(
            NUM,
            MIN,
            MAX,
            NEW_MIN,
            NEW_MAX);

        // Assert
        result.Should()
              .Be(5);
    }

    [Test]
    public void ScaleRange_ShouldScaleNumberFromOneRangeToAnother_ForDifferentNumericTypes()
    {
        // Arrange
        const decimal NUM = 50;
        const decimal MIN = 0;
        const decimal MAX = 100;
        const decimal NEW_MIN = 0;
        const decimal NEW_MAX = 10;

        // Act
        var result = Math.ScaleRange(
            NUM,
            MIN,
            MAX,
            NEW_MIN,
            NEW_MAX);

        // Assert
        result.Should()
              .Be(5.0m);
    }

    [Test]
    public void ScaleRange_ShouldScaleNumberFromOneRangeToAnother_WithNumericTypes()
    {
        // Arrange
        const int NUM = 50;
        const int MIN = 0;
        const int MAX = 100;
        const double NEW_MIN = 0.0;
        const double NEW_MAX = 10.0;

        // Act
        var result = Math.ScaleRange(
            NUM,
            MIN,
            MAX,
            NEW_MIN,
            NEW_MAX);

        // Assert
        result.Should()
              .Be(5.0);
    }
}