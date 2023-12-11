using Chaos.Geometry.Abstractions;
using FluentAssertions;
using Xunit;

namespace Chaos.Geometry.Tests;

public sealed class RectangleTests
{
    [Fact]
    public void Constructor_AdjustsWidthAndHeightForOddNumbers()
    {
        // Arrange
        var center = new Point(5, 10);
        const int WIDTH = 4;
        const int HEIGHT = 6;

        // Act
        var rectangle = new Rectangle(center, WIDTH, HEIGHT);

        // Assert
        rectangle.Width
                 .Should()
                 .Be(4);

        rectangle.Height
                 .Should()
                 .Be(6);

        rectangle.Top
                 .Should()
                 .Be(8);

        rectangle.Left
                 .Should()
                 .Be(4);

        rectangle.Right
                 .Should()
                 .Be(7);

        rectangle.Bottom
                 .Should()
                 .Be(13);
    }

    [Fact]
    public void Constructor_CreatesRectangleWithCorrectCenterAndDimensions()
    {
        // Arrange
        var center = new Point(5, 10);
        const int WIDTH = 3;
        const int HEIGHT = 5;

        // Act
        var rectangle = new Rectangle(center, WIDTH, HEIGHT);

        // Assert
        rectangle.Width
                 .Should()
                 .Be(WIDTH);

        rectangle.Height
                 .Should()
                 .Be(HEIGHT);

        rectangle.Top
                 .Should()
                 .Be(8);

        rectangle.Left
                 .Should()
                 .Be(4);

        rectangle.Right
                 .Should()
                 .Be(6);

        rectangle.Bottom
                 .Should()
                 .Be(12);
    }

    [Fact]
    public void Rectangle_Constructor_CreatesRectangleWithGivenValues()
    {
        // Arrange
        const int LEFT = 10;
        const int TOP = 20;
        const int WIDTH = 30;
        const int HEIGHT = 40;

        // Act
        var rectangle = new Rectangle(
            LEFT,
            TOP,
            WIDTH,
            HEIGHT);

        // Assert
        rectangle.Left
                 .Should()
                 .Be(LEFT);

        rectangle.Top
                 .Should()
                 .Be(TOP);

        rectangle.Width
                 .Should()
                 .Be(WIDTH);

        rectangle.Height
                 .Should()
                 .Be(HEIGHT);

        rectangle.Right
                 .Should()
                 .Be(LEFT + WIDTH - 1);

        rectangle.Bottom
                 .Should()
                 .Be(TOP + HEIGHT - 1);

        rectangle.Area
                 .Should()
                 .Be(WIDTH * HEIGHT);

        rectangle.Vertices
                 .Should()
                 .HaveCount(4);

        rectangle.Vertices
                 .Should()
                 .ContainInOrder(
                     new List<IPoint>
                     {
                         new Point(LEFT, TOP),
                         new Point(LEFT + WIDTH - 1, TOP),
                         new Point(LEFT + WIDTH - 1, TOP + HEIGHT - 1),
                         new Point(LEFT, TOP + HEIGHT - 1)
                     });
    }

    [Fact]
    public void Rectangle_Equals_ReturnsFalseWhenComparingWithDifferentType()
    {
        // Arrange
        var rectangle = new Rectangle(
            10,
            20,
            30,
            40);

        var otherObject = new object();

        // Act
        var result = rectangle.Equals(otherObject);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Fact]
    public void Rectangle_Equals_ReturnsFalseWhenRectanglesAreNotEqual()
    {
        // Arrange
        var rectangle1 = new Rectangle(
            10,
            20,
            30,
            40);

        var rectangle2 = new Rectangle(
            50,
            60,
            70,
            80);

        // Act
        var result = rectangle1.Equals(rectangle2);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Fact]
    public void Rectangle_Equals_ReturnsTrueWhenRectanglesAreEqual()
    {
        // Arrange
        var rectangle1 = new Rectangle(
            10,
            20,
            30,
            40);

        var rectangle2 = new Rectangle(
            10,
            20,
            30,
            40);

        // Act
        var result = rectangle1.Equals(rectangle2);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void Rectangle_GetEnumerator_ReturnsVerticesEnumerator()
    {
        // Arrange
        var rectangle = new Rectangle(
            10,
            20,
            30,
            40);

        var expectedVertices = new List<IPoint>
        {
            new Point(10, 20),
            new Point(39, 20),
            new Point(39, 59),
            new Point(10, 59)
        };

        // Act
        var actualVertices = rectangle.ToList();

        // Assert
        actualVertices.Should()
                      .ContainInOrder(expectedVertices);
    }

    [Fact]
    public void Rectangle_GetHashCode_ReturnsConsistentHashCode()
    {
        // Arrange
        var rectangle = new Rectangle(
            10,
            20,
            30,
            40);

        var expectedHashCode = HashCode.Combine(
            rectangle.Height,
            rectangle.Left,
            rectangle.Top,
            rectangle.Width);

        // Act
        var hashCode1 = rectangle.GetHashCode();
        var hashCode2 = rectangle.GetHashCode();

        // Assert
        hashCode1.Should()
                 .Be(expectedHashCode);

        hashCode2.Should()
                 .Be(expectedHashCode);
    }
}