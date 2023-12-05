using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using FluentAssertions;
using Xunit;

namespace Chaos.Extensions.Geometry.Tests;

public sealed class CircleExtensionsTests
{
    [Fact]
    public void CalculateIntersectionEntryPoint_Should_Return_Intersection_Point_When_Line_Intersects_Circle()
    {
        // Arrange
        var circle = new Circle(new Point(0, 0), 5);
        var lineStart = new Point(-10, 0);
        var lineEnd = new Point(10, 0);
        var expectedIntersectionPoint = new Point(-5, 0);

        // Act
        var intersectionPoint = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        intersectionPoint.Should()
                         .NotBeNull();

        intersectionPoint.Should()
                         .Be(expectedIntersectionPoint);
    }

    [Fact]
    public void CalculateIntersectionEntryPoint_Should_Return_Linestart_When_Line_Starts_Inside_Circle()
    {
        // Arrange
        var circle = new Circle(new Point(0, 0), 5);
        var lineStart = new Point(2, 2);
        var lineEnd = new Point(8, 8);
        var expectedIntersectionPoint = new Point(2, 2);

        // Act
        var intersectionPoint = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        intersectionPoint.Should()
                         .NotBeNull();

        intersectionPoint.Should()
                         .Be(expectedIntersectionPoint);
    }

    [Fact]
    public void CalculateIntersectionEntryPoint_Should_Return_Null_When_Line_Does_Not_Intersect_Circle()
    {
        // Arrange
        var circle = new Circle(new Point(0, 0), 5);
        var lineStart = new Point(10, 10);
        var lineEnd = new Point(20, 20);

        // Act
        var intersectionPoint = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        intersectionPoint.Should()
                         .BeNull();
    }

    [Fact]
    public void Contains_WhenCirclesIntersect_ShouldReturnFalse()
    {
        var circle = new Circle(new Point(5, 5), 5);
        var other = new Circle(new Point(7, 7), 4);

        circle.Contains(other)
              .Should()
              .BeFalse();
    }

    [Fact]
    public void Contains_WhenLargerCircleOutside_ShouldReturnFalse()
    {
        var circle = new Circle(new Point(5, 5), 3);
        var other = new Circle(new Point(5, 5), 5);

        circle.Contains(other)
              .Should()
              .BeFalse();
    }

    [Fact]
    public void Contains_WhenNullCircle_ShouldThrowArgumentNullException()
    {
        var circle = new Circle(new Point(5, 5), 3);
        ICircle? other = null;

        Action act = () => circle.Contains(other!);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Fact]
    public void Contains_WhenNullPoint_ShouldThrowArgumentNullException()
    {
        var circle = new Circle(new Point(5, 5), 3);
        IPoint? point = null;

        Action act = () => circle.Contains(point!);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Fact]
    public void Contains_WhenPointIsCenter_ShouldReturnTrue()
    {
        var circle = new Circle(new Point(5, 5), 3);
        var point = new Point(5, 5);

        circle.Contains(point)
              .Should()
              .BeTrue();
    }

    [Fact]
    public void Contains_WhenPointIsInside_ShouldReturnTrue()
    {
        var circle = new Circle(new Point(5, 5), 5);
        var point = new Point(6, 6);

        circle.Contains(point)
              .Should()
              .BeTrue();
    }

    [Fact]
    public void Contains_WhenPointIsOnEdge_ShouldReturnTrue()
    {
        var circle = new Circle(new Point(5, 5), 5);
        var point = new Point(5, 0);

        circle.Contains(point)
              .Should()
              .BeTrue();
    }

    [Fact]
    public void Contains_WhenPointIsOutside_ShouldReturnFalse()
    {
        var circle = new Circle(new Point(5, 5), 3);
        var point = new Point(10, 10);

        circle.Contains(point)
              .Should()
              .BeFalse();
    }

    [Fact]
    public void Contains_WhenSameCircle_ShouldReturnTrue()
    {
        var circle = new Circle(new Point(5, 5), 3);

        circle.Contains(circle)
              .Should()
              .BeTrue();
    }

    [Fact]
    public void Contains_WhenSmallerCircleInside_ShouldReturnTrue()
    {
        var circle = new Circle(new Point(5, 5), 5);
        var other = new Circle(new Point(5, 5), 3);

        circle.Contains(other)
              .Should()
              .BeTrue();
    }

