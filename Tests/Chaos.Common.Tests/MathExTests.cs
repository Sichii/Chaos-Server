using Chaos.Common.Utilities;
using FluentAssertions;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class MathExTests
{
    [Fact]
    public void CalculatePercent_ShouldCalculatePercentBetweenTwoNumbers()
    {
        // Arrange
        const int CURRENT = 25;
        const int MAX = 100;

        // Act
        var result = MathEx.CalculatePercent<ulong>(CURRENT, MAX);

        // Assert
        result.Should()
              .Be(25);
    }

    [Fact]
    public void CalculatePercent_ShouldCalculatePercentBetweenTwoNumbers_ForDifferentNumericTypes()
    {
        // Arrange
        const int CURRENT = 25;
        const int MAX = 100;

        // Act
        var result = MathEx.CalculatePercent<decimal>(CURRENT, MAX);

        // Assert
        result.Should()
              .Be(25.0m);
    }

    [Fact]
    public void GetPercentOf_ShouldCalculatePercentageOfNumber()
    {
        // Arrange
        const int NUM = 50;
        const decimal PERCENT = 20;

        // Act
        var result = MathEx.GetPercentOf<ulong>(NUM, PERCENT);

        // Assert
        result.Should()
              .Be(10);
    }

    [Fact]
    public void GetPercentOf_ShouldCalculatePercentageOfNumber_ForDifferentNumericTypes()
    {
        // Arrange
        const int NUM = 50;
        const decimal PERCENT = 20;

        // Act
        var result = MathEx.GetPercentOf<decimal>(NUM, PERCENT);

        // Assert
        result.Should()
              .Be(10.0m);
    }

    [Fact]
    public void ScaleRange_ShouldScaleNumberFromOneRangeToAnother()
    {
        // Arrange
        const double NUM = 50;
        const double MIN = 0;
        const double MAX = 100;
        const double NEW_MIN = 0;
        const double NEW_MAX = 10;

        // Act
        var result = MathEx.ScaleRange(
            NUM,
            MIN,
            MAX,
            NEW_MIN,
            NEW_MAX);

        // Assert
        result.Should()
              .Be(5);
    }

    [Fact]
    public void ScaleRange_ShouldScaleNumberFromOneRangeToAnother_ForDifferentNumericTypes()
    {
        // Arrange
        const decimal NUM = 50;
        const decimal MIN = 0;
        const decimal MAX = 100;
        const decimal NEW_MIN = 0;
        const decimal NEW_MAX = 10;

        // Act
        var result = MathEx.ScaleRange(
            NUM,
            MIN,
            MAX,
            NEW_MIN,
            NEW_MAX);

        // Assert
        result.Should()
              .Be(5.0m);
    }

    [Fact]
    public void ScaleRange_ShouldScaleNumberFromOneRangeToAnother_WithNumericTypes()
    {
        // Arrange
        const int NUM = 50;
        const int MIN = 0;
        const int MAX = 100;
        const double NEW_MIN = 0.0;
        const double NEW_MAX = 10.0;

        // Act
        var result = MathEx.ScaleRange(
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