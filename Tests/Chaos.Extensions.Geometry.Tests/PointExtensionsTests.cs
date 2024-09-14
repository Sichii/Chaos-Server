// ReSharper disable ArrangeAttributes

using Chaos.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using FluentAssertions;
using Xunit;

namespace Chaos.Extensions.Geometry.Tests;

public sealed class PointExtensionsTests
{
    [Fact]
    public void ConalSearch_MaxDistanceGreaterThanOne_ReturnsAllPointsWithinCone()
    {
        var startingPoint = new Point(0, 0);

        var points = startingPoint.ConalSearch(Direction.Up, 2)
                                  .ToList();

        points.Should()
              .HaveCount(8);

        points.Should()
              .BeEquivalentTo(
                  new[]
                  {
                      new Point(-2, -2),
                      new Point(-1, -1),
                      new Point(0, -2),
                      new Point(1, -1),
                      new Point(2, -2),
                      new Point(-1, -2),
                      new Point(1, -2),
                      new Point(0, -1)
                  });
    }

    [Fact]
    public void ConalSearch_MaxDistanceOne_ReturnsThreePointsInSpecifiedDirection()
    {
        var startingPoint = new Point(0, 0);

        var points = startingPoint.ConalSearch(Direction.Up, 1)
                                  .ToList();

        points.Should()
              .HaveCount(3);

        points.Should()
              .BeEquivalentTo(
                  new[]
                  {
                      new Point(-1, -1),
                      new Point(0, -1),
                      new Point(1, -1)
                  });
    }

    [Fact]
    public void ConalSearch_MaxDistanceZero_ReturnsEmpty()
    {
        var startingPoint = new Point(0, 0);

        var points = startingPoint.ConalSearch(Direction.Up, 0)
                                  .ToList();

        points.Should()
              .BeEmpty();
    }

    //@formatter:off
    [Theory]
    [InlineData(1, 1, Direction.Up, 1, 1, 0)]
    [InlineData(1, 1, Direction.Up, 2, 1, -1)]
    [InlineData(1, 1, Direction.Right, 1, 2, 1)]
    [InlineData(1, 1, Direction.Right, 2, 3, 1)]
    [InlineData(1, 1, Direction.Down, 1, 1, 2)]
    [InlineData(1, 1, Direction.Down, 2, 1, 3)]
    [InlineData(1, 1, Direction.Left, 1, 0, 1)]
    [InlineData(1, 1, Direction.Left, 2, -1, 1)]
    [InlineData(0, 0, Direction.Up, 1, 0, -1)]
    [InlineData(0, 0, Direction.Right, 1, 1, 0)]
    [InlineData(0, 0, Direction.Down, 1, 0, 1)]
    [InlineData(0, 0, Direction.Left, 1, -1, 0)]
    //@formatter:on
    public void DirectionalOffset_ShouldOffsetPointByDistance(
        int startX,
        int startY,
        Direction direction,
        int distance,
        int expectedX,
        int expectedY)
    {
        // Arrange
        var point = new Point(startX, startY);
        var expectedOffsetPoint = new Point(expectedX, expectedY);

        // Act
        var result = point.DirectionalOffset(direction, distance);

        // Assert
        result.Should()
              .Be(expectedOffsetPoint);
    }

    [Fact]
    public void DirectionalOffset_ShouldThrowException_WhenDirectionIsInvalid()
    {
        // Arrange
        var point = new Point(1, 1);
        const Direction DIRECTION = Direction.Invalid;
        const int DISTANCE = 2;

        // Act
        var action = new Action(() => point.DirectionalOffset(DIRECTION, DISTANCE));

        // Assert
        action.Should()
              .Throw<ArgumentOutOfRangeException>();
    }

    //@formatter:off
    [Theory]
    [InlineData(0, 0, 0, 0, Direction.Invalid)]
    [InlineData(0, 0, 0, 1, Direction.Up)]
    [InlineData(0, 0, 0, -1, Direction.Down)]
    [InlineData(0, 0, -1, 0, Direction.Right)]
    [InlineData(0, 0, 1, 0, Direction.Left)]
    [InlineData(0, 0, -1, 1, Direction.Up, Direction.Right)]
    [InlineData(0, 0, -1, -1, Direction.Down, Direction.Right)]
    [InlineData(0, 0, 1, 1, Direction.Up, Direction.Left)]
    [InlineData(0, 0, 1, -1, Direction.Down, Direction.Left)]
    //@formatter:on
    public void DirectionalRelationTo_VariousPoints_ReturnsExpectedDirection(
        int startX,
        int startY,
        int endX,
        int endY,
        params Direction[] expected)
    {
        var start = new Point(startX, startY);
        var end = new Point(endX, endY);

        var direction = start.DirectionalRelationTo(end);

        expected.Should()
                .Contain(direction);
    }