    [Fact]
    public void EdgeDistanceFrom_Should_Return_Positive_Value_When_Other_Point_Is_Outside_Circle()
    {
        // Arrange
        var circle = new Circle(new Point(0, 0), 5);
        var otherPoint = new Point(10, 10);

        // Act
        var edgeDistance = circle.EdgeDistanceFrom(otherPoint);

        // Assert
        edgeDistance.Should()
                    .BeGreaterThan(0);
    }

    [Fact]
    public void EdgeDistanceFrom_Should_Return_Zero_When_Other_Point_Is_Center_Of_Circle()
    {
        // Arrange
        var circle = new Circle(new Point(0, 0), 5);
        var otherPoint = new Point(0, 0);

        // Act
        var edgeDistance = circle.EdgeDistanceFrom(otherPoint);

        // Assert
        edgeDistance.Should()
                    .Be(0);
    }

    [Fact]
    public void EdgeDistanceFrom_Should_Return_Zero_When_Other_Point_Is_Inside_Circle()
    {
        // Arrange
        var circle = new Circle(new Point(0, 0), 5);
        var otherPoint = new Point(2, 2);

        // Act
        var edgeDistance = circle.EdgeDistanceFrom(otherPoint);

        // Assert
        edgeDistance.Should()
                    .Be(0);
    }

    [Fact]
    public void EdgeToEdgeDistanceFrom_Should_Return_Positive_Value_When_Circles_Do_Not_Overlap()
    {
        // Arrange
        var circle1 = new Circle(new Point(0, 0), 5);
        var circle2 = new Circle(new Point(15, 15), 5);

        // Act
        var edgeToEdgeDistance = circle1.EdgeToEdgeDistanceFrom(circle2);

        // Assert
        edgeToEdgeDistance.Should()
                          .BeGreaterThan(0);
    }

    [Fact]
    public void EdgeToEdgeDistanceFrom_Should_Return_Zero_When_Circles_Overlap()
    {
        // Arrange
        var circle1 = new Circle(new Point(0, 0), 5);
        var circle2 = new Circle(new Point(4, 4), 5);

        // Act
        var edgeToEdgeDistance = circle1.EdgeToEdgeDistanceFrom(circle2);

        // Assert
        edgeToEdgeDistance.Should()
                          .Be(0);
    }

    [Fact]
    public void EdgeToEdgeDistanceFrom_Should_Return_Zero_When_Circles_Touch_Each_Other()
    {
        // Arrange
        var circle1 = new Circle(new Point(0, 0), 5);
        var circle2 = new Circle(new Point(10, 0), 5);

        // Act
        var edgeToEdgeDistance = circle1.EdgeToEdgeDistanceFrom(circle2);

        // Assert
        edgeToEdgeDistance.Should()
                          .Be(0);
    }

    [Fact]
    public void GetOutline_GivenCircle_ShouldReturnCorrectOutline()
    {
        var circle = new Circle(new Point(0, 0), 1);

        var outline = circle.GetOutline();

        outline.Should()
               .BeEquivalentTo(
                   new Point[]
                   {
                       new(0, 1), // North
                       new(1, 1), // Northeast
                       new(1, 0), // East
                       new(1, -1), // Southeast
                       new(0, -1), // South
                       new(-1, -1), // Southwest
                       new(-1, 0), // West
                       new(-1, 1) // Northwest
                   });
    }

    [Fact]
    public void GetOutline_GivenCircleWithCenterOffset_ShouldReturnCorrectOutline()
    {
        var circle = new Circle(new Point(1, 1), 1);

        var outline = circle.GetOutline();

        outline.Should()
               .BeEquivalentTo(
                   new Point[]
                   {
                       new(1, 2),
                       new(2, 2),
                       new(2, 1),
                       new(2, 0),
                       new(1, 0),
                       new(0, 0),
                       new(0, 1),
                       new(0, 2)
                   });
    }

