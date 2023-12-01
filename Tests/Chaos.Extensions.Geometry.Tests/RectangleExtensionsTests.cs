

// ReSharper disable ArrangeAttributes

namespace Chaos.Extensions.Geometry.Tests;

public sealed class RectangleExtensionsTests
{
    //@formatter:off
    [Theory]
    [InlineData(0, 0, 10, 10, 5, 5, true)]    // Point is inside the rectangle
    [InlineData(0, 0, 10, 10, 0, 0, true)]    // Point is on the top-left corner of the rectangle
    [InlineData(0, 0, 10, 10, 9, 9, true)]  // Point is on the bottom-right corner of the rectangle
    [InlineData(0, 0, 10, 10, 15, 15, false)] // Point is outside the rectangle
    [InlineData(0, 0, 10, 10, 5, 15, false)]  // Point is below the rectangle
    [InlineData(0, 0, 10, 10, -5, -5, false)] // Point is outside the rectangle
    //@formatter:on
    public void Contains_Should_Return_Correct_Result(
        int rectX,
        int rectY,
        int rectWidth,
        int rectHeight,
        int pointX,
        int pointY,
        bool expectedResult)
    {
        // Arrange
        var rect = new Rectangle(
            rectX,
            rectY,
            rectWidth,
            rectHeight);

        var point = new Point(pointX, pointY);

        // Act
        var result = rect.Contains(point);

        // Assert
        result.Should()
              .Be(expectedResult);
    }

