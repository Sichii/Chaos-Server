// ReSharper disable ArrangeAttributes

#region
using Chaos.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Extensions.Geometry.Tests;

public sealed class PointExtensionsTests
{
    [Test]
    public void ConalSearch_MaxDistanceGreaterThanOne_ReturnsAllPointsWithinCone()
    {
        var startingPoint = new Point(0, 0);

        var points = startingPoint.ConalSearch(Direction.Up, 2)
                                  .ToList();

        points.Should()
              .HaveCount(8);

        points.Should()
              .BeEquivalentTo(
                  [
                      new Point(-2, -2),
                      new Point(-1, -1),
                      new Point(0, -2),
                      new Point(1, -1),
                      new Point(2, -2),
                      new Point(-1, -2),
                      new Point(1, -2),
                      new Point(0, -1)
                  ]);
    }

    [Test]
    public void ConalSearch_MaxDistanceOne_ReturnsThreePointsInSpecifiedDirection()
    {
        var startingPoint = new Point(0, 0);

        var points = startingPoint.ConalSearch(Direction.Up, 1)
                                  .ToList();

        points.Should()
              .HaveCount(3);

        points.Should()
              .BeEquivalentTo(
                  [
                      new Point(-1, -1),
                      new Point(0, -1),
                      new Point(1, -1)
                  ]);
    }

    [Test]
    public void ConalSearch_MaxDistanceZero_ReturnsEmpty()
    {
        var startingPoint = new Point(0, 0);

        var points = startingPoint.ConalSearch(Direction.Up, 0)
                                  .ToList();

        points.Should()
              .BeEmpty();
    }

    //@formatter:off
    [Test]
    [Arguments(1, 1, Direction.Up, 1, 1, 0)]
    [Arguments(1, 1, Direction.Up, 2, 1, -1)]
    [Arguments(1, 1, Direction.Right, 1, 2, 1)]
    [Arguments(1, 1, Direction.Right, 2, 3, 1)]
    [Arguments(1, 1, Direction.Down, 1, 1, 2)]
    [Arguments(1, 1, Direction.Down, 2, 1, 3)]
    [Arguments(1, 1, Direction.Left, 1, 0, 1)]
    [Arguments(1, 1, Direction.Left, 2, -1, 1)]
    [Arguments(0, 0, Direction.Up, 1, 0, -1)]
    [Arguments(0, 0, Direction.Right, 1, 1, 0)]
    [Arguments(0, 0, Direction.Down, 1, 0, 1)]
    [Arguments(0, 0, Direction.Left, 1, -1, 0)]
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