    //@formatter:off
    [Theory]
    [InlineData(0, 0, 0, 0, 0)]
    [InlineData(0, 0, 0, 1, 1)]
    [InlineData(0, 0, 1, 0, 1)]
    [InlineData(0, 0, 1, 1, 2)]
    [InlineData(1, 1, 1, 1, 0)]
    [InlineData(1, 1, 1, 2, 1)]
    [InlineData(1, 1, 2, 1, 1)]
    [InlineData(1, 1, 2, 2, 2)]
    [InlineData(-1, -1, 1, 1, 4)]
    [InlineData(-1, -1, -1, -1, 0)]
    [InlineData(-1, -1, -2, -1, 1)]
    [InlineData(-1, -1, -1, -2, 1)]
    //@formatter:on
    public void DistanceFrom_ShouldReturnDistanceBetweenTwoPoints(
        int startX,
        int startY,
        int otherX,
        int otherY,
        int expectedDistance)
    {
        // Arrange
        var point = new Point(startX, startY);
        var otherPoint = new Point(otherX, otherY);

        // Act
        var result = point.ManhattanDistanceFrom(otherPoint);

        // Assert
        result.Should()
              .Be(expectedDistance);
    }

    //@formatter:off
    [Theory]
    [InlineData(0, 0, 0, 0, 0)]
    [InlineData(0, 0, 0, 1, 1)]
    [InlineData(0, 0, 1, 0, 1)]
    [InlineData(0, 0, 1, 1, 1.4142135623730951)]
    [InlineData(1, 1, 1, 1, 0)]
    [InlineData(1, 1, 1, 2, 1)]
    [InlineData(1, 1, 2, 1, 1)]
    [InlineData(1, 1, 2, 2, 1.4142135623730951)]
    [InlineData(-1, -1, 1, 1, 2.8284271247461903)]
    [InlineData(-1, -1, -1, -1, 0)]
    [InlineData(-1, -1, -2, -1, 1)]
    [InlineData(-1, -1, -1, -2, 1)]
    //@formatter:on
    public void EuclideanDistanceFrom_ShouldReturnEuclideanDistanceBetweenTwoPoints(
        int startX,
        int startY,
        int otherX,
        int otherY,
        float expectedDistance)
    {
        // Arrange
        var point = new Point(startX, startY);
        var otherPoint = new Point(otherX, otherY);

        // Act
        var result = point.EuclideanDistanceFrom(otherPoint);

        // Assert
        result.Should()
              .BeApproximately(expectedDistance, 0.000001f);
    }

    [Fact]
    public void FloodFill_ShouldOnlyReturnStartPoint_WhenNoTouchingPointsFound()
    {
        // Arrange
        var points = new[]
        {
            new Point(0, 0),
            new Point(1, 0),
            new Point(2, 0),
            new Point(3, 0)
        };

        var startPoint = new Point(1, 2);

        // Act
        var result = points.FloodFill(startPoint);

        // Assert
        result.Should()
              .BeEquivalentTo(
                  new[]
                  {
                      startPoint
                  });
    }

    [Fact]
    public void FloodFill_ShouldReturnAllTouchingPoints_WhenFloodFillingFromStartPoint()
    {
        // Arrange
        var points = new[]
        {
            new Point(0, 0),
            new Point(1, 0),
            new Point(0, 1),
            new Point(1, 1),
            new Point(2, 0),
            new Point(0, 2),
            new Point(2, 2),
            new Point(3, 0),
            new Point(3, 1),
            new Point(3, 2),
            new Point(3, 3)
        };

        var startPoint = new Point(0, 0);

        // Act
        var result = points.FloodFill(startPoint);

        // Assert
        result.Should()
              .BeEquivalentTo(points);
    }

