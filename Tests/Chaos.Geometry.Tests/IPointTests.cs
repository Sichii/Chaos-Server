#region
using Chaos.Geometry.Abstractions;
using FluentAssertions;

// ReSharper disable ArrangeAttributes
#endregion

namespace Chaos.Geometry.Tests;

public sealed class IPointTests
{
    [Test]
    public void IPoint_ToString_Instance_And_Static_ReturnSameResult_ForPoint()
    {
        // Arrange
        IPoint point = new Point(42, 84);

        // Act
        var instanceResult = point.ToString();
        var staticResult = IPoint.ToString(point);

        // Assert
        instanceResult.Should()
                      .Be(staticResult);
    }

    //formatter:off
    [Test]
    [Arguments(0, 0, "(0, 0)")]
    [Arguments(1, 2, "(1, 2)")]
    [Arguments(-5, 10, "(-5, 10)")]
    [Arguments(100, -200, "(100, -200)")]
    [Arguments(int.MaxValue, int.MinValue, "(2147483647, -2147483648)")]

    //formatter:on
    public void IPoint_ToString_Instance_Point_ReturnsExpectedFormat(int x, int y, string expected)
    {
        // Arrange
        IPoint point = new Point(x, y);

        // Act
        var result = point.ToString();

        // Assert
        result.Should()
              .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(0, 0, "(0, 0)")]
    [Arguments(1, 2, "(1, 2)")]
    [Arguments(-5, 10, "(-5, 10)")]
    [Arguments(100, -200, "(100, -200)")]
    [Arguments(int.MaxValue, int.MinValue, "(2147483647, -2147483648)")]

    //formatter:on
    public void IPoint_ToString_Instance_ValuePoint_ReturnsExpectedFormat(int x, int y, string expected)
    {
        // Arrange
        var point = new ValuePoint(x, y);

        // Act
        var result = point.ToString();

        // Assert
        result.Should()
              .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(0, 0, "(0, 0)")]
    [Arguments(1, 2, "(1, 2)")]
    [Arguments(-5, 10, "(-5, 10)")]
    [Arguments(100, -200, "(100, -200)")]
    [Arguments(int.MaxValue, int.MinValue, "(2147483647, -2147483648)")]

    //formatter:on
    public void IPoint_ToString_Static_Point_ReturnsExpectedFormat(int x, int y, string expected)
    {
        // Arrange
        IPoint point = new Point(x, y);

        // Act
        var result = IPoint.ToString(point);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void Point_And_ValuePoint_ToString_ReturnSameFormat()
    {
        // Arrange
        const int X = 42;
        const int Y = 84;
        var point = new Point(X, Y);
        var valuePoint = new ValuePoint(X, Y);

        // Act
        var pointResult = point.ToString();
        var valuePointResult = valuePoint.ToString();

        // Assert
        pointResult.Should()
                   .Be(valuePointResult);
    }
}