    [Fact]
    public void GetOutline_GivenLargeCircle_ShouldReturnCorrectOutline()
    {
        var circle = new Circle(new Point(0, 0), 2);

        var outline = circle.GetOutline();

        outline.Should()
               .BeEquivalentTo(
                   new Point[]
                   {
                       new(-2, 0),
                       new(-2, -1),
                       new(-1, -2),
                       new(0, -2),
                       new(1, -2),
                       new(2, -1),
                       new(2, 0),
                       new(2, 1),
                       new(1, 2),
                       new(0, 2),
                       new(-1, 2),
                       new(-2, 1)
                   });
    }

    [Fact]
    public void GetPoints_GivenCircleWithRadiusOne_ShouldReturnFivePoints()
    {
        var circle = new Circle(new Point(0, 0), 1);

        var points = circle.GetPoints()
                           .ToList();

        points.Should()
              .HaveCount(5);

        points.Should()
              .BeEquivalentTo(
                  new Point[]
                  {
                      new(0, 0),
                      new(0, 1),
                      new(1, 0),
                      new(0, -1),
                      new(-1, 0)
                  });
    }

    [Fact]
    public void GetPoints_GivenCircleWithRadiusTwo_ShouldReturnThirteenPoints()
    {
        var circle = new Circle(new Point(0, 0), 2);

        var points = circle.GetPoints()
                           .ToList();

        points.Should()
              .HaveCount(13);

        points.Should()
              .BeEquivalentTo(
                  new Point[]
                  {
                      new(0, 0),
                      new(-1, 0),
                      new(1, 0),
                      new(0, 1),
                      new(0, -1),
                      new(-1, -1),
                      new(1, 1),
                      new(-1, 1),
                      new(1, -1),
                      new(-2, 0),
                      new(2, 0),
                      new(0, -2),
                      new(0, 2)
                  });
    }

    [Fact]
    public void GetPoints_GivenCircleWithZeroRadius_ShouldReturnOnlyCenterPoint()
    {
        var circle = new Circle(new Point(0, 0), 0);

        var points = circle.GetPoints()
                           .ToList();

        points.Should()
              .HaveCount(1);

        points.Should()
              .BeEquivalentTo(
                  new[]
                  {
                      new Point(0, 0)
                  });
    }

    [Fact]
    public void GetRandomPoint_Should_Generate_Points_Within_Circle()
    {
        // Arrange
        //circle should be smallish
        var circle = new Circle(new Point(0, 0), 5);

        //high enough it rules out probability
        //low enough it runs fast
        const int ITERATIONS = 10000;

        for (var i = 0; i < ITERATIONS; i++)
        {
            var randomPoint = circle.GetRandomPoint();

            // Assert
            var distanceFromCenter = circle.Center.EuclideanDistanceFrom(randomPoint);

            distanceFromCenter.Should()
                              .BeLessOrEqualTo(circle.Radius);
        }
    }

    [Fact]
    public void GetRandomPoint_Should_Generate_Random_Points()
    {
        // Arrange
        var circle = new Circle(new Point(0, 0), 3);
        const int ITERATIONS = 10000;

        // Act
        var set = new HashSet<Point>();

        for (var i = 0; i < ITERATIONS; i++)
            set.Add(circle.GetRandomPoint());

        // Assert
        set.Should()
           .HaveCount(25);
    }

    [Fact]
    public void Intersects_ShouldReturnFalse_WhenCirclesDoNotIntersect()
    {
        // Arrange
        var circle1 = new Circle(new Point(0, 0), 5);
        var circle2 = new Circle(new Point(10, 10), 4);

        // Act
        var intersects = circle1.Intersects(circle2);

        // Assert
        intersects.Should()
                  .BeFalse();
    }

    [Fact]
    public void Intersects_ShouldReturnTrue_WhenCircleContainsAnotherCircle()
    {
        // Arrange
        var circle1 = new Circle(new Point(0, 0), 10);
        var circle2 = new Circle(new Point(0, 0), 5);

        // Act
        var intersects = circle1.Intersects(circle2);

        // Assert
        intersects.Should()
                  .BeTrue();
    }

    [Fact]
    public void Intersects_ShouldReturnTrue_WhenCirclesIntersect()
    {
        // Arrange
        var circle1 = new Circle(new Point(0, 0), 5);
        var circle2 = new Circle(new Point(3, 0), 4);

        // Act
        var intersects = circle1.Intersects(circle2);

        // Assert
        intersects.Should()
                  .BeTrue();
    }
}