    [Fact]
    public void FloodFill_ShouldReturnOnlyReachablePoints_WhenStartingFromInsideReachableArea()
    {
        // Arrange
        var points = new[]
        {
            new Point(0, 0),
            new Point(1, 0),
            new Point(0, 1),
            new Point(1, 1),
            new Point(2, 0),
            new Point(0, 2),
            new Point(2, 2),
            new Point(3, 0),
            new Point(3, 1),
            new Point(3, 2),
            new Point(3, 3),
            new Point(5, 5),
            new Point(6, 5),
            new Point(5, 6),
            new Point(6, 6)
        };

        var startPoint = new Point(0, 0);

        // Act
        var result = points.FloodFill(startPoint);

        // Assert
        result.Should()
              .BeEquivalentTo(
                  new[]
                  {
                      new Point(0, 0),
                      new Point(1, 0),
                      new Point(0, 1),
                      new Point(1, 1),
                      new Point(2, 0),
                      new Point(0, 2),
                      new Point(2, 2),
                      new Point(3, 0),
                      new Point(3, 1),
                      new Point(3, 2),
                      new Point(3, 3)
                  });
    }

    [Fact]
    public void FloodFill_ShouldReturnSinglePoint_WhenStartingWithSinglePoint()
    {
        // Arrange
        var points = new[]
        {
            new Point(0, 0)
        };

        var startPoint = new Point(0, 0);

        // Act
        var result = points.FloodFill(startPoint);

        // Assert
        result.Should()
              .BeEquivalentTo(
                  new[]
                  {
                      new Point(0, 0)
                  });
    }

    [Fact]
    public void GenerateCardinalPoints_ShouldGenerateNoPoints_WhenDirectionIsInvalid()
    {
        // Arrange
        var startPoint = new Point(0, 0);

        // Act
        var result = startPoint.GenerateCardinalPoints(Direction.Invalid);

        // Assert
        result.Should()
              .BeEmpty();
    }

    [Theory]
    [MemberData(nameof(GenerateCardinalPointsTestData))]
    public void GenerateCardinalPoints_ShouldGeneratePoints(
        int startX,
        int startY,
        Direction direction,
        int radius,
        Point[] expectedPoints)
    {
        // Arrange
        var startPoint = new Point(startX, startY);

        // Act
        var result = startPoint.GenerateCardinalPoints(direction, radius);

        // Assert
        result.Should()
              .BeEquivalentTo(expectedPoints);
    }

    [Fact]
    public void GenerateCardinalPoints_ShouldGeneratePointsInAllDirections_WhenDirectionIsAll()
    {
        // Arrange
        var startPoint = new Point(0, 0);

        var expectedPoints = new[]
        {
            new Point(0, 1),
            new Point(1, 0),
            new Point(0, -1),
            new Point(-1, 0)
        };

        // Act
        var result = startPoint.GenerateCardinalPoints();

        // Assert
        result.Should()
              .BeEquivalentTo(expectedPoints);
    }

    [Fact]
    public void GenerateCardinalPoints_ShouldGeneratePointsInSingleDirection_WhenDirectionIsNotAll()
    {
        // Arrange
        var startPoint = new Point(2, 2);

        var expectedPoints = new[]
        {
            new Point(2, 1),
            new Point(2, 0),
            new Point(2, -1)
        };

        // Act
        var result = startPoint.GenerateCardinalPoints(Direction.Up, 3);

        // Assert
        result.Should()
              .BeEquivalentTo(expectedPoints);
    }

    [Fact]
    public void GenerateCardinalPoints_ShouldThrowException_WhenRadiusIsNotPositive()
    {
        // Arrange
        var startPoint = new Point(0, 0);

        // Act
        var func = () => startPoint.GenerateCardinalPoints(Direction.All, 0);

        func.Enumerating()
            .Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithMessage("*radius must be positive*");
    }

    public static IEnumerable<object[]> GenerateCardinalPointsTestData()
    {
        yield return
        [
            0,
            0,
            Direction.All,
            1,
            new[]
            {
                new Point(0, 1),
                new Point(1, 0),
                new Point(0, -1),
                new Point(-1, 0)
            }
        ];

        yield return
        [
            2,
            2,
            Direction.Up,
            3,
            new[]
            {
                new Point(2, 1),
                new Point(2, 0),
                new Point(2, -1)
            }
        ];
    }