    [Test]
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
    [Test]
    [Arguments(0, 0, 0, 0, new[] {Direction.Invalid})]
    [Arguments(0, 0, 0, 1, new[] {Direction.Up})]
    [Arguments(0, 0, 0, -1, new[] {Direction.Down})]
    [Arguments(0, 0, -1, 0, new[] {Direction.Right})]
    [Arguments(0, 0, 1, 0, new[] {Direction.Left})]
    [Arguments(0, 0, -1, 1, new[] {Direction.Up, Direction.Right})]
    [Arguments(0, 0, -1, -1, new[] {Direction.Down, Direction.Right})]
    [Arguments(0, 0, 1, 1, new[] {Direction.Up, Direction.Left})]
    [Arguments(0, 0, 1, -1, new[] {Direction.Down, Direction.Left})]
    //@formatter:on
    public void DirectionalRelationTo_VariousPoints_ReturnsExpectedDirection(
        int startX,
        int startY,
        int endX,
        int endY,
        params IEnumerable<Direction> expected)
    {
        var start = new Point(startX, startY);
        var end = new Point(endX, endY);

        var direction = start.DirectionalRelationTo(end);

        expected.Should()
                .Contain(direction);
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 0, 0, 0)]
    [Arguments(0, 0, 0, 1, 1)]
    [Arguments(0, 0, 1, 0, 1)]
    [Arguments(0, 0, 1, 1, 2)]
    [Arguments(1, 1, 1, 1, 0)]
    [Arguments(1, 1, 1, 2, 1)]
    [Arguments(1, 1, 2, 1, 1)]
    [Arguments(1, 1, 2, 2, 2)]
    [Arguments(-1, -1, 1, 1, 4)]
    [Arguments(-1, -1, -1, -1, 0)]
    [Arguments(-1, -1, -2, -1, 1)]
    [Arguments(-1, -1, -1, -2, 1)]
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
    [Test]
    [Arguments(0, 0, 0, 0, 0)]
    [Arguments(0, 0, 0, 1, 1)]
    [Arguments(0, 0, 1, 0, 1)]
    [Arguments(0, 0, 1, 1, 1.4142135623730951f)]
    [Arguments(1, 1, 1, 1, 0)]
    [Arguments(1, 1, 1, 2, 1)]
    [Arguments(1, 1, 2, 1, 1)]
    [Arguments(1, 1, 2, 2, 1.4142135623730951f)]
    [Arguments(-1, -1, 1, 1, 2.8284271247461903f)]
    [Arguments(-1, -1, -1, -1, 0)]
    [Arguments(-1, -1, -2, -1, 1)]
    [Arguments(-1, -1, -1, -2, 1)]
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

    [Test]
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
              .BeEquivalentTo([startPoint]);
    }

    [Test]
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

    [Test]
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
                  [
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
                  ]);
    }

    [Test]
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
              .BeEquivalentTo([new Point(0, 0)]);
    }

    [Test]
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

    [Test]
    [MethodDataSource(nameof(GenerateCardinalPointsTestData))]
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

    [Test]
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

    [Test]
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

    [Test]
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

    public static IEnumerable<(int, int, Direction, int, Point[])> GenerateCardinalPointsTestData()
        =>
        [
            (0, 0, Direction.All, 1, [
                                         new Point(0, 1),
                                         new Point(1, 0),
                                         new Point(0, -1),
                                         new Point(-1, 0)
                                     ]),
            (2, 2, Direction.Up, 3, [
                                        new Point(2, 1),
                                        new Point(2, 0),
                                        new Point(2, -1)
                                    ])
        ];

    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
    public void GenerateIntercardinalPoints_WithDirectionInvalid_ReturnsNoPoints()
    {
        var start = new Point(0, 0);

        var result = start.GenerateIntercardinalPoints(Direction.Invalid)
                          .ToList();

        result.Count
              .Should()
              .Be(0);
    }

    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
    [MethodDataSource(nameof(GetDirectPathTestData))]
    public void GetDirectPath_ShouldGenerateDirectPath(Point start, Point end, Point[] expectedPath)
    {
        // Act
        var result = start.GetDirectPath(end);

        // Assert
        result.Should()
              .BeSubsetOf(expectedPath);
    }

    public static IEnumerable<(Point, Point, Point[])> GetDirectPathTestData()
    {
        yield return (new Point(0, 0), new Point(0, 0), [new Point(0, 0)]);

        yield return (new Point(0, 0), new Point(2, 2), [
                                                            new Point(0, 0),
                                                            new Point(0, 1),
                                                            new Point(1, 0),
                                                            new Point(1, 1),
                                                            new Point(1, 2),
                                                            new Point(2, 1),
                                                            new Point(2, 2)
                                                        ]);

        yield return (new Point(1, 1), new Point(3, 1), [
                                                            new Point(1, 1),
                                                            new Point(2, 1),
                                                            new Point(3, 1)
                                                        ]);
    }

    //@formatter:off
    [Test]
    // Positive test cases
    [Arguments(0, 0, 1, 1, Direction.Up, true)]
    [Arguments(0, 0, 1, 1, Direction.Left, true)]
    [Arguments(0, 0, 1, 1, Direction.Right, false)]
    [Arguments(0, 0, 1, 1, Direction.Down, false)]
    [Arguments(0, 0, -1, -1, Direction.Up, false)]
    [Arguments(0, 0, -1, -1, Direction.Left, false)]
    [Arguments(0, 0, -1, -1, Direction.Right, true)]
    [Arguments(0, 0, -1, -1, Direction.Down, true)]
    [Arguments(0, 0, 0, 0, Direction.Invalid, false)]
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
    [Test]
    [Arguments(0, 0, 0, 1, 0, 1)]   // Offset towards North (Up)
    [Arguments(0, 0, 1, 0, 1, 0)]   // Offset towards East (Right)
    [Arguments(0, 0, 0, -1, 0, -1)] // Offset towards South (Down)
    [Arguments(0, 0, -1, 0, -1, 0)] // Offset towards West (Left)
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

    [Test]
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
    [Test]
    [Arguments(0, 0, 0, 0, new[] {0, 0})]           // Same start and end point
    [Arguments(0, 0, 0, 5, new[] {0, 1, 0, 2, 0, 3, 0, 4, 0, 5})]           // Vertical line, upwards
    [Arguments(0, 0, 5, 0, new[] {1, 0, 2, 0, 3, 0, 4, 0, 5, 0})]           // Horizontal line, rightwards
    [Arguments(0, 0, 5, 5, new[] {1, 1, 2, 2, 3, 3, 4, 4, 5, 5})]       // Diagonal line, ascending
    [Arguments(5, 5, 0, 0, new[] {5, 5, 4, 4, 3, 3, 2, 2, 1, 1, 0, 0})]   // Diagonal line, descending
    [Arguments(1, 1, 4, 2, new[] {1, 1, 2, 1, 3, 2, 4, 2})]               // Sloped line, positive slope
    [Arguments(4, 2, 1, 1, new[] {4, 2, 3, 2, 2, 1, 1, 1})]               // Sloped line, negative slope
    [Arguments(1, 1, 1, 5, new[] {1, 2, 1, 3, 1, 4, 1, 5})]               // Vertical line, downwards
    [Arguments(1, 1, 5, 1, new[] {2, 1, 3, 1, 4, 1, 5, 1})]               // Horizontal line, rightwards
    [Arguments(1, 1, 5, 5, new[] {2, 2, 3, 3, 4, 4, 5, 5})]           // Diagonal line, ascending
    [Arguments(5, 5, 1, 1, new[] {5, 5, 4, 4, 3, 3, 2, 2, 1, 1})]       // Diagonal line, descending
    //@formatter:on
    public void RayTraceTo_Should_Generate_All_Points_Between_Start_And_End(
        int startX,
        int startY,
        int endX,
        int endY,
        params IEnumerable<int> expectedPoints)
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
    [Test]
    [Arguments(0, 0, 0, new int[0])]                       // Zero distance, single point
    [Arguments(0, 0, 1, new[] {0, 0, 1, 0, 0, 1, -1, 0, 0, -1})]    // Distance 1, spiral search
    [Arguments(0, 0, 2, new[] {0, 0, 1, 0, 0, 1, -1, 0, 0, -1, 1, -1, 2, 0, 1, 1, 0, 2, -1, 1, -2, 0, -1, -1, 0, -2})] // Distance 2, spiral search
    //@formatter:on
    public void SpiralSearch_Should_Generate_Points_In_Spiral_Pattern(
        int startX,
        int startY,
        int maxRadius,
        params IEnumerable<int> expectedPoints)
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

    [Test]
    [Arguments(
        Direction.Down,
        new[]
        {
            2,
            3,
            3,
            2,
            2,
            2,
            1,
            2,
            2,
            1
        })]
    [Arguments(
        Direction.Left,
        new[]
        {
            1,
            2,
            2,
            1,
            2,
            2,
            2,
            3,
            3,
            2
        })]
    [Arguments(
        Direction.Up,
        new[]
        {
            2,
            1,
            1,
            2,
            2,
            2,
            3,
            2,
            2,
            3
        })]
    [Arguments(
        Direction.Right,
        new[]
        {
            3,
            2,
            2,
            3,
            2,
            2,
            2,
            1,
            1,
            2
        })]
    public void WithConsistentDirectionBias_Should_Order_Points_Correctly(Direction direction, params IEnumerable<int> expectedOrder)
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
    [Test]
    [Arguments(Direction.Down, new[] {1, 2, 2, 1, 3, 0})]             // Sort points by Y in ascending order (Up)
    [Arguments(Direction.Left, new[] {1, 2, 2, 1, 3, 0})]         // Sort points by X in descending order (Right)
    [Arguments(Direction.Up, new[] {3, 0, 2, 1, 1, 2})]          // Sort points by Y in descending order (Down)
    [Arguments(Direction.Right, new[] {3, 0, 2, 1, 1, 2})]          // Sort points by X in ascending order (Left)
    //@formatter:on
    public void WithDirectionBias_Should_Order_Points_Correctly(Direction direction, params IEnumerable<int> expectedOrder)
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