    [Fact]
    public void Contains_Should_Return_False_When_Rectangle_Does_Not_Contain_Other_Rectangle()
    {
        // Arrange
        var rect = new Rectangle(
            0,
            0,
            10,
            10);

        var otherRect = new Rectangle(
            12,
            12,
            15,
            15);

        // Act
        var result = rect.Contains(otherRect);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Fact]
    public void Contains_Should_Return_False_When_Rectangles_Intersect()
    {
        // Arrange
        var rect = new Rectangle(
            0,
            0,
            10,
            10);

        var intersectingRect = new Rectangle(
            8,
            8,
            15,
            15);

        // Act
        var result = rect.Contains(intersectingRect);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Fact]
    public void Contains_Should_Return_True_When_Rectangle_Contains_Other_Rectangle()
    {
        // Arrange
        var rect = new Rectangle(
            0,
            0,
            10,
            10);

        var otherRect = new Rectangle(
            2,
            2,
            6,
            6);

        // Act
        var result = rect.Contains(otherRect);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void Contains_Should_Return_True_When_Rectangle_Contains_Same_Rectangle()
    {
        // Arrange
        var rect = new Rectangle(
            0,
            0,
            10,
            10);

        var sameRect = new Rectangle(
            0,
            0,
            10,
            10);

        // Act
        var result = rect.Contains(sameRect);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void GetOutline_Should_Return_Correct_Outline_Points_For_Rectangle()
    {
        // Arrange
        var rect = new Rectangle(
            0,
            0,
            5,
            3);

        // Act
        var outline = rect.GetOutline()
                          .ToList();

        // Assert
        outline.Should()
               .HaveCount(12);

        outline.Should()
               .BeEquivalentTo(
                   new[]
                   {
                       new Point(0, 0),
                       new Point(0, 1),
                       new Point(0, 2),
                       new Point(1, 2),
                       new Point(2, 2),
                       new Point(3, 2),
                       new Point(4, 2),
                       new Point(4, 1),
                       new Point(4, 0),
                       new Point(3, 0),
                       new Point(2, 0),
                       new Point(1, 0)
                   });
    }

    [Fact]
    public void GetOutline_Should_Return_Correct_Outline_Points_For_Rectangle_With_Negative_Coordinates()
    {
        // Arrange
        var rect = new Rectangle(
            -3,
            -2,
            6,
            4);

        // Act
        var outline = rect.GetOutline()
                          .ToList();

        // Assert
        outline.Should()
               .HaveCount(16);

        outline.Should()
               .BeEquivalentTo(
                   new[]
                   {
                       new Point(-3, -2),
                       new Point(-3, -1),
                       new Point(-3, 0),
                       new Point(-3, 1),
                       new Point(-2, 1),
                       new Point(-1, 1),
                       new Point(0, 1),
                       new Point(1, 1),
                       new Point(2, 1),
                       new Point(2, 0),
                       new Point(2, -1),
                       new Point(2, -2),
                       new Point(1, -2),
                       new Point(0, -2),
                       new Point(-1, -2),
                       new Point(-2, -2)
                   });
    }

    [Fact]
    public void GetOutline_Should_Return_Correct_Outline_Points_For_Square()
    {
        // Arrange
        var square = new Rectangle(
            0,
            0,
            4,
            4);

        // Act
        var outline = square.GetOutline()
                            .ToList();

        // Assert
        outline.Should()
               .HaveCount(12);

        outline.Should()
               .BeEquivalentTo(
                   new[]
                   {
                       new Point(0, 0),
                       new Point(0, 1),
                       new Point(0, 2),
                       new Point(0, 3),
                       new Point(1, 3),
                       new Point(2, 3),
                       new Point(3, 3),
                       new Point(3, 2),
                       new Point(3, 1),
                       new Point(3, 0),
                       new Point(2, 0),
                       new Point(1, 0)
                   });
    }

    [Fact]
    public void GetPoints_Should_Return_All_Points_Inside_Rectangle()
    {
        // Arrange
        var rect = new Rectangle(
            0,
            0,
            3,
            2);

        // Act
        var points = rect.GetPoints()
                         .ToList();

        // Assert
        points.Should()
              .HaveCount(6);

        points.Should()
              .BeEquivalentTo(
                  new[]
                  {
                      new Point(0, 0),
                      new Point(1, 0),
                      new Point(2, 0),
                      new Point(0, 1),
                      new Point(1, 1),
                      new Point(2, 1)
                  });
    }

    [Fact]
    public void GetPoints_Should_Return_All_Points_Inside_Rectangle_With_Negative_Coordinates()
    {
        // Arrange
        var rect = new Rectangle(
            -2,
            -1,
            3,
            2);

        // Act
        var points = rect.GetPoints()
                         .ToList();

        // Assert
        points.Should()
              .HaveCount(6);

        points.Should()
              .BeEquivalentTo(
                  new[]
                  {
                      new Point(-2, -1),
                      new Point(-1, -1),
                      new Point(0, -1),
                      new Point(-2, 0),
                      new Point(-1, 0),
                      new Point(0, 0)
                  });
    }

    [Fact]
    public void GetPoints_Should_Return_All_Points_Inside_Square()
    {
        // Arrange
        var square = new Rectangle(
            1,
            1,
            3,
            3);

        // Act
        var points = square.GetPoints()
                           .ToList();

        // Assert
        points.Should()
              .HaveCount(9);

        points.Should()
              .BeEquivalentTo(
                  new[]
                  {
                      new Point(1, 1),
                      new Point(2, 1),
                      new Point(3, 1),
                      new Point(1, 2),
                      new Point(2, 2),
                      new Point(3, 2),
                      new Point(1, 3),
                      new Point(2, 3),
                      new Point(3, 3)
                  });
    }

    [Fact]
    public void GetRandomPoint_Should_Return_Point_Inside_Rectangle()
    {
        // Arrange
        var rect = new Rectangle(
            0,
            0,
            5,
            5);

        // Act
        var points = Enumerable.Range(0, 10000)
                               .Select(_ => rect.GetRandomPoint())
                               .ToList();

        // Assert
        foreach (var point in points)
        {
            point.X
                 .Should()
                 .BeGreaterOrEqualTo(rect.Left);

            point.X
                 .Should()
                 .BeLessOrEqualTo(rect.Right);

            point.X
                 .Should()
                 .BeLessThan(rect.Left + rect.Width);

            point.Y
                 .Should()
                 .BeGreaterOrEqualTo(rect.Top);

            point.Y
                 .Should()
                 .BeLessOrEqualTo(rect.Bottom);

            point.Y
                 .Should()
                 .BeLessThan(rect.Top + rect.Height);
        }
    }

    [Fact]
    public void GetRandomPoint_WithPredicate_ReturnsPointMatchingPredicate()
    {
        // Arrange
        var rect = new Rectangle(
            0,
            0,
            10,
            10);

        // Act
        var point = rect.GetRandomPoint(p => p is (> 5, > 5));

        // Assert
        point.X
             .Should()
             .BeGreaterThan(5);

        point.Y
             .Should()
             .BeGreaterThan(5);
    }

    [Fact]
    public void Intersects_Should_Return_False_When_Rectangles_Do_Not_Intersect()
    {
        // Arrange
        var rect1 = new Rectangle(
            0,
            0,
            10,
            10);

        var rect2 = new Rectangle(
            20,
            20,
            10,
            10);

        // Act
        var intersects = rect1.Intersects(rect2);

        // Assert
        intersects.Should()
                  .BeFalse();
    }

    [Fact]
    public void Intersects_Should_Return_True_When_Rectangles_Are_Contained_Within_Each_Other()
    {
        // Arrange
        var rect1 = new Rectangle(
            0,
            0,
            10,
            10);

        var rect2 = new Rectangle(
            2,
            2,
            6,
            6);

        // Act
        var intersects = rect1.Intersects(rect2);

        // Assert
        intersects.Should()
                  .BeTrue();
    }

    [Fact]
    public void Intersects_Should_Return_True_When_Rectangles_Intersect()
    {
        // Arrange
        var rect1 = new Rectangle(
            0,
            0,
            10,
            10);

        var rect2 = new Rectangle(
            5,
            5,
            10,
            10);

        // Act
        var intersects = rect1.Intersects(rect2);

        // Assert
        intersects.Should()
                  .BeTrue();
    }
}