    [Fact]
    public void GenerateIntercardinalPoints_ShouldGenerateNoPoints_WhenDirectionIsInvalid()
    {
        // Arrange
        var startPoint = new Point(0, 0);

        // Act
        var result = startPoint.GenerateIntercardinalPoints(Direction.Invalid);

        // Assert
        result.Should()
              .BeEmpty();
    }

    [Fact]
    public void GenerateIntercardinalPoints_WithDirectionAll_ReturnsExpectedPoints()
    {
        var start = new Point(0, 0);

        var result = start.GenerateIntercardinalPoints(Direction.All, 3)
                          .ToList();

        result.Count
              .Should()
              .Be(12);

        result.Should()
              .Contain(new Point(-1, -1));

        result.Should()
              .Contain(new Point(1, -1));

        result.Should()
              .Contain(new Point(1, 1));

        result.Should()
              .Contain(new Point(-1, 1));

        result.Should()
              .Contain(new Point(-2, -2));

        result.Should()
              .Contain(new Point(2, -2));

        result.Should()
              .Contain(new Point(2, 2));

        result.Should()
              .Contain(new Point(-2, 2));

        result.Should()
              .Contain(new Point(-3, -3));

        result.Should()
              .Contain(new Point(3, -3));

        result.Should()
              .Contain(new Point(3, 3));

        result.Should()
              .Contain(new Point(-3, 3));
    }

    [Fact]
    public void GenerateIntercardinalPoints_WithDirectionDown_ReturnsExpectedPoints()
    {
        var start = new Point(0, 0);

        var result = start.GenerateIntercardinalPoints(Direction.Down, 5)
                          .ToList();

        result.Count
              .Should()
              .Be(10);

        result.Should()
              .Contain(new Point(1, 1));

        result.Should()
              .Contain(new Point(-1, 1));

        result.Should()
              .Contain(new Point(2, 2));

        result.Should()
              .Contain(new Point(-2, 2));

        result.Should()
              .Contain(new Point(3, 3));

        result.Should()
              .Contain(new Point(-3, 3));

        result.Should()
              .Contain(new Point(4, 4));

        result.Should()
              .Contain(new Point(-4, 4));

        result.Should()
              .Contain(new Point(5, 5));

        result.Should()
              .Contain(new Point(-5, 5));
    }

    [Fact]
    public void GenerateIntercardinalPoints_WithDirectionInvalid_ReturnsNoPoints()
    {
        var start = new Point(0, 0);

        var result = start.GenerateIntercardinalPoints(Direction.Invalid)
                          .ToList();

        result.Count
              .Should()
              .Be(0);
    }

    [Fact]
    public void GenerateIntercardinalPoints_WithDirectionLeft_ReturnsExpectedPoints()
    {
        var start = new Point(0, 0);

        var result = start.GenerateIntercardinalPoints(Direction.Left, 3)
                          .ToList();

        result.Count
              .Should()
              .Be(6);

        result.Should()
              .Contain(new Point(-1, -1));

        result.Should()
              .Contain(new Point(-1, 1));

        result.Should()
              .Contain(new Point(-2, -2));

        result.Should()
              .Contain(new Point(-2, 2));

        result.Should()
              .Contain(new Point(-3, -3));

        result.Should()
              .Contain(new Point(-3, 3));
    }

    [Fact]
    public void GenerateIntercardinalPoints_WithDirectionRight_ReturnsExpectedPoints()
    {
        var start = new Point(0, 0);

        var result = start.GenerateIntercardinalPoints(Direction.Right, 3)
                          .ToList();

        result.Count
              .Should()
              .Be(6);

        result.Should()
              .Contain(new Point(1, -1));

        result.Should()
              .Contain(new Point(1, 1));

        result.Should()
              .Contain(new Point(2, -2));

        result.Should()
              .Contain(new Point(2, 2));

        result.Should()
              .Contain(new Point(3, -3));

        result.Should()
              .Contain(new Point(3, 3));
    }

    [Fact]
    public void GenerateIntercardinalPoints_WithDirectionUp_ReturnsExpectedPoints()
    {
        var start = new Point(0, 0);

        var result = start.GenerateIntercardinalPoints(Direction.Up, 2)
                          .ToList();

        result.Count
              .Should()
              .Be(4);

        result.Should()
              .Contain(new Point(-1, -1));

        result.Should()
              .Contain(new Point(1, -1));

        result.Should()
              .Contain(new Point(-2, -2));

        result.Should()
              .Contain(new Point(2, -2));
    }

    [Theory]
    [MemberData(nameof(GetDirectPathTestData))]
    public void GetDirectPath_ShouldGenerateDirectPath(Point start, Point end, Point[] expectedPath)
    {
        // Act
        var result = start.GetDirectPath(end);

        // Assert
        result.Should()
              .BeSubsetOf(expectedPath);
    }

    public static IEnumerable<object[]> GetDirectPathTestData()
    {
        yield return
        [
            new Point(0, 0),
            new Point(0, 0),
            new[]
            {
                new Point(0, 0)
            }
        ];

        yield return
        [
            new Point(0, 0),
            new Point(2, 2),
            new[]
            {
                new Point(0, 0),
                new Point(0, 1),
                new Point(1, 0),
                new Point(1, 1),
                new Point(1, 2),
                new Point(2, 1),
                new Point(2, 2)
            }
        ];

        yield return
        [
            new Point(1, 1),
            new Point(3, 1),
            new[]
            {
                new Point(1, 1),
                new Point(2, 1),
                new Point(3, 1)
            }
        ];
    }

    //@formatter:off
    [Theory]
    // Positive test cases
    [InlineData(0, 0, 1, 1, Direction.Up, true)]
    [InlineData(0, 0, 1, 1, Direction.Left, true)]
    [InlineData(0, 0, 1, 1, Direction.Right, false)]
    [InlineData(0, 0, 1, 1, Direction.Down, false)]
    [InlineData(0, 0, -1, -1, Direction.Up, false)]
    [InlineData(0, 0, -1, -1, Direction.Left, false)]
    [InlineData(0, 0, -1, -1, Direction.Right, true)]
    [InlineData(0, 0, -1, -1, Direction.Down, true)]
    [InlineData(0, 0, 0, 0, Direction.Invalid, false)]
    //@formatter:on
    public void IsInterCardinalTo_Should_Return_Correct_Result(
        int startX,
        int startY,
        int otherX,
        int otherY,
        Direction direction,
        bool expectedResult)
    {
        // Arrange
        var start = new Point(startX, startY);
        var other = new Point(otherX, otherY);

        // Act
        var result = start.IsInterCardinalTo(other, direction);

        // Assert
        result.Should()
              .Be(expectedResult);
    }

    //@formatter:off
    [Theory]
    [InlineData(0, 0, 0, 1, 0, 1)]   // Offset towards North (Up)
    [InlineData(0, 0, 1, 0, 1, 0)]   // Offset towards East (Right)
    [InlineData(0, 0, 0, -1, 0, -1)] // Offset towards South (Down)
    [InlineData(0, 0, -1, 0, -1, 0)] // Offset towards West (Left)
    //@formatter:on
    public void OffsetTowards_Should_Offset_Correctly(
        int startX,
        int startY,
        int otherX,
        int otherY,
        int expectedOffsetX,
        int expectedOffsetY)
    {
        // Arrange
        var point = new Point(startX, startY);
        var other = new Point(otherX, otherY);
        var expectedOffset = new Point(expectedOffsetX, expectedOffsetY);

        // Act
        var result = point.OffsetTowards(other);

        // Assert
        result.Should()
              .BeEquivalentTo(expectedOffset);
    }

    [Fact]
    public void OffsetTowards_Should_Return_Correct_Offset()
    {
        // Arrange
        var point = new Point(0, 0);
        var other = new Point(5, 5);

        var expectedOffsets = new[]
        {
            new Point(1, 0),
            new Point(0, 1)
        };

        // Act
        var result = point.OffsetTowards(other);

        // Assert
        expectedOffsets.Should()
                       .Contain(result);
    }

    //@formatter:off
    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0)]           // Same start and end point
    [InlineData(0, 0, 0, 5, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5)]           // Vertical line, upwards
    [InlineData(0, 0, 5, 0, 1, 0, 2, 0, 3, 0, 4, 0, 5, 0)]           // Horizontal line, rightwards
    [InlineData(0, 0, 5, 5, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5)]       // Diagonal line, ascending
    [InlineData(5, 5, 0, 0, 5, 5, 4, 4, 3, 3, 2, 2, 1, 1, 0, 0)]   // Diagonal line, descending
    [InlineData(1, 1, 4, 2, 1, 1, 2, 1, 3, 2, 4, 2)]               // Sloped line, positive slope
    [InlineData(4, 2, 1, 1, 4, 2, 3, 2, 2, 1, 1, 1)]               // Sloped line, negative slope
    [InlineData(1, 1, 1, 5, 1, 2, 1, 3, 1, 4, 1, 5)]               // Vertical line, downwards
    [InlineData(1, 1, 5, 1, 2, 1, 3, 1, 4, 1, 5, 1)]               // Horizontal line, rightwards
    [InlineData(1, 1, 5, 5, 2, 2, 3, 3, 4, 4, 5, 5)]           // Diagonal line, ascending
    [InlineData(5, 5, 1, 1, 5, 5, 4, 4, 3, 3, 2, 2, 1, 1)]       // Diagonal line, descending
    //@formatter:on
    public void RayTraceTo_Should_Generate_All_Points_Between_Start_And_End(
        int startX,
        int startY,
        int endX,
        int endY,
        params int[] expectedPoints)
    {
        // Arrange
        var start = new Point(startX, startY);
        var end = new Point(endX, endY);

        // Act
        var result = start.RayTraceTo(end);

        // Assert
        result.Should()
              .ContainInOrder(
                  expectedPoints.Chunk(2)
                                .Select(pts => new Point(pts[0], pts[1])));
    }

    //@formatter:off
    [Theory]
    [InlineData(0, 0, 0)]                       // Zero distance, single point
    [InlineData(0, 0, 1, 0, 0, 1, 0, 0, 1, -1, 0, 0, -1)]    // Distance 1, spiral search
    [InlineData(0, 0, 2, 0, 0, 1, 0, 0, 1, -1, 0, 0, -1, 1, -1, 2, 0, 1, 1, 0, 2, -1, 1, -2, 0, -1, -1, 0, -2)] // Distance 2, spiral search
    //@formatter:on
    public void SpiralSearch_Should_Generate_Points_In_Spiral_Pattern(
        int startX,
        int startY,
        int maxRadius,
        params int[] expectedPoints)
    {
        // Arrange
        var start = new Point(startX, startY);

        // Act
        var result = start.SpiralSearch(maxRadius);

        // Assert
        result.Should()
              .ContainInOrder(
                  expectedPoints.Chunk(2)
                                .Select(pts => new Point(pts[0], pts[1])));
    }

    [Theory]
    [InlineData(
        Direction.Down,
        2,
        3,
        3,
        2,
        2,
        2,
        1,
        2,
        2,
        1)]
    [InlineData(
        Direction.Left,
        1,
        2,
        2,
        1,
        2,
        2,
        2,
        3,
        3,
        2)]
    [InlineData(
        Direction.Up,
        2,
        1,
        1,
        2,
        2,
        2,
        3,
        2,
        2,
        3)]
    [InlineData(
        Direction.Right,
        3,
        2,
        2,
        3,
        2,
        2,
        2,
        1,
        1,
        2)]
    public void WithConsistentDirectionBias_Should_Order_Points_Correctly(Direction direction, params int[] expectedOrder)
    {
        // Arrange
        var points = expectedOrder.Chunk(2)
                                  .Select(pts => new Point(pts[0], pts[1]))
                                  .ToList();

        // Act
        var result = points.WithConsistentDirectionBias(direction);

        // Assert
        result.Should()
              .ContainInOrder(points);
    }

    //@formatter:off
    [Theory]
    [InlineData(Direction.Down, 1, 2, 2, 1, 3, 0)]             // Sort points by Y in ascending order (Up)
    [InlineData(Direction.Left, 1, 2, 2, 1, 3, 0)]         // Sort points by X in descending order (Right)
    [InlineData(Direction.Up, 3, 0, 2, 1, 1, 2)]          // Sort points by Y in descending order (Down)
    [InlineData(Direction.Right, 3, 0, 2, 1, 1, 2)]          // Sort points by X in ascending order (Left)
    //@formatter:on
    public void WithDirectionBias_Should_Order_Points_Correctly(Direction direction, params int[] expectedOrder)
    {
        // Arrange
        var points = expectedOrder.Chunk(2)
                                  .Select(pts => new Point(pts[0], pts[1]))
                                  .ToList();

        // Act
        var result = points.WithDirectionBias(direction);

        // Assert
        result.Should()
              .ContainInOrder(points);
    }
}