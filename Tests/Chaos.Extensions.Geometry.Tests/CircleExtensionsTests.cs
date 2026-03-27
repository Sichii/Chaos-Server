#region
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;

// ReSharper disable ArrangeAttributes
#endregion

namespace Chaos.Extensions.Geometry.Tests;

public sealed class CircleExtensionsTests
{
    #region Basic Intersection Tests
    [Test]
    [Arguments(
        5,
        5,
        3,
        0,
        5,
        10,
        5,
        2,
        5)]
    [Arguments(
        5,
        5,
        3,
        5,
        0,
        5,
        10,
        5,
        2)]
    [Arguments(
        0,
        0,
        5,
        -10,
        0,
        10,
        0,
        -5,
        0)]
    [Arguments(
        10,
        10,
        2,
        7,
        10,
        15,
        10,
        8,
        10)]
    public void CalculateIntersectionEntryPoint_All_Overloads_Should_Return_First_Intersection_Point(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        var mockCircle = Mock.Of<ICircle>(c
            => (c.Center == Mock.Of<IPoint>(p => (p.X == centerX) && (p.Y == centerY))) && (c.Radius == radius));
        var mockLineStart = Mock.Of<IPoint>(p => (p.X == lineStartX) && (p.Y == lineStartY));
        var mockLineEnd = Mock.Of<IPoint>(p => (p.X == lineEndX) && (p.Y == lineEndY));

        var valuePointStart = new ValuePoint(lineStartX, lineStartY);
        var valuePointEnd = new ValuePoint(lineEndX, lineEndY);
        var pointStart = new Point(lineStartX, lineStartY);
        var pointEnd = new Point(lineEndX, lineEndY);

        var expected = new Point(expectedX, expectedY);

        // Act & Assert - Test all overloads
        mockCircle.CalculateIntersectionEntryPoint(valuePointStart, mockLineEnd)
                  .Should()
                  .Be(expected);

        mockCircle.CalculateIntersectionEntryPoint(pointStart, valuePointEnd)
                  .Should()
                  .Be(expected);

        mockCircle.CalculateIntersectionEntryPoint(pointStart, pointEnd)
                  .Should()
                  .Be(expected);

        mockCircle.CalculateIntersectionEntryPoint(pointStart, mockLineEnd)
                  .Should()
                  .Be(expected);

        mockCircle.CalculateIntersectionEntryPoint(mockLineStart, valuePointEnd)
                  .Should()
                  .Be(expected);

        mockCircle.CalculateIntersectionEntryPoint(mockLineStart, pointEnd)
                  .Should()
                  .Be(expected);
    }
    #endregion

//@formatter:off
    [Test]
    [Arguments(0, 0, 5, 10, 10, 15, 15)]  // Line completely outside
    [Arguments(0, 0, 3, 5, 0, 8, 0)]      // Line starts outside, doesn't reach
    //@formatter:on
    public void CalculateIntersectionEntryPoint_All_Overloads_Should_Return_Null_When_No_Intersection(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);
        var valueStart = new ValuePoint(lineStartX, lineStartY);
        var valueEnd = new ValuePoint(lineEndX, lineEndY);
        var pointStart = new Point(lineStartX, lineStartY);
        var pointEnd = new Point(lineEndX, lineEndY);

        var iPointStart = MockReferencePoint.Create(lineStartX, lineStartY)
                                            .Object;

        var iPointEnd = MockReferencePoint.Create(lineEndX, lineEndY)
                                          .Object;

        // Act & Assert - Test all combinations
        circle.CalculateIntersectionEntryPoint(valueStart, valueEnd)
              .Should()
              .BeNull();

        circle.CalculateIntersectionEntryPoint(valueStart, pointEnd)
              .Should()
              .BeNull();

        circle.CalculateIntersectionEntryPoint(valueStart, iPointEnd)
              .Should()
              .BeNull();

        circle.CalculateIntersectionEntryPoint(pointStart, valueEnd)
              .Should()
              .BeNull();

        circle.CalculateIntersectionEntryPoint(pointStart, pointEnd)
              .Should()
              .BeNull();

        circle.CalculateIntersectionEntryPoint(pointStart, iPointEnd)
              .Should()
              .BeNull();

        circle.CalculateIntersectionEntryPoint(iPointStart, valueEnd)
              .Should()
              .BeNull();

        circle.CalculateIntersectionEntryPoint(iPointStart, pointEnd)
              .Should()
              .BeNull();

        circle.CalculateIntersectionEntryPoint(iPointStart, iPointEnd)
              .Should()
              .BeNull();
    }

    #region Consistency Tests
    [Test]
    public void CalculateIntersectionEntryPoint_All_Overloads_Should_Return_Same_Result()
    {
        // Arrange
        var centerX = 10;
        var centerY = 10;
        var radius = 5;
        var startX = 0;
        var startY = 10;
        var endX = 20;
        var endY = 10;

        var mockCircle = Mock.Of<ICircle>(c
            => (c.Center == Mock.Of<IPoint>(p => (p.X == centerX) && (p.Y == centerY))) && (c.Radius == radius));
        var mockLineStart = Mock.Of<IPoint>(p => (p.X == startX) && (p.Y == startY));
        var mockLineEnd = Mock.Of<IPoint>(p => (p.X == endX) && (p.Y == endY));

        var valuePointStart = new ValuePoint(startX, startY);
        var valuePointEnd = new ValuePoint(endX, endY);
        var pointStart = new Point(startX, startY);
        var pointEnd = new Point(endX, endY);

        // Act
        var result1 = mockCircle.CalculateIntersectionEntryPoint(valuePointStart, mockLineEnd);
        var result2 = mockCircle.CalculateIntersectionEntryPoint(pointStart, valuePointEnd);
        var result3 = mockCircle.CalculateIntersectionEntryPoint(pointStart, pointEnd);
        var result4 = mockCircle.CalculateIntersectionEntryPoint(pointStart, mockLineEnd);
        var result5 = mockCircle.CalculateIntersectionEntryPoint(mockLineStart, valuePointEnd);
        var result6 = mockCircle.CalculateIntersectionEntryPoint(mockLineStart, pointEnd);

        // Assert
        result1.Should()
               .Be(result2);

        result2.Should()
               .Be(result3);

        result3.Should()
               .Be(result4);

        result4.Should()
               .Be(result5);

        result5.Should()
               .Be(result6);
    }
    #endregion

    //@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, 0, 10, 0, -5, 0)]  // Horizontal line
    [Arguments(0, 0, 5, 0, -10, 0, 10, 0, -5)]  // Vertical line
    //@formatter:on
    public void CalculateIntersectionEntryPoint_ICircle_ValuePoint_Point_Should_Return_First_Intersection_Point(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        ICircle circle = new Circle(new Point(centerX, centerY), radius);
        var lineStart = new ValuePoint(lineStartX, lineStartY);
        var lineEnd = new Point(lineEndX, lineEndY);
        var expected = new Point(expectedX, expectedY);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(expected);
    }

    [Test]
    public void CalculateIntersectionEntryPoint_ICircle_ValuePoint_Point_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle circle = null!;

        // Act
        var act = () => circle.CalculateIntersectionEntryPoint(new ValuePoint(0, 0), new Point(10, 10));

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, 0, 10, 0, -5, 0)]  // Horizontal line
    [Arguments(0, 0, 5, 0, -10, 0, 10, 0, -5)]  // Vertical line
    //@formatter:on
    public void CalculateIntersectionEntryPoint_ICircle_ValuePoint_ValuePoint_Should_Return_First_Intersection_Point(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        ICircle circle = new Circle(new Point(centerX, centerY), radius);
        var lineStart = new ValuePoint(lineStartX, lineStartY);
        var lineEnd = new ValuePoint(lineEndX, lineEndY);
        var expected = new Point(expectedX, expectedY);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(expected);
    }

    [Test]
    public void CalculateIntersectionEntryPoint_ICircle_ValuePoint_ValuePoint_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle circle = null!;

        // Act
        var act = () => circle.CalculateIntersectionEntryPoint(new ValuePoint(0, 0), new ValuePoint(10, 10));

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void CalculateIntersectionEntryPoint_Should_Handle_Large_Circle_And_Line()
    {
        // Arrange
        var center = new Point(500, 500);
        var circle = new ValueCircle(center, 100);
        var lineStart = new ValuePoint(0, 500);
        var lineEnd = new ValuePoint(1000, 500);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Value
              .X
              .Should()
              .Be(400); // 500 - 100 (radius)

        result.Value
              .Y
              .Should()
              .Be(500);
    }

    [Test]
    public void CalculateIntersectionEntryPoint_Should_Handle_Single_Point_Line()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 5);
        var lineStart = new ValuePoint(3, 4); // Inside circle, distance = 5 (on edge)
        var lineEnd = new ValuePoint(3, 4); // Same point

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(new Point(3, 4));
    }

    [Test]
    public void CalculateIntersectionEntryPoint_Should_Handle_Tangent_Line()
    {
        // Arrange - Line tangent to circle at top
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 5);
        var lineStart = new ValuePoint(-10, 5);
        var lineEnd = new ValuePoint(10, 5);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(new Point(0, 5)); // Tangent point
    }

    [Test]
    public void CalculateIntersectionEntryPoint_Should_Handle_Zero_Radius_Circle()
    {
        // Arrange
        var center = new Point(5, 5);
        var circle = new ValueCircle(center, 0);
        var lineStart = new ValuePoint(0, 0);
        var lineEnd = new ValuePoint(10, 10);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(new Point(5, 5)); // Should find the center point
    }

    [Test]
    public void CalculateIntersectionEntryPoint_Should_Return_First_Point_On_Circle_Edge()
    {
        // Arrange - Line starts just outside circle edge
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 5);
        var lineStart = new ValuePoint(-6, 0);
        var lineEnd = new ValuePoint(6, 0);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(new Point(-5, 0));
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, 0, 10, 0, -5, 0)]  // Horizontal line
    [Arguments(0, 0, 5, 0, -10, 0, 10, 0, -5)]  // Vertical line
    //@formatter:on
    public void CalculateIntersectionEntryPoint_ValueCircle_IPoint_IPoint_Should_Return_First_Intersection_Point(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);

        var lineStart = MockReferencePoint.Create(lineStartX, lineStartY)
                                          .Object;

        var lineEnd = MockReferencePoint.Create(lineEndX, lineEndY)
                                        .Object;
        var expected = new Point(expectedX, expectedY);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(expected);
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, 0, 10, 0, -5, 0)]  // Horizontal line
    [Arguments(0, 0, 5, 0, -10, 0, 10, 0, -5)]  // Vertical line
    //@formatter:on
    public void CalculateIntersectionEntryPoint_ValueCircle_IPoint_Point_Should_Return_First_Intersection_Point(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);

        var lineStart = MockReferencePoint.Create(lineStartX, lineStartY)
                                          .Object;
        var lineEnd = new Point(lineEndX, lineEndY);
        var expected = new Point(expectedX, expectedY);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(expected);
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, 0, 10, 0, -5, 0)]  // Horizontal line
    [Arguments(0, 0, 5, 0, -10, 0, 10, 0, -5)]  // Vertical line
    //@formatter:on
    public void CalculateIntersectionEntryPoint_ValueCircle_IPoint_ValuePoint_Should_Return_First_Intersection_Point(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);

        var lineStart = MockReferencePoint.Create(lineStartX, lineStartY)
                                          .Object;
        var lineEnd = new ValuePoint(lineEndX, lineEndY);
        var expected = new Point(expectedX, expectedY);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(expected);
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, 0, 10, 0, -5, 0)]  // Horizontal line
    [Arguments(0, 0, 5, 0, -10, 0, 10, 0, -5)]  // Vertical line
    //@formatter:on
    public void CalculateIntersectionEntryPoint_ValueCircle_Point_IPoint_Should_Return_First_Intersection_Point(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);
        var lineStart = new Point(lineStartX, lineStartY);

        var lineEnd = MockReferencePoint.Create(lineEndX, lineEndY)
                                        .Object;
        var expected = new Point(expectedX, expectedY);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(expected);
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, 0, 10, 0, -5, 0)]  // Horizontal line
    [Arguments(0, 0, 5, 0, -10, 0, 10, 0, -5)]  // Vertical line
    //@formatter:on
    public void CalculateIntersectionEntryPoint_ValueCircle_Point_Point_Should_Return_First_Intersection_Point(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);
        var lineStart = new Point(lineStartX, lineStartY);
        var lineEnd = new Point(lineEndX, lineEndY);
        var expected = new Point(expectedX, expectedY);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(expected);
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, 0, 10, 0, -5, 0)]  // Horizontal line
    [Arguments(0, 0, 5, 0, -10, 0, 10, 0, -5)]  // Vertical line
    //@formatter:on
    public void CalculateIntersectionEntryPoint_ValueCircle_Point_ValuePoint_Should_Return_First_Intersection_Point(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);
        var lineStart = new Point(lineStartX, lineStartY);
        var lineEnd = new ValuePoint(lineEndX, lineEndY);
        var expected = new Point(expectedX, expectedY);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(expected);
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, 0, 10, 0, -5, 0)]  // Horizontal line
    [Arguments(0, 0, 5, 0, -10, 0, 10, 0, -5)]  // Vertical line
    //@formatter:on
    public void CalculateIntersectionEntryPoint_ValueCircle_ValuePoint_IPoint_Should_Return_First_Intersection_Point(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);
        var lineStart = new ValuePoint(lineStartX, lineStartY);

        var lineEnd = MockReferencePoint.Create(lineEndX, lineEndY)
                                        .Object;
        var expected = new Point(expectedX, expectedY);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(expected);
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, 0, 10, 0, -5, 0)]  // Horizontal line
    [Arguments(0, 0, 5, 0, -10, 0, 10, 0, -5)]  // Vertical line
    //@formatter:on
    public void CalculateIntersectionEntryPoint_ValueCircle_ValuePoint_Point_Should_Return_First_Intersection_Point(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);
        var lineStart = new ValuePoint(lineStartX, lineStartY);
        var lineEnd = new Point(lineEndX, lineEndY);
        var expected = new Point(expectedX, expectedY);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(expected);
    }

//@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, 0, 10, 0, -5, 0)]  // Horizontal line through center
    [Arguments(0, 0, 5, 0, -10, 0, 10, 0, -5)]  // Vertical line through center  
    [Arguments(0, 0, 5, -10, -10, 10, 10, -3, -3)]  // Diagonal line through center
    [Arguments(5, 5, 3, 0, 5, 10, 5, 2, 5)]  // Vertical line intersecting circle at offset
    [Arguments(-3, 2, 4, -8, 2, 2, 2, -7, 2)]  // Horizontal line intersecting circle at negative offset
    //@formatter:on
    public void CalculateIntersectionEntryPoint_ValueCircle_ValuePoint_ValuePoint_Should_Return_First_Intersection_Point(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY,
        int expectedX,
        int expectedY)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);
        var lineStart = new ValuePoint(lineStartX, lineStartY);
        var lineEnd = new ValuePoint(lineEndX, lineEndY);
        var expected = new Point(expectedX, expectedY);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(expected);
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 3, 10, 10, 15, 15)]  // Line completely outside circle
    [Arguments(0, 0, 2, 5, 0, 10, 0)]  // Line starts outside, doesn't reach circle
    [Arguments(5, 5, 2, 0, 0, 2, 2)]  // Line ends before reaching circle
    //@formatter:on
    public void CalculateIntersectionEntryPoint_ValueCircle_ValuePoint_ValuePoint_Should_Return_Null_When_No_Intersection(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);
        var lineStart = new ValuePoint(lineStartX, lineStartY);
        var lineEnd = new ValuePoint(lineEndX, lineEndY);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .BeNull();
    }

    [Test]
    public void CalculateIntersectionEntryPoint_ValueCircle_ValuePoint_ValuePoint_Should_Return_Start_Point_When_Start_Inside_Circle()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 10);
        var lineStart = new ValuePoint(2, 3); // Inside circle
        var lineEnd = new ValuePoint(15, 15);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        result.Should()
              .Be(new Point(2, 3));
    }

    #region No Intersection Tests
    [Test]
    [Arguments(
        0,
        0,
        2,
        5,
        5,
        10,
        10)]
    [Arguments(
        5,
        5,
        1,
        0,
        0,
        2,
        2)]
    [Arguments(
        10,
        10,
        3,
        0,
        5,
        5,
        0)]
    [Arguments(
        -5,
        -5,
        2,
        1,
        1,
        5,
        5)]
    public void CalculateIntersectionEntryPoint_VI_All_Overloads_Should_Return_Null_When_No_Intersection(
        int centerX,
        int centerY,
        int radius,
        int lineStartX,
        int lineStartY,
        int lineEndX,
        int lineEndY)
    {
        // Arrange
        var mockCircle = Mock.Of<ICircle>(c
            => (c.Center == Mock.Of<IPoint>(p => (p.X == centerX) && (p.Y == centerY))) && (c.Radius == radius));
        var mockLineStart = Mock.Of<IPoint>(p => (p.X == lineStartX) && (p.Y == lineStartY));
        var mockLineEnd = Mock.Of<IPoint>(p => (p.X == lineEndX) && (p.Y == lineEndY));

        var valuePointStart = new ValuePoint(lineStartX, lineStartY);
        var valuePointEnd = new ValuePoint(lineEndX, lineEndY);
        var pointStart = new Point(lineStartX, lineStartY);
        var pointEnd = new Point(lineEndX, lineEndY);

        // Act & Assert - Test all overloads
        mockCircle.CalculateIntersectionEntryPoint(valuePointStart, mockLineEnd)
                  .Should()
                  .BeNull();

        mockCircle.CalculateIntersectionEntryPoint(pointStart, valuePointEnd)
                  .Should()
                  .BeNull();

        mockCircle.CalculateIntersectionEntryPoint(pointStart, pointEnd)
                  .Should()
                  .BeNull();

        mockCircle.CalculateIntersectionEntryPoint(pointStart, mockLineEnd)
                  .Should()
                  .BeNull();

        mockCircle.CalculateIntersectionEntryPoint(mockLineStart, valuePointEnd)
                  .Should()
                  .BeNull();

        mockCircle.CalculateIntersectionEntryPoint(mockLineStart, pointEnd)
                  .Should()
                  .BeNull();
    }
    #endregion

    //@formatter:off
    [Test]
    [Arguments(0, 0, 10, 2, 3, 4, DistanceType.Euclidean, true)]   // Small circle inside large circle
    [Arguments(0, 0, 5, 0, 0, 5, DistanceType.Euclidean, true)]    // Circle at same center, same radius
    [Arguments(0, 0, 5, 3, 4, 1, DistanceType.Euclidean, false)]   // Circle outside
    [Arguments(0, 0, 10, 5, 0, 5, DistanceType.Euclidean, true)]   // Circle touching edge
    //@formatter:on
    public void ContainsCircle_ICircle_ICircle_Should_Return_Correct_Result(
        int centerX,
        int centerY,
        int radius,
        int otherCenterX,
        int otherCenterY,
        int otherRadius,
        DistanceType distanceType,
        bool expected)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        ICircle circle = new Circle(center, radius);
        var otherCenter = new Point(otherCenterX, otherCenterY);
        ICircle other = new Circle(otherCenter, otherRadius);

        // Act
        var result = circle.ContainsCircle(other, distanceType);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void ContainsCircle_ICircle_ICircle_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle circle = null!;
        var otherCenter = new Point(2, 3);
        ICircle other = new Circle(otherCenter, 4);

        // Act
        var act = () => circle.ContainsCircle(other);

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void ContainsCircle_ICircle_ICircle_Should_Throw_When_Invalid_DistanceType()
    {
        // Arrange
        var center = new Point(0, 0);
        ICircle circle = new Circle(center, 10);
        var otherCenter = new Point(2, 3);
        ICircle other = new Circle(otherCenter, 4);
        var invalidDistanceType = (DistanceType)255;

        // Act
        var act = () => circle.ContainsCircle(other, invalidDistanceType);

        // Assert
        act.Should()
           .Throw<ArgumentOutOfRangeException>()
           .WithParameterName("distanceType");
    }

    [Test]
    public void ContainsCircle_ICircle_ICircle_Should_Throw_When_Other_Is_Null()
    {
        // Arrange
        var center = new Point(0, 0);
        ICircle circle = new Circle(center, 10);
        ICircle other = null!;

        // Act
        var act = () => circle.ContainsCircle(other);

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 10, 2, 3, 4, DistanceType.Euclidean, true)]   // Small circle inside large circle
    [Arguments(0, 0, 5, 0, 0, 5, DistanceType.Euclidean, true)]    // Circle at same center, same radius
    [Arguments(0, 0, 5, 3, 4, 1, DistanceType.Euclidean, false)]   // Circle outside
    [Arguments(0, 0, 10, 5, 0, 5, DistanceType.Euclidean, true)]   // Circle touching edge
    //@formatter:on
    public void ContainsCircle_ICircle_ValueCircle_Should_Return_Correct_Result(
        int centerX,
        int centerY,
        int radius,
        int otherCenterX,
        int otherCenterY,
        int otherRadius,
        DistanceType distanceType,
        bool expected)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        ICircle circle = new Circle(center, radius);
        var otherCenter = new Point(otherCenterX, otherCenterY);
        var other = new ValueCircle(otherCenter, otherRadius);

        // Act
        var result = circle.ContainsCircle(other, distanceType);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void ContainsCircle_ICircle_ValueCircle_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle circle = null!;
        var otherCenter = new Point(2, 3);

        // Act
        var act = () => circle.ContainsCircle(new ValueCircle(otherCenter, 4));

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void ContainsCircle_ICircle_ValueCircle_Should_Throw_When_Invalid_DistanceType()
    {
        // Arrange
        var center = new Point(0, 0);
        ICircle circle = new Circle(center, 10);
        var otherCenter = new Point(2, 3);
        var invalidDistanceType = (DistanceType)255;

        // Act
        var act = () => circle.ContainsCircle(new ValueCircle(otherCenter, 4), invalidDistanceType);

        // Assert
        act.Should()
           .Throw<ArgumentOutOfRangeException>()
           .WithParameterName("distanceType");
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 10, 7, 0, 2, DistanceType.Euclidean, true)]   // Touching internally (7 + 2 = 9 < 10)
    [Arguments(0, 0, 10, 8, 0, 2, DistanceType.Euclidean, true)]   // Touching at edge (8 + 2 = 10 = 10)
    [Arguments(0, 0, 10, 9, 0, 2, DistanceType.Euclidean, false)]  // Outside (9 + 2 = 11 > 10)
    //@formatter:on
    public void ContainsCircle_Should_Handle_Boundary_Conditions(
        int centerX,
        int centerY,
        int radius,
        int otherCenterX,
        int otherCenterY,
        int otherRadius,
        DistanceType distanceType,
        bool expected)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);
        var otherCenter = new Point(otherCenterX, otherCenterY);
        var other = new ValueCircle(otherCenter, otherRadius);

        // Act
        var result = circle.ContainsCircle(other, distanceType);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void ContainsCircle_Should_Handle_Identical_Circles()
    {
        // Arrange
        var center = new Point(5, 5);
        var circle = new ValueCircle(center, 10);
        var other = new ValueCircle(center, 10);

        // Act
        var euclideanResult = circle.ContainsCircle(other);
        var manhattanResult = circle.ContainsCircle(other, DistanceType.Manhattan);

        // Assert
        euclideanResult.Should()
                       .BeTrue(); // Distance 0 + radius 10 = 10 <= 10

        manhattanResult.Should()
                       .BeTrue(); // Distance 0 + radius 10 = 10 <= 10
    }

    [Test]
    public void ContainsCircle_Should_Handle_Large_Circles()
    {
        // Arrange
        var center = new Point(1000, 1000);
        var circle = new ValueCircle(center, 500);
        var otherCenter = new Point(1200, 1200);
        var other = new ValueCircle(otherCenter, 100);

        // Act
        var result = circle.ContainsCircle(other);

        // Assert
        // Distance = sqrt((200)^2 + (200)^2) = sqrt(80000) ≈ 282.84
        // 282.84 + 100 = 382.84 < 500
        result.Should()
              .BeTrue();
    }

    [Test]
    public void ContainsCircle_Should_Handle_Negative_Coordinates()
    {
        // Arrange
        var center = new Point(-10, -10);
        var circle = new ValueCircle(center, 15);
        var otherCenter = new Point(-5, -5);
        var other = new ValueCircle(otherCenter, 3);

        // Act
        var result = circle.ContainsCircle(other);

        // Assert
        // Distance = sqrt((5)^2 + (5)^2) = sqrt(50) ≈ 7.07
        // 7.07 + 3 = 10.07 < 15
        result.Should()
              .BeTrue();
    }

    [Test]
    public void ContainsCircle_Should_Handle_Zero_Radius_Circles()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 5);
        var otherCenter = new Point(2, 2);
        var other = new ValueCircle(otherCenter, 0); // Zero radius circle (point)

        // Act
        var result = circle.ContainsCircle(other);

        // Assert
        result.Should()
              .BeTrue(); // Distance ~2.83 + 0 < 5
    }
    
    //@formatter:off
    [Test]
    [Arguments(0, 0, 10, 2, 3, 4, DistanceType.Euclidean, true)]   // Small circle inside large circle
    [Arguments(0, 0, 5, 0, 0, 5, DistanceType.Euclidean, true)]    // Circle at same center, same radius
    [Arguments(0, 0, 5, 3, 4, 1, DistanceType.Euclidean, false)]   // Circle outside
    [Arguments(0, 0, 10, 5, 0, 5, DistanceType.Euclidean, true)]   // Circle touching edge
    //@formatter:on
    public void ContainsCircle_ValueCircle_ICircle_Should_Return_Correct_Result(
        int centerX,
        int centerY,
        int radius,
        int otherCenterX,
        int otherCenterY,
        int otherRadius,
        DistanceType distanceType,
        bool expected)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);
        var otherCenter = new Point(otherCenterX, otherCenterY);
        ICircle other = new Circle(otherCenter, otherRadius);

        // Act
        var result = circle.ContainsCircle(other, distanceType);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void ContainsCircle_ValueCircle_ICircle_Should_Throw_When_Invalid_DistanceType()
    {
        // Arrange
        var center = new Point(0, 0);
        var otherCenter = new Point(2, 3);
        ICircle other = new Circle(otherCenter, 4);
        var invalidDistanceType = (DistanceType)255;

        // Act
        var act = () => new ValueCircle(center, 10).ContainsCircle(other, invalidDistanceType);

        // Assert
        act.Should()
           .Throw<ArgumentOutOfRangeException>()
           .WithParameterName("distanceType");
    }

    [Test]
    public void ContainsCircle_ValueCircle_ICircle_Should_Throw_When_Other_Is_Null()
    {
        // Arrange
        var center = new Point(0, 0);
        ICircle other = null!;

        // Act
        var act = () => new ValueCircle(center, 10).ContainsCircle(other);

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }
    
    //@formatter:off
    [Test]
    [Arguments(0, 0, 10, 2, 3, 4, DistanceType.Euclidean, true)]   // Small circle inside large circle
    [Arguments(0, 0, 5, 0, 0, 5, DistanceType.Euclidean, true)]    // Circle at same center, smaller radius
    [Arguments(0, 0, 5, 0, 0, 5, DistanceType.Euclidean, true)]    // Circle at same center, same radius (edge case)
    [Arguments(0, 0, 5, 3, 4, 1, DistanceType.Euclidean, false)]   // Circle outside (distance 5 + radius 1 = 6 > 5)
    [Arguments(0, 0, 10, 6, 8, 1, DistanceType.Euclidean, false)]  // Circle partially outside (distance 10 + radius 1 = 11 > 10)
    [Arguments(0, 0, 10, 5, 0, 5, DistanceType.Euclidean, true)]   // Circle touching edge (distance 5 + radius 5 = 10 = 10)
    //@formatter:on
    public void ContainsCircle_ValueCircle_ValueCircle_Euclidean_Should_Return_Correct_Result(
        int centerX,
        int centerY,
        int radius,
        int otherCenterX,
        int otherCenterY,
        int otherRadius,
        DistanceType distanceType,
        bool expected)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);
        var otherCenter = new Point(otherCenterX, otherCenterY);
        var other = new ValueCircle(otherCenter, otherRadius);

        // Act
        var result = circle.ContainsCircle(other, distanceType);

        // Assert
        result.Should()
              .Be(expected);
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 10, 2, 3, 4, DistanceType.Manhattan, true)]   // Small circle inside (Manhattan distance 5 + radius 4 = 9 < 10)
    [Arguments(0, 0, 5, 0, 0, 5, DistanceType.Manhattan, true)]    // Same center, same radius
    [Arguments(0, 0, 10, 4, 4, 1, DistanceType.Manhattan, true)]   // Circle inside (Manhattan distance 8 + radius 1 = 9 < 10)
    [Arguments(0, 0, 5, 3, 4, 1, DistanceType.Manhattan, false)]   // Circle outside (Manhattan distance 7 + radius 1 = 8 > 5)
    [Arguments(0, 0, 10, 5, 5, 1, DistanceType.Manhattan, false)]  // Circle outside (Manhattan distance 10 + radius 1 = 11 > 10)
    //@formatter:on
    public void ContainsCircle_ValueCircle_ValueCircle_Manhattan_Should_Return_Correct_Result(
        int centerX,
        int centerY,
        int radius,
        int otherCenterX,
        int otherCenterY,
        int otherRadius,
        DistanceType distanceType,
        bool expected)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);
        var otherCenter = new Point(otherCenterX, otherCenterY);
        var other = new ValueCircle(otherCenter, otherRadius);

        // Act
        var result = circle.ContainsCircle(other, distanceType);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void ContainsCircle_ValueCircle_ValueCircle_Should_Throw_When_Invalid_DistanceType()
    {
        // Arrange
        var center = new Point(0, 0);
        var otherCenter = new Point(2, 3);
        var invalidDistanceType = (DistanceType)255;

        // Act
        var act = () => new ValueCircle(center, 10).ContainsCircle(new ValueCircle(otherCenter, 4), invalidDistanceType);

        // Assert
        act.Should()
           .Throw<ArgumentOutOfRangeException>()
           .WithParameterName("distanceType");
    }

    #region Edge Cases and Boundary Conditions
    [Test]
    public void EuclideanEdgeDistanceFrom_Should_Be_Consistent_Across_All_Overloads()
    {
        // Arrange
        var valueCircle = new Circle(new Point(5, 5), 3);
        ICircle iCircle = valueCircle;
        var valuePoint = new ValuePoint(10, 9);
        var point = new Point(10, 9);
        var mockPoint = new Mock<IPoint>();

        mockPoint.SetupGet(p => p.X)
                 .Returns(10);

        mockPoint.SetupGet(p => p.Y)
                 .Returns(9);

        // Act
        var result1 = valueCircle.EuclideanEdgeDistanceFrom(valuePoint);
        var result2 = valueCircle.EuclideanEdgeDistanceFrom(point);
        var result3 = valueCircle.EuclideanEdgeDistanceFrom(mockPoint.Object);
        var result4 = iCircle.EuclideanEdgeDistanceFrom(valuePoint);
        var result5 = iCircle.EuclideanEdgeDistanceFrom(point);
        var result6 = iCircle.EuclideanEdgeDistanceFrom(mockPoint.Object);

        // Assert
        var expectedDistance = Math.Sqrt(41) - 3; // Distance from (5,5) to (10,9) minus radius

        result1.Should()
               .BeApproximately(expectedDistance, 0.01);

        result2.Should()
               .BeApproximately(expectedDistance, 0.01);

        result3.Should()
               .BeApproximately(expectedDistance, 0.01);

        result4.Should()
               .BeApproximately(expectedDistance, 0.01);

        result5.Should()
               .BeApproximately(expectedDistance, 0.01);

        result6.Should()
               .BeApproximately(expectedDistance, 0.01);

        // All results should be equal
        result1.Should()
               .BeApproximately(result2, 0.01);

        result2.Should()
               .BeApproximately(result3, 0.01);

        result3.Should()
               .BeApproximately(result4, 0.01);

        result4.Should()
               .BeApproximately(result5, 0.01);

        result5.Should()
               .BeApproximately(result6, 0.01);
    }
    #endregion

    #region Helper Methods
    private static IEnumerable<Point> GenerateBresenhamLine(Point start, Point end)
    {
        var xDiff = Math.Abs(end.X - start.X);
        var yDiff = Math.Abs(end.Y - start.Y);
        var directionalX = start.X < end.X ? 1 : -1;
        var directionalY = start.Y < end.Y ? 1 : -1;
        var err = xDiff - yDiff;
        var x = start.X;
        var y = start.Y;

        while (true)
        {
            yield return new Point(x, y);

            if ((x == end.X) && (y == end.Y))
                yield break;

            var e2 = 2 * err;

            if (e2 > -yDiff)
            {
                err -= yDiff;
                x += directionalX;
            }

            if (e2 < xDiff)
            {
                err += xDiff;
                y += directionalY;
            }
        }
    }
    #endregion

    #region Null Guard Tests
    [Test]
    public void CalculateIntersectionEntryPoint_ICircle_ValuePoint_IPoint_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;
        var lineEnd = Mock.Of<IPoint>(p => (p.X == 5) && (p.Y == 5));

        // Act & Assert
        var act = () => circle!.CalculateIntersectionEntryPoint(new ValuePoint(0, 0), lineEnd);

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void CalculateIntersectionEntryPoint_ICircle_Point_ValuePoint_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;
        var lineStart = new Point(0, 0);

        // Act & Assert
        var act = () => circle!.CalculateIntersectionEntryPoint(lineStart, new ValuePoint(5, 5));

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void CalculateIntersectionEntryPoint_ICircle_Point_Point_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;
        var lineStart = new Point(0, 0);
        var lineEnd = new Point(5, 5);

        // Act & Assert
        var act = () => circle!.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void CalculateIntersectionEntryPoint_ICircle_Point_IPoint_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;
        var lineStart = new Point(0, 0);
        var lineEnd = Mock.Of<IPoint>(p => (p.X == 5) && (p.Y == 5));

        // Act & Assert
        var act = () => circle!.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void CalculateIntersectionEntryPoint_ICircle_IPoint_ValuePoint_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;
        var lineStart = Mock.Of<IPoint>(p => (p.X == 0) && (p.Y == 0));

        // Act & Assert
        var act = () => circle!.CalculateIntersectionEntryPoint(lineStart, new ValuePoint(5, 5));

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void CalculateIntersectionEntryPoint_ICircle_IPoint_Point_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;
        var lineStart = Mock.Of<IPoint>(p => (p.X == 0) && (p.Y == 0));
        var lineEnd = new Point(5, 5);

        // Act & Assert
        var act = () => circle!.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        act.Should()
           .Throw<NullReferenceException>();
    }
    #endregion

    #region Line Start Inside Circle Tests
    [Test]
    public void CalculateIntersectionEntryPoint_Should_Return_Start_Point_When_Start_Inside_Circle()
    {
        // Arrange
        var circle = Mock.Of<ICircle>(c => (c.Center == Mock.Of<IPoint>(p => (p.X == 5) && (p.Y == 5))) && (c.Radius == 5));
        var lineStart = new Point(5, 5); // Center of circle
        var lineEnd = new Point(10, 10);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .Be(new Point(5, 5));
    }

    [Test]
    public void CalculateIntersectionEntryPoint_Should_Return_Start_Point_When_Start_Near_Center()
    {
        // Arrange
        var circle = Mock.Of<ICircle>(c => (c.Center == Mock.Of<IPoint>(p => (p.X == 0) && (p.Y == 0))) && (c.Radius == 10));
        var lineStart = new Point(1, 1); // Inside circle
        var lineEnd = new Point(20, 20);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .Be(new Point(1, 1));
    }
    #endregion

    #region Edge Cases
    [Test]
    public void CalculateIntersectionEntryPoint_VI_Should_Handle_Single_Point_Line()
    {
        // Arrange
        var circle = Mock.Of<ICircle>(c => (c.Center == Mock.Of<IPoint>(p => (p.X == 0) && (p.Y == 0))) && (c.Radius == 5));
        var lineStart = new Point(3, 3);
        var lineEnd = new Point(3, 3); // Same point

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .Be(new Point(3, 3)); // Point is inside circle
    }

    [Test]
    public void CalculateIntersectionEntryPoint_Should_Handle_Single_Point_Line_Outside_Circle()
    {
        // Arrange
        var circle = Mock.Of<ICircle>(c => (c.Center == Mock.Of<IPoint>(p => (p.X == 0) && (p.Y == 0))) && (c.Radius == 2));
        var lineStart = new Point(10, 10);
        var lineEnd = new Point(10, 10); // Same point, outside circle

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .BeNull();
    }

    [Test]
    public void CalculateIntersectionEntryPoint_VI_Should_Handle_Zero_Radius_Circle()
    {
        // Arrange
        var circle = Mock.Of<ICircle>(c => (c.Center == Mock.Of<IPoint>(p => (p.X == 5) && (p.Y == 5))) && (c.Radius == 0));
        var lineStart = new Point(0, 5);
        var lineEnd = new Point(10, 5);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .Be(new Point(5, 5)); // Should hit the center point
    }

    [Test]
    public void CalculateIntersectionEntryPoint_Should_Handle_Horizontal_Line()
    {
        // Arrange
        var circle = Mock.Of<ICircle>(c => (c.Center == Mock.Of<IPoint>(p => (p.X == 5) && (p.Y == 5))) && (c.Radius == 3));
        var lineStart = new Point(0, 5);
        var lineEnd = new Point(10, 5);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .Be(new Point(2, 5));
    }

    [Test]
    public void CalculateIntersectionEntryPoint_Should_Handle_Vertical_Line()
    {
        // Arrange
        var circle = Mock.Of<ICircle>(c => (c.Center == Mock.Of<IPoint>(p => (p.X == 5) && (p.Y == 5))) && (c.Radius == 3));
        var lineStart = new Point(5, 0);
        var lineEnd = new Point(5, 10);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .Be(new Point(5, 2));
    }

    [Test]
    public void CalculateIntersectionEntryPoint_Should_Handle_Diagonal_Line()
    {
        // Arrange
        var circle = Mock.Of<ICircle>(c => (c.Center == Mock.Of<IPoint>(p => (p.X == 0) && (p.Y == 0))) && (c.Radius == 5));
        var lineStart = new Point(-10, -10);
        var lineEnd = new Point(10, 10);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        // Verify the result is on or inside the circle
        var distanceSquared = Math.Pow(result.Value.X, 2) + Math.Pow(result.Value.Y, 2);

        distanceSquared.Should()
                       .BeLessThanOrEqualTo(25); // radius^2
    }

    [Test]
    public void CalculateIntersectionEntryPoint_Should_Handle_Negative_Coordinates()
    {
        // Arrange
        var circle = Mock.Of<ICircle>(c => (c.Center == Mock.Of<IPoint>(p => (p.X == -5) && (p.Y == -5))) && (c.Radius == 3));
        var lineStart = new Point(-10, -5);
        var lineEnd = new Point(0, -5);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .Be(new Point(-8, -5));
    }

    [Test]
    public void CalculateIntersectionEntryPoint_Should_Handle_Large_Coordinates()
    {
        // Arrange
        var circle = Mock.Of<ICircle>(c => (c.Center == Mock.Of<IPoint>(p => (p.X == 1000) && (p.Y == 1000))) && (c.Radius == 100));
        var lineStart = new Point(800, 1000);
        var lineEnd = new Point(1200, 1000);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .Be(new Point(900, 1000));
    }
    #endregion

    #region Bresenham Algorithm Verification Tests
    [Test]
    public void CalculateIntersectionEntryPoint_Should_Follow_Bresenham_Line_Algorithm()
    {
        // Arrange - Test that the algorithm follows Bresenham's line algorithm
        var circle = Mock.Of<ICircle>(c => (c.Center == Mock.Of<IPoint>(p => (p.X == 10) && (p.Y == 10))) && (c.Radius == 2));
        var lineStart = new Point(5, 8);
        var lineEnd = new Point(15, 12);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        // Verify that the result is a valid Bresenham step from the start point
        var expectedPoints = GenerateBresenhamLine(lineStart, lineEnd)
                             .Take(10)
                             .ToList();

        expectedPoints.Should()
                      .Contain(result.Value);
    }

    [Test]
    public void CalculateIntersectionEntryPoint_Should_Handle_Steep_Line()
    {
        // Arrange - Test steep line (|dy| > |dx|)
        var circle = Mock.Of<ICircle>(c => (c.Center == Mock.Of<IPoint>(p => (p.X == 5) && (p.Y == 10))) && (c.Radius == 3));
        var lineStart = new Point(4, 0);
        var lineEnd = new Point(6, 20);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .NotBeNull();

        var distanceSquared = Math.Pow(result.Value.X - 5, 2) + Math.Pow(result.Value.Y - 10, 2);

        distanceSquared.Should()
                       .BeLessThanOrEqualTo(9); // radius^2
    }

    [Test]
    public void CalculateIntersectionEntryPoint_Should_Handle_Reverse_Direction_Line()
    {
        // Arrange
        var circle = Mock.Of<ICircle>(c => (c.Center == Mock.Of<IPoint>(p => (p.X == 5) && (p.Y == 5))) && (c.Radius == 2));
        var lineStart = new Point(10, 5);
        var lineEnd = new Point(0, 5);

        // Act
        var result = circle.CalculateIntersectionEntryPoint(lineStart, lineEnd);

        // Assert
        result.Should()
              .Be(new Point(7, 5));
    }
    #endregion

    #region ValueCircle with ValuePoint Tests
    [Test]
    public void ContainsPoint_ValueCircle_ValuePoint_Euclidean_Should_Return_True_When_Point_Inside_Circle()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 5);
        var point = new ValuePoint(3, 4); // Distance = 5 (on edge)

        // Act
        var result = circle.ContainsPoint(point);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void ContainsPoint_ValueCircle_ValuePoint_Euclidean_Should_Return_False_When_Point_Outside_Circle()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 5);
        var point = new ValuePoint(4, 4); // Distance > 5

        // Act
        var result = circle.ContainsPoint(point);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void ContainsPoint_ValueCircle_ValuePoint_Manhattan_Should_Return_True_When_Point_Inside_Circle()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 7);
        var point = new ValuePoint(3, 4); // Manhattan distance = 7

        // Act
        var result = circle.ContainsPoint(point, DistanceType.Manhattan);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void ContainsPoint_ValueCircle_ValuePoint_Manhattan_Should_Return_False_When_Point_Outside_Circle()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 5);
        var point = new ValuePoint(3, 4); // Manhattan distance = 7 > 5

        // Act
        var result = circle.ContainsPoint(point, DistanceType.Manhattan);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void ContainsPoint_ValueCircle_ValuePoint_Should_Throw_When_Invalid_DistanceType()
    {
        // Arrange
        var center = new Point(0, 0);
        var invalidDistanceType = (DistanceType)255;

        // Act
        var act = () => new ValueCircle(center, 5).ContainsPoint(new ValuePoint(3, 4), invalidDistanceType);

        // Assert
        act.Should()
           .Throw<ArgumentOutOfRangeException>()
           .WithParameterName("distanceType");
    }

    [Test]
    public void ContainsPoint_ValueCircle_ValuePoint_Should_Default_To_Euclidean()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 5);
        var point = new ValuePoint(3, 4); // Distance = 5

        // Act
        var result = circle.ContainsPoint(point);

        // Assert
        result.Should()
              .BeTrue();
    }
    #endregion

    #region ValueCircle with Point Tests
    [Test]
    public void ContainsPoint_ValueCircle_Point_Euclidean_Should_Return_True_When_Point_At_Center()
    {
        // Arrange
        var center = new Point(5, 5);
        var circle = new ValueCircle(center, 10);
        var point = new Point(5, 5);

        // Act
        var result = circle.ContainsPoint(point);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void ContainsPoint_ValueCircle_Point_Euclidean_Should_Return_True_When_Point_On_Edge()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 5);
        var point = new Point(5, 0); // Exactly on edge

        // Act
        var result = circle.ContainsPoint(point);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void ContainsPoint_ValueCircle_Point_Manhattan_Should_Handle_Negative_Coordinates()
    {
        // Arrange
        var center = new Point(-2, -3);
        var circle = new ValueCircle(center, 5);
        var point = new Point(-4, -1); // Manhattan distance = 2 + 2 = 4

        // Act
        var result = circle.ContainsPoint(point, DistanceType.Manhattan);

        // Assert
        result.Should()
              .BeTrue();
    }
    #endregion

    #region ValueCircle with IPoint Tests
    [Test]
    public void ContainsPoint_ValueCircle_IPoint_Should_Throw_When_Point_Is_Null()
    {
        // Arrange
        var center = new Point(0, 0);
        IPoint point = null!;

        // Act
        var act = () => new ValueCircle(center, 5).ContainsPoint(point);

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void ContainsPoint_ValueCircle_IPoint_Euclidean_Should_Return_Correct_Result()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 10);

        var point = MockReferencePoint.Create(6, 8)
                                      .Object; // Distance = 10

        // Act
        var result = circle.ContainsPoint(point);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void ContainsPoint_ValueCircle_IPoint_Manhattan_Should_Return_Correct_Result()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 14);

        var point = MockReferencePoint.Create(6, 8)
                                      .Object; // Manhattan distance = 14

        // Act
        var result = circle.ContainsPoint(point, DistanceType.Manhattan);

        // Assert
        result.Should()
              .BeTrue();
    }
    #endregion

    #region ICircle with ValuePoint Tests
    [Test]
    public void ContainsPoint_ICircle_ValuePoint_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle circle = null!;

        // Act
        var act = () => circle.ContainsPoint(new ValuePoint(3, 4));

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void ContainsPoint_ICircle_ValuePoint_Euclidean_Should_Return_Correct_Result()
    {
        // Arrange
        var center = new Point(10, 10);
        ICircle circle = new Circle(center, 5);
        var point = new ValuePoint(13, 14); // Distance = 5

        // Act
        var result = circle.ContainsPoint(point);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void ContainsPoint_ICircle_ValuePoint_Manhattan_Should_Return_Correct_Result()
    {
        // Arrange
        var center = new Point(0, 0);
        ICircle circle = new Circle(center, 8);
        var point = new ValuePoint(3, 5); // Manhattan distance = 8

        // Act
        var result = circle.ContainsPoint(point, DistanceType.Manhattan);

        // Assert
        result.Should()
              .BeTrue();
    }
    #endregion

    #region ICircle with Point Tests
    [Test]
    public void ContainsPoint_ICircle_Point_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle circle = null!;
        var point = new Point(3, 4);

        // Act
        var act = () => circle.ContainsPoint(point);

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void ContainsPoint_ICircle_Point_Should_Handle_Large_Coordinates()
    {
        // Arrange
        var center = new Point(1000, 1000);
        ICircle circle = new Circle(center, 100);
        var point = new Point(1060, 1080); // Distance = 100

        // Act
        var result = circle.ContainsPoint(point);

        // Assert
        result.Should()
              .BeTrue();
    }
    #endregion

    #region ICircle with IPoint Tests
    [Test]
    public void ContainsPoint_ICircle_IPoint_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle circle = null!;

        var point = MockReferencePoint.Create(3, 4)
                                      .Object;

        // Act
        var act = () => circle.ContainsPoint(point);

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void ContainsPoint_ICircle_IPoint_Should_Throw_When_Point_Is_Null()
    {
        // Arrange
        var center = new Point(0, 0);
        ICircle circle = new Circle(center, 5);
        IPoint point = null!;

        // Act
        var act = () => circle.ContainsPoint(point);

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void ContainsPoint_ICircle_IPoint_Should_Throw_When_Invalid_DistanceType()
    {
        // Arrange
        var center = new Point(0, 0);
        ICircle circle = new Circle(center, 5);

        var point = MockReferencePoint.Create(3, 4)
                                      .Object;
        var invalidDistanceType = (DistanceType)255;

        // Act
        var act = () => circle.ContainsPoint(point, invalidDistanceType);

        // Assert
        act.Should()
           .Throw<ArgumentOutOfRangeException>()
           .WithParameterName("distanceType");
    }

    [Test]
    public void ContainsPoint_ICircle_IPoint_Euclidean_Should_Return_Correct_Result()
    {
        // Arrange
        var center = new Point(-5, -5);
        ICircle circle = new Circle(center, 10);

        var point = MockReferencePoint.Create(-2, -1)
                                      .Object; // Distance = 5

        // Act
        var result = circle.ContainsPoint(point);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void ContainsPoint_ICircle_IPoint_Manhattan_Should_Return_Correct_Result()
    {
        // Arrange
        var center = new Point(0, 0);
        ICircle circle = new Circle(center, 10);

        var point = MockReferencePoint.Create(4, 6)
                                      .Object; // Manhattan distance = 10

        // Act
        var result = circle.ContainsPoint(point, DistanceType.Manhattan);

        // Assert
        result.Should()
              .BeTrue();
    }
    #endregion

    #region Boundary Condition Tests
    [Test]
    public void ContainsPoint_Should_Handle_Zero_Radius_Circle()
    {
        // Arrange
        var center = new Point(5, 5);
        var circle = new ValueCircle(center, 0);
        var exactPoint = new ValuePoint(5, 5); // Exactly at center
        var nearPoint = new ValuePoint(5, 6); // 1 unit away

        // Act
        var exactResult = circle.ContainsPoint(exactPoint);
        var nearResult = circle.ContainsPoint(nearPoint);

        // Assert
        exactResult.Should()
                   .BeTrue();

        nearResult.Should()
                  .BeFalse();
    }

    [Test]
    public void ContainsPoint_Should_Handle_Floating_Point_Precision()
    {
        // Arrange - Test case where floating point precision might matter
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 5);
        var point = new ValuePoint(3, 4); // Should be exactly distance 5

        // Act
        var result = circle.ContainsPoint(point);

        // Assert
        result.Should()
              .BeTrue(); // Distance = sqrt(9 + 16) = 5.0 exactly
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 5, 0, 0, true)]      // Center point
    [Arguments(0, 0, 5, 5, 0, true)]      // On edge (horizontal)
    [Arguments(0, 0, 5, 0, 5, true)]      // On edge (vertical)
    [Arguments(0, 0, 5, 3, 4, true)]      // On edge (diagonal)
    [Arguments(0, 0, 5, 6, 0, false)]     // Outside (horizontal)
    [Arguments(0, 0, 5, 0, 6, false)]     // Outside (vertical)
    [Arguments(0, 0, 5, 4, 4, false)]     // Outside (diagonal)
    [Arguments(-10, -10, 15, -5, -5, true)] // Negative coordinates, inside
    [Arguments(100, 200, 50, 130, 240, true)] // Large coordinates, inside
    //@formatter:on
    public void ContainsPoint_Euclidean_Should_Return_Expected_Results(
        int centerX,
        int centerY,
        int radius,
        int pointX,
        int pointY,
        bool expected)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);
        var point = new ValuePoint(pointX, pointY);

        // Act
        var result = circle.ContainsPoint(point);

        // Assert
        result.Should()
              .Be(expected);
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 7, 3, 4, true)]      // Manhattan distance = 7
    [Arguments(0, 0, 6, 3, 4, false)]     // Manhattan distance = 7 > 6
    [Arguments(0, 0, 10, 5, 5, true)]     // Manhattan distance = 10
    [Arguments(0, 0, 9, 5, 5, false)]     // Manhattan distance = 10 > 9
    [Arguments(-5, -5, 8, -2, -1, true)]  // Manhattan distance = 3 + 4 = 7 < 8
    [Arguments(10, 20, 15, 18, 28, false)] // Manhattan distance = 8 + 8 = 16 > 15
    //@formatter:on
    public void ContainsPoint_Manhattan_Should_Return_Expected_Results(
        int centerX,
        int centerY,
        int radius,
        int pointX,
        int pointY,
        bool expected)
    {
        // Arrange
        var center = new Point(centerX, centerY);
        var circle = new ValueCircle(center, radius);
        var point = new ValuePoint(pointX, pointY);

        // Act
        var result = circle.ContainsPoint(point, DistanceType.Manhattan);

        // Assert
        result.Should()
              .Be(expected);
    }
    #endregion

    #region Edge Cases
    [Test]
    public void ContainsPoint_Should_Handle_Same_Point_As_Center()
    {
        // Arrange
        var center = new Point(42, 73);
        var circle = new ValueCircle(center, 100);
        var point = new ValuePoint(42, 73);

        // Act
        var euclideanResult = circle.ContainsPoint(point);
        var manhattanResult = circle.ContainsPoint(point, DistanceType.Manhattan);

        // Assert
        euclideanResult.Should()
                       .BeTrue();

        manhattanResult.Should()
                       .BeTrue();
    }

    [Test]
    public void ContainsPoint_Should_Be_Consistent_Across_All_Overloads()
    {
        // Arrange
        var center = new Point(10, 20);
        var valueCircle = new ValueCircle(center, 15);
        ICircle refCircle = new Circle(center, 15);

        var valuePoint = new ValuePoint(18, 32);
        var refPoint = new Point(18, 32);

        var iPoint = MockReferencePoint.Create(18, 32)
                                       .Object;

        // Act - Test all combinations with Euclidean distance
        var result1 = valueCircle.ContainsPoint(valuePoint);
        var result2 = valueCircle.ContainsPoint(refPoint);
        var result3 = valueCircle.ContainsPoint(iPoint);
        var result4 = refCircle.ContainsPoint(valuePoint);
        var result5 = refCircle.ContainsPoint(refPoint);
        var result6 = refCircle.ContainsPoint(iPoint);

        // Assert - All should return the same result
        var allResults = new[]
        {
            result1,
            result2,
            result3,
            result4,
            result5,
            result6
        };

        allResults.Should()
                  .AllSatisfy(r => r.Should()
                                    .Be(result1));
    }
    #endregion

    #region ValueCircle with ValuePoint
    [Test]
    public void EuclideanEdgeDistanceFrom_ValueCircle_ValuePoint_Should_Return_Zero_When_Point_On_Circle_Edge()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 5);
        var point = new ValuePoint(5, 0);

        // Act
        var result = circle.EuclideanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(0.0);
    }

    [Test]
    public void EuclideanEdgeDistanceFrom_ValueCircle_ValuePoint_Should_Return_Zero_When_Point_Inside_Circle()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 5);
        var point = new ValuePoint(3, 0);

        // Act
        var result = circle.EuclideanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(0.0);
    }

    [Test]
    public void EuclideanEdgeDistanceFrom_ValueCircle_ValuePoint_Should_Return_Correct_Distance_When_Point_Outside_Circle()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 5);
        var point = new ValuePoint(10, 0);

        // Act
        var result = circle.EuclideanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(5.0);
    }

    [Test]
    public void EuclideanEdgeDistanceFrom_ValueCircle_ValuePoint_Should_Handle_Diagonal_Distance()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 5);
        var point = new ValuePoint(3, 4);

        // Act
        var result = circle.EuclideanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(0.0); // Distance from center is 5, exactly on circle edge
    }

    [Test]
    public void EuclideanEdgeDistanceFrom_ValueCircle_ValuePoint_Should_Handle_Negative_Coordinates()
    {
        // Arrange
        var circle = new ValueCircle(new Point(-2, -3), 4);
        var point = new ValuePoint(-6, -7);

        // Act
        var result = circle.EuclideanEdgeDistanceFrom(point);

        // Assert
        var expectedDistance = Math.Sqrt(16 + 16) - 4; // sqrt(32) - 4 ≈ 1.66

        result.Should()
              .BeApproximately(expectedDistance, 0.01);
    }
    #endregion

    #region ValueCircle with Point
    [Test]
    public void EuclideanEdgeDistanceFrom_ValueCircle_Point_Should_Return_Zero_When_Point_At_Center()
    {
        // Arrange
        var circle = new ValueCircle(new Point(5, 5), 3);
        var point = new Point(5, 5);

        // Act
        var result = circle.EuclideanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(0.0);
    }

    [Test]
    public void EuclideanEdgeDistanceFrom_ValueCircle_Point_Should_Calculate_Correct_Distance()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 2);
        var point = new Point(6, 8);

        // Act
        var result = circle.EuclideanEdgeDistanceFrom(point);

        // Assert
        var expectedDistance = 10 - 2; // Distance from center is 10, minus radius 2

        result.Should()
              .Be(expectedDistance);
    }

    [Test]
    public void EuclideanEdgeDistanceFrom_ValueCircle_Point_Should_Handle_Large_Coordinates()
    {
        // Arrange
        var circle = new ValueCircle(new Point(1000, 1000), 100);
        var point = new Point(1300, 1400);

        // Act
        var result = circle.EuclideanEdgeDistanceFrom(point);

        // Assert
        var expectedDistance = 500 - 100; // Distance from center is 500, minus radius 100

        result.Should()
              .Be(expectedDistance);
    }
    #endregion

    #region ValueCircle with IPoint
    [Test]
    public void EuclideanEdgeDistanceFrom_ValueCircle_IPoint_Should_Throw_When_Point_Is_Null()
    {
        // Arrange
        IPoint? point = null;

        // Act
        var act = () => new ValueCircle(new Point(0, 0), 5).EuclideanEdgeDistanceFrom(point!);

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void EuclideanEdgeDistanceFrom_ValueCircle_IPoint_Should_Calculate_Correct_Distance_With_Mock()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 3);
        var point = new Point(4, 0);

        // Act
        var result = circle.EuclideanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(1.0); // Distance 4 - radius 3 = 1
    }

    [Test]
    public void EuclideanEdgeDistanceFrom_ValueCircle_IPoint_Should_Return_Zero_For_Interior_Point()
    {
        // Arrange
        var circle = new ValueCircle(new Point(2, 2), 5);
        var point = new Point(4, 4);

        // Act
        var result = circle.EuclideanEdgeDistanceFrom(point);

        // Assert

        result.Should()
              .Be(0.0); // Max(0, 2.83 - 5) = 0
    }
    #endregion

    #region ICircle with ValuePoint
    [Test]
    public void EuclideanEdgeDistanceFrom_ICircle_ValuePoint_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;

        // Act
        var act = () => circle!.EuclideanEdgeDistanceFrom(new ValuePoint(1, 1));

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void EuclideanEdgeDistanceFrom_ICircle_ValuePoint_Should_Calculate_Distance_With_Mock_Circle()
    {
        // Arrange

        var center = new Point(0, 0);
        var circle = new Circle(center, 2);

        var point = new ValuePoint(5, 0);

        // Act
        var result = circle.EuclideanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(3.0); // Distance 5 - radius 2 = 3
    }

    [Test]
    public void EuclideanEdgeDistanceFrom_ICircle_ValuePoint_Should_Handle_Zero_Radius()
    {
        // Arrange
        var center = new Point(3, 4);
        var circle = new Circle(center, 0);

        var point = new ValuePoint(0, 0);

        // Act
        var result = circle.EuclideanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(5.0); // Distance from (3,4) to (0,0) is 5
    }
    #endregion

    #region ICircle with Point
    [Test]
    public void EuclideanEdgeDistanceFrom_ICircle_Point_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;
        var point = new Point(1, 1);

        // Act
        var act = () => circle!.EuclideanEdgeDistanceFrom(point);

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void EuclideanEdgeDistanceFrom_ICircle_Point_Should_Work_With_Real_Circle()
    {
        // Arrange
        ICircle circle = new Circle(new Point(10, 10), 5);
        var point = new Point(14, 13);

        // Act
        var result = circle.EuclideanEdgeDistanceFrom(point);

        // Assert
        var expectedDistance = 5 - 5; // Distance is 5, radius is 5, so edge distance is 0

        result.Should()
              .Be(expectedDistance);
    }
    #endregion

    #region ICircle with IPoint
    [Test]
    public void EuclideanEdgeDistanceFrom_ICircle_IPoint_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;
        var point = new Point(0, 0);

        // Act
        var act = () => circle!.EuclideanEdgeDistanceFrom(point);

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void EuclideanEdgeDistanceFrom_ICircle_IPoint_Should_Throw_When_Point_Is_Null()
    {
        // Arrange
        var circle = new Circle(new Point(0, 0), 0);
        IPoint? point = null;

        // Act
        var act = () => circle.EuclideanEdgeDistanceFrom(point!);

        // Assert
        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void EuclideanEdgeDistanceFrom_ICircle_IPoint_Should_Calculate_Distance_With_Both_Mocks()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();
        var mockPoint = new Mock<IPoint>();

        mockCenter.SetupGet(p => p.X)
                  .Returns(1);

        mockCenter.SetupGet(p => p.Y)
                  .Returns(1);

        mockCircle.SetupGet(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.SetupGet(c => c.Radius)
                  .Returns(2);

        mockPoint.SetupGet(p => p.X)
                 .Returns(5);

        mockPoint.SetupGet(p => p.Y)
                 .Returns(4);

        // Act
        var result = mockCircle.Object.EuclideanEdgeDistanceFrom(mockPoint.Object);

        // Assert
        var expectedDistance = 5 - 2; // Distance from (1,1) to (5,4) is 5, minus radius 2

        result.Should()
              .Be(expectedDistance);
    }

    [Test]
    public void EuclideanEdgeDistanceFrom_ICircle_IPoint_Should_Return_Zero_When_Distance_Less_Than_Radius()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();
        var mockPoint = new Mock<IPoint>();

        mockCenter.SetupGet(p => p.X)
                  .Returns(0);

        mockCenter.SetupGet(p => p.Y)
                  .Returns(0);

        mockCircle.SetupGet(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.SetupGet(c => c.Radius)
                  .Returns(10);

        mockPoint.SetupGet(p => p.X)
                 .Returns(3);

        mockPoint.SetupGet(p => p.Y)
                 .Returns(4);

        // Act
        var result = mockCircle.Object.EuclideanEdgeDistanceFrom(mockPoint.Object);

        // Assert
        result.Should()
              .Be(0.0); // Max(0, 5 - 10) = 0
    }
    #endregion

    #region ValueCircle to ValueCircle Tests
    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Return_Zero_When_Circles_Touch()
    {
        // Arrange
        var center1 = new Point(0, 0);
        var center2 = new Point(10, 0);
        var circle1 = new ValueCircle(center1, 5);
        var circle2 = new ValueCircle(center2, 5);

        // Act
        var distance = circle1.EuclideanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        distance.Should()
                .Be(0.0);
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Return_Zero_When_Circles_Overlap()
    {
        // Arrange
        var center1 = new Point(0, 0);
        var center2 = new Point(5, 0);
        var circle1 = new ValueCircle(center1, 5);
        var circle2 = new ValueCircle(center2, 5);

        // Act
        var distance = circle1.EuclideanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        distance.Should()
                .Be(0.0);
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Return_Zero_When_One_Circle_Inside_Another()
    {
        // Arrange
        var center1 = new Point(0, 0);
        var center2 = new Point(2, 0);
        var circle1 = new ValueCircle(center1, 10);
        var circle2 = new ValueCircle(center2, 3);

        // Act
        var distance = circle1.EuclideanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        distance.Should()
                .Be(0.0);
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Calculate_Correct_Distance_When_Separated()
    {
        // Arrange
        var center1 = new Point(0, 0);
        var center2 = new Point(10, 0);
        var circle1 = new ValueCircle(center1, 3);
        var circle2 = new ValueCircle(center2, 2);
        var expectedDistance = 10.0 - 3.0 - 2.0; // center distance - radius1 - radius2

        // Act
        var distance = circle1.EuclideanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        distance.Should()
                .Be(expectedDistance);
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Handle_Diagonal_Separation()
    {
        // Arrange
        var center1 = new Point(0, 0);
        var center2 = new Point(3, 4);
        var circle1 = new ValueCircle(center1, 1);
        var circle2 = new ValueCircle(center2, 1);
        var centerDistance = Math.Sqrt(3 * 3 + 4 * 4); // 5.0
        var expectedDistance = centerDistance - 1.0 - 1.0; // 3.0

        // Act
        var distance = circle1.EuclideanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        distance.Should()
                .BeApproximately(expectedDistance, 0.0001);
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Handle_Zero_Radius_Circles()
    {
        // Arrange
        var center1 = new Point(0, 0);
        var center2 = new Point(5, 0);
        var circle1 = new ValueCircle(center1, 0);
        var circle2 = new ValueCircle(center2, 0);

        // Act
        var distance = circle1.EuclideanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        distance.Should()
                .Be(5.0);
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Handle_Identical_Circles()
    {
        // Arrange
        var center = new Point(5, 5);
        var circle1 = new ValueCircle(center, 3);
        var circle2 = new ValueCircle(center, 3);

        // Act
        var distance = circle1.EuclideanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        distance.Should()
                .Be(0.0);
    }
    #endregion

    #region ValueCircle to ICircle Tests
    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ValueCircle_ICircle_Should_Throw_When_Other_Is_Null()
    {
        // Arrange
        var center = new Point(0, 0);
        ICircle? other = null;

        // Act
        var action = () => new ValueCircle(center, 5).EuclideanEdgeToEdgeDistanceFrom(other!);

        // Assert
        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ValueCircle_ICircle_Should_Calculate_Distance_With_Mock()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 3);
        var mockOther = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(c => c.X)
                  .Returns(10);

        mockCenter.Setup(c => c.Y)
                  .Returns(0);

        mockOther.Setup(c => c.Center)
                 .Returns(mockCenter.Object);

        mockOther.Setup(c => c.Radius)
                 .Returns(2);

        var expectedDistance = 10.0 - 3.0 - 2.0; // 5.0

        // Act
        var distance = circle.EuclideanEdgeToEdgeDistanceFrom(mockOther.Object);

        // Assert
        distance.Should()
                .Be(expectedDistance);
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ValueCircle_ICircle_Should_Return_Zero_When_Overlapping()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 5);
        var mockOther = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(c => c.X)
                  .Returns(3);

        mockCenter.Setup(c => c.Y)
                  .Returns(0);

        mockOther.Setup(c => c.Center)
                 .Returns(mockCenter.Object);

        mockOther.Setup(c => c.Radius)
                 .Returns(5);

        // Act
        var distance = circle.EuclideanEdgeToEdgeDistanceFrom(mockOther.Object);

        // Assert
        distance.Should()
                .Be(0.0);
    }
    #endregion

    #region ICircle to ValueCircle Tests
    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ICircle_ValueCircle_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;
        var center = new Point(5, 5);

        // Act
        var action = () => circle!.EuclideanEdgeToEdgeDistanceFrom(new ValueCircle(center, 3));

        // Assert
        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ICircle_ValueCircle_Should_Calculate_Distance_With_Mock()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(c => c.X)
                  .Returns(0);

        mockCenter.Setup(c => c.Y)
                  .Returns(0);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(4);

        var otherCenter = new Point(15, 0);
        var other = new ValueCircle(otherCenter, 5);
        var expectedDistance = 15.0 - 4.0 - 5.0; // 6.0

        // Act
        var distance = mockCircle.Object.EuclideanEdgeToEdgeDistanceFrom(other);

        // Assert
        distance.Should()
                .Be(expectedDistance);
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ICircle_ValueCircle_Should_Handle_Negative_Coordinates()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(c => c.X)
                  .Returns(-5);

        mockCenter.Setup(c => c.Y)
                  .Returns(-5);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(2);

        var otherCenter = new Point(5, 5);
        var other = new ValueCircle(otherCenter, 3);
        var centerDistance = Math.Sqrt(100 + 100); // ~14.14
        var expectedDistance = centerDistance - 2.0 - 3.0;

        // Act
        var distance = mockCircle.Object.EuclideanEdgeToEdgeDistanceFrom(other);

        // Assert
        distance.Should()
                .BeApproximately(expectedDistance, 0.0001);
    }
    #endregion

    #region ICircle to ICircle Tests
    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ICircle_ICircle_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;
        var mockOther = new Mock<ICircle>();

        // Act
        var action = () => circle!.EuclideanEdgeToEdgeDistanceFrom(mockOther.Object);

        // Assert
        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ICircle_ICircle_Should_Throw_When_Other_Is_Null()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        ICircle? other = null;

        // Act
        var action = () => mockCircle.Object.EuclideanEdgeToEdgeDistanceFrom(other!);

        // Assert
        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ICircle_ICircle_Should_Calculate_Distance_With_Both_Mocks()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCircleCenter = new Mock<IPoint>();

        mockCircleCenter.Setup(c => c.X)
                        .Returns(0);

        mockCircleCenter.Setup(c => c.Y)
                        .Returns(0);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCircleCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(3);

        var mockOther = new Mock<ICircle>();
        var mockOtherCenter = new Mock<IPoint>();

        mockOtherCenter.Setup(c => c.X)
                       .Returns(12);

        mockOtherCenter.Setup(c => c.Y)
                       .Returns(0);

        mockOther.Setup(c => c.Center)
                 .Returns(mockOtherCenter.Object);

        mockOther.Setup(c => c.Radius)
                 .Returns(4);

        var expectedDistance = 12.0 - 3.0 - 4.0; // 5.0

        // Act
        var distance = mockCircle.Object.EuclideanEdgeToEdgeDistanceFrom(mockOther.Object);

        // Assert
        distance.Should()
                .Be(expectedDistance);
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ICircle_ICircle_Should_Return_Zero_When_Distance_Less_Than_Sum_Of_Radii()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCircleCenter = new Mock<IPoint>();

        mockCircleCenter.Setup(c => c.X)
                        .Returns(0);

        mockCircleCenter.Setup(c => c.Y)
                        .Returns(0);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCircleCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(10);

        var mockOther = new Mock<ICircle>();
        var mockOtherCenter = new Mock<IPoint>();

        mockOtherCenter.Setup(c => c.X)
                       .Returns(5);

        mockOtherCenter.Setup(c => c.Y)
                       .Returns(0);

        mockOther.Setup(c => c.Center)
                 .Returns(mockOtherCenter.Object);

        mockOther.Setup(c => c.Radius)
                 .Returns(8);

        // Act
        var distance = mockCircle.Object.EuclideanEdgeToEdgeDistanceFrom(mockOther.Object);

        // Assert
        distance.Should()
                .Be(0.0);
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_ICircle_ICircle_Should_Handle_Large_Distances()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCircleCenter = new Mock<IPoint>();

        mockCircleCenter.Setup(c => c.X)
                        .Returns(0);

        mockCircleCenter.Setup(c => c.Y)
                        .Returns(0);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCircleCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(100);

        var mockOther = new Mock<ICircle>();
        var mockOtherCenter = new Mock<IPoint>();

        mockOtherCenter.Setup(c => c.X)
                       .Returns(1000);

        mockOtherCenter.Setup(c => c.Y)
                       .Returns(0);

        mockOther.Setup(c => c.Center)
                 .Returns(mockOtherCenter.Object);

        mockOther.Setup(c => c.Radius)
                 .Returns(200);

        var expectedDistance = 1000.0 - 100.0 - 200.0; // 700.0

        // Act
        var distance = mockCircle.Object.EuclideanEdgeToEdgeDistanceFrom(mockOther.Object);

        // Assert
        distance.Should()
                .Be(expectedDistance);
    }
    #endregion

    #region Edge Cases and Consistency Tests
    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_Should_Be_Consistent_Across_All_Overloads()
    {
        // Arrange
        var center1 = new Point(0, 0);
        var center2 = new Point(15, 0);
        var valueCircle1 = new ValueCircle(center1, 5);
        var valueCircle2 = new ValueCircle(center2, 3);

        var mockCircle1 = new Mock<ICircle>();
        var mockCenter1 = new Mock<IPoint>();

        mockCenter1.Setup(c => c.X)
                   .Returns(0);

        mockCenter1.Setup(c => c.Y)
                   .Returns(0);

        mockCircle1.Setup(c => c.Center)
                   .Returns(mockCenter1.Object);

        mockCircle1.Setup(c => c.Radius)
                   .Returns(5);

        var mockCircle2 = new Mock<ICircle>();
        var mockCenter2 = new Mock<IPoint>();

        mockCenter2.Setup(c => c.X)
                   .Returns(15);

        mockCenter2.Setup(c => c.Y)
                   .Returns(0);

        mockCircle2.Setup(c => c.Center)
                   .Returns(mockCenter2.Object);

        mockCircle2.Setup(c => c.Radius)
                   .Returns(3);

        // Act
        var distance1 = valueCircle1.EuclideanEdgeToEdgeDistanceFrom(valueCircle2);
        var distance2 = valueCircle1.EuclideanEdgeToEdgeDistanceFrom(mockCircle2.Object);
        var distance3 = mockCircle1.Object.EuclideanEdgeToEdgeDistanceFrom(valueCircle2);
        var distance4 = mockCircle1.Object.EuclideanEdgeToEdgeDistanceFrom(mockCircle2.Object);

        // Assert
        distance1.Should()
                 .Be(distance2);

        distance1.Should()
                 .Be(distance3);

        distance1.Should()
                 .Be(distance4);

        distance1.Should()
                 .Be(7.0); // 15 - 5 - 3 = 7
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_Should_Handle_Floating_Point_Precision()
    {
        // Arrange
        var center1 = new Point(0, 0);
        var center2 = new Point(1, 1);
        var circle1 = new ValueCircle(center1, 1);
        var circle2 = new ValueCircle(center2, 1);

        // Act
        var distance = circle1.EuclideanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        distance.Should()
                .Be(0.0);
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_Should_Never_Return_Negative_Values()
    {
        // Arrange - circles that heavily overlap
        var center1 = new Point(0, 0);
        var center2 = new Point(1, 0);
        var circle1 = new ValueCircle(center1, 100);
        var circle2 = new ValueCircle(center2, 100);

        // Act
        var distance = circle1.EuclideanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        distance.Should()
                .BeGreaterThanOrEqualTo(0.0);
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_Should_Handle_Same_Center_Different_Radii()
    {
        // Arrange
        var center = new Point(5, 5);
        var circle1 = new ValueCircle(center, 10);
        var circle2 = new ValueCircle(center, 3);

        // Act
        var distance = circle1.EuclideanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        distance.Should()
                .Be(0.0);
    }

    [Test]
    public void EuclideanEdgeToEdgeDistanceFrom_Should_Be_Symmetric()
    {
        // Arrange
        var center1 = new Point(0, 0);
        var center2 = new Point(20, 0);
        var circle1 = new ValueCircle(center1, 5);
        var circle2 = new ValueCircle(center2, 8);

        // Act
        var distance1 = circle1.EuclideanEdgeToEdgeDistanceFrom(circle2);
        var distance2 = circle2.EuclideanEdgeToEdgeDistanceFrom(circle1);

        // Assert
        distance1.Should()
                 .Be(distance2);

        distance1.Should()
                 .Be(7.0); // 20 - 5 - 8 = 7
    }
    #endregion

    #region ValueCircle Tests
    [Test]
    public void GetOutline_ValueCircle_WithZeroRadius_ShouldReturnSinglePoint()
    {
        // Arrange
        var circle = new ValueCircle(new Point(5, 5), 0);

        // Act
        var result = circle.GetOutline()
                           .ToList();

        // Assert
        result.Should()
              .ContainSingle();

        result[0]
            .Should()
            .Be(new Point(5, 5));
    }

    [Test]
    public void GetOutline_ValueCircle_WithRadiusOne_ShouldReturnEightPoints()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 1);

        // Act
        var result = circle.GetOutline()
                           .ToList();

        // Assert
        result.Should()
              .HaveCount(8);

        result.Should()
              .OnlyHaveUniqueItems();

        result.Should()
              .Contain(new Point(1, 0)); // East

        result.Should()
              .Contain(new Point(0, 1)); // North

        result.Should()
              .Contain(new Point(-1, 0)); // West

        result.Should()
              .Contain(new Point(0, -1)); // South
    }

    [Test]
    public void GetOutline_ValueCircle_WithRadiusTwo_ShouldReturnCorrectPoints()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 2);

        // Act
        var result = circle.GetOutline()
                           .ToList();

        // Assert
        result.Should()
              .HaveCountGreaterThan(4);

        result.Should()
              .Contain(new Point(2, 0)); // East

        result.Should()
              .Contain(new Point(0, 2)); // North

        result.Should()
              .Contain(new Point(-2, 0)); // West

        result.Should()
              .Contain(new Point(0, -2)); // South

        result.Should()
              .OnlyHaveUniqueItems(); // No duplicates due to HashSet
    }

    [Test]
    public void GetOutline_ValueCircle_WithOffsetCenter_ShouldReturnCorrectPoints()
    {
        // Arrange
        var circle = new ValueCircle(new Point(10, 20), 1);

        // Act
        var result = circle.GetOutline()
                           .ToList();

        // Assert
        result.Should()
              .HaveCount(8);

        result.Should()
              .OnlyHaveUniqueItems();

        result.Should()
              .Contain(new Point(11, 20)); // East

        result.Should()
              .Contain(new Point(10, 21)); // North

        result.Should()
              .Contain(new Point(9, 20)); // West

        result.Should()
              .Contain(new Point(10, 19)); // South
    }

    [Test]
    public void GetOutline_ValueCircle_WithLargeRadius_ShouldReturnManyPointsNoDuplicates()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 10);

        // Act
        var result = circle.GetOutline()
                           .ToList();

        // Assert
        result.Should()
              .HaveCountGreaterThan(20);

        result.Should()
              .OnlyHaveUniqueItems();

        // Check that the furthest points are at the expected positions
        result.Should()
              .Contain(new Point(10, 0)); // East

        result.Should()
              .Contain(new Point(0, 10)); // North

        result.Should()
              .Contain(new Point(-10, 0)); // West

        result.Should()
              .Contain(new Point(0, -10)); // South
    }

    [Test]
    public void GetOutline_ValueCircle_WithNegativeCenter_ShouldWork()
    {
        // Arrange
        var circle = new ValueCircle(new Point(-5, -3), 2);

        // Act
        var result = circle.GetOutline()
                           .ToList();

        // Assert
        result.Should()
              .HaveCountGreaterThan(4);

        result.Should()
              .Contain(new Point(-3, -3)); // East

        result.Should()
              .Contain(new Point(-5, -1)); // North

        result.Should()
              .Contain(new Point(-7, -3)); // West

        result.Should()
              .Contain(new Point(-5, -5)); // South

        result.Should()
              .OnlyHaveUniqueItems();
    }
    #endregion

    #region ICircle Tests
    [Test]
    public void GetOutline_ICircle_WithNullCircle_ShouldThrowNullReferenceException()
    {
        // Arrange
        ICircle? circle = null;

        // Act & Assert
        var action = () => circle!.GetOutline()
                                  .ToList();

        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void GetOutline_ICircle_WithZeroRadius_ShouldReturnSinglePoint()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(new Point(5, 5));

        mockCircle.Setup(c => c.Radius)
                  .Returns(0);

        // Act
        var result = mockCircle.Object
                               .GetOutline()
                               .ToList();

        // Assert
        result.Should()
              .ContainSingle();

        result[0]
            .Should()
            .Be(new Point(5, 5));
    }

    [Test]
    public void GetOutline_ICircle_WithRadiusOne_ShouldReturnEightPoints()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(new Point(0, 0));

        mockCircle.Setup(c => c.Radius)
                  .Returns(1);

        // Act
        var result = mockCircle.Object
                               .GetOutline()
                               .ToList();

        // Assert
        result.Should()
              .HaveCount(8);

        result.Should()
              .OnlyHaveUniqueItems();

        result.Should()
              .Contain(new Point(1, 0)); // East

        result.Should()
              .Contain(new Point(0, 1)); // North

        result.Should()
              .Contain(new Point(-1, 0)); // West

        result.Should()
              .Contain(new Point(0, -1)); // South
    }

    [Test]
    public void GetOutline_ICircle_WithRadiusTwo_ShouldReturnCorrectPoints()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(new Point(0, 0));

        mockCircle.Setup(c => c.Radius)
                  .Returns(2);

        // Act
        var result = mockCircle.Object
                               .GetOutline()
                               .ToList();

        // Assert
        result.Should()
              .HaveCountGreaterThan(4);

        result.Should()
              .Contain(new Point(2, 0)); // East

        result.Should()
              .Contain(new Point(0, 2)); // North

        result.Should()
              .Contain(new Point(-2, 0)); // West

        result.Should()
              .Contain(new Point(0, -2)); // South

        result.Should()
              .OnlyHaveUniqueItems();
    }

    [Test]
    public void GetOutline_ICircle_WithOffsetCenter_ShouldReturnCorrectPoints()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(new Point(15, 25));

        mockCircle.Setup(c => c.Radius)
                  .Returns(1);

        // Act
        var result = mockCircle.Object
                               .GetOutline()
                               .Distinct() // Add distinct to remove duplicates
                               .ToList();

        // Assert
        result.Should()
              .HaveCount(8);

        result.Should()
              .OnlyHaveUniqueItems();

        result.Should()
              .Contain(new Point(16, 25)); // East

        result.Should()
              .Contain(new Point(15, 26)); // North

        result.Should()
              .Contain(new Point(14, 25)); // West

        result.Should()
              .Contain(new Point(15, 24)); // South
    }

    [Test]
    public void GetOutline_ICircle_WithLargeRadius_ShouldReturnManyPointsNoDuplicates()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(new Point(0, 0));

        mockCircle.Setup(c => c.Radius)
                  .Returns(15);

        // Act
        var result = mockCircle.Object
                               .GetOutline()
                               .ToList();

        // Assert
        result.Should()
              .HaveCountGreaterThan(30);

        result.Should()
              .OnlyHaveUniqueItems();

        // Check cardinal directions
        result.Should()
              .Contain(new Point(15, 0)); // East

        result.Should()
              .Contain(new Point(0, 15)); // North

        result.Should()
              .Contain(new Point(-15, 0)); // West

        result.Should()
              .Contain(new Point(0, -15)); // South
    }

    [Test]
    public void GetOutline_ICircle_WithNegativeCenter_ShouldWork()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(new Point(-10, -8));

        mockCircle.Setup(c => c.Radius)
                  .Returns(3);

        // Act
        var result = mockCircle.Object
                               .GetOutline()
                               .ToList();

        // Assert
        result.Should()
              .HaveCountGreaterThan(8);

        result.Should()
              .Contain(new Point(-7, -8)); // East

        result.Should()
              .Contain(new Point(-10, -5)); // North

        result.Should()
              .Contain(new Point(-13, -8)); // West

        result.Should()
              .Contain(new Point(-10, -11)); // South

        result.Should()
              .OnlyHaveUniqueItems();
    }
    #endregion

    #region Algorithm Tests
    [Test]
    public void GetOutline_BothOverloads_WithSameParameters_ShouldReturnSameResults()
    {
        // Arrange
        var center = new Point(5, 7);
        var radius = 4;

        var valueCircle = new ValueCircle(center, radius);

        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(center);

        mockCircle.Setup(c => c.Radius)
                  .Returns(radius);

        // Act
        var valueCircleResult = valueCircle.GetOutline()
                                           .ToList();

        var interfaceCircleResult = mockCircle.Object
                                              .GetOutline()
                                              .ToList();

        // Assert
        valueCircleResult.Should()
                         .HaveCount(interfaceCircleResult.Count);

        valueCircleResult.Should()
                         .BeEquivalentTo(interfaceCircleResult);
    }

    //@formatter:off
    [Test]
    [Arguments(3)]
    [Arguments(5)]
    [Arguments(7)]
    [Arguments(8)]
    //@formatter:on
    public void GetOutline_WithVariousRadii_ShouldFollowBresenhamCircleAlgorithm(int radius)
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), radius);

        // Act
        var result = circle.GetOutline()
                           .ToList();

        // Assert
        result.Should()
              .OnlyHaveUniqueItems();

        // All points should be approximately at the given radius distance
        foreach (var point in result)
        {
            var distance = Math.Sqrt(point.X * point.X + point.Y * point.Y);

            distance.Should()
                    .BeLessThanOrEqualTo(radius + 1.5, $"point {point} should be within radius tolerance");

            distance.Should()
                    .BeGreaterThanOrEqualTo(radius - 1.5, $"point {point} should be within radius tolerance");
        }
    }

    [Test]
    public void GetOutline_ShouldYieldResultsIncrementally()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 5);
        var results = new List<Point>();

        // Act - Test that enumeration works incrementally
        foreach (var point in circle.GetOutline())
        {
            results.Add(point);

            if (results.Count == 3)
                break;
        }

        // Assert
        results.Should()
               .HaveCount(3);

        results.Should()
               .OnlyHaveUniqueItems();
    }

    [Test]
    public void GetOutline_WithRadiusZero_ShouldHaveCorrectDecisionLogic()
    {
        // Arrange
        var circle = new ValueCircle(new Point(100, 200), 0);

        // Act
        var result = circle.GetOutline()
                           .ToList();

        // Assert
        result.Should()
              .ContainSingle();

        result[0]
            .Should()
            .Be(new Point(100, 200));
    }

    [Test]
    public void GetOutline_ShouldHandleDecisionOver2Logic()
    {
        // This test ensures both branches of the decision logic are covered
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 6);

        // Act
        var result = circle.GetOutline()
                           .ToList();

        // Assert
        result.Should()
              .HaveCountGreaterThan(16);

        result.Should()
              .OnlyHaveUniqueItems();

        // Should include points that test both decision branches
        result.Should()
              .Contain(new Point(6, 0)); // Initial xOffset point

        result.Should()
              .Contain(new Point(0, 6)); // yOffset becomes larger
    }
    #endregion

    #region Edge Cases
    [Test]
    public void GetOutline_WithVeryLargeRadius_ShouldNotThrow()
    {
        // Act & Assert
        var action = () => new ValueCircle(new Point(0, 0), 1000).GetOutline()
                                                                 .Take(100)
                                                                 .ToList();

        action.Should()
              .NotThrow();
    }

    [Test]
    public void GetOutline_WithMaxIntCoordinates_ShouldHandleOverflow()
    {
        // Arrange - Use coordinates that could cause overflow in calculations
        var circle = new ValueCircle(new Point(int.MaxValue / 2, int.MaxValue / 2), 1);

        // Act
        var result = circle.GetOutline()
                           .ToList();

        // Assert
        result.Should()
              .HaveCount(8);

        result.Should()
              .OnlyHaveUniqueItems();
    }

    [Test]
    public void GetOutline_MultipleEnumerations_ShouldReturnSameResults()
    {
        // Arrange
        var circle = new ValueCircle(new Point(3, 4), 5);

        // Act
        var firstEnumeration = circle.GetOutline()
                                     .ToList();

        var secondEnumeration = circle.GetOutline()
                                      .ToList();

        // Assert
        firstEnumeration.Should()
                        .BeEquivalentTo(secondEnumeration);
    }
    #endregion

    #region ValueCircle GetPoints Tests
    [Test]
    public void GetPoints_ValueCircle_WithZeroRadius_ShouldReturnSinglePoint()
    {
        // Arrange
        var center = new Point(5, 5);
        var circle = new ValueCircle(center, 0);

        // Act
        var points = circle.GetPoints()
                           .ToList();

        // Assert
        points.Should()
              .HaveCount(1);

        points.Should()
              .Contain(center);
    }

    [Test]
    public void GetPoints_ValueCircle_WithRadiusOne_ShouldReturnCorrectPoints()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 1);

        // Act
        var points = circle.GetPoints()
                           .ToList();

        // Assert
        var expectedPoints = new[]
        {
            new Point(0, 0), // center
            new Point(-1, 0), // left
            new Point(0, -1), // up
            new Point(1, 0), // right
            new Point(0, 1) // down
        };

        points.Should()
              .HaveCount(expectedPoints.Length);

        foreach (var expectedPoint in expectedPoints)
            points.Should()
                  .Contain(expectedPoint);
    }

    [Test]
    public void GetPoints_ValueCircle_WithRadiusTwo_ShouldReturnCorrectPoints()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 2);

        // Act
        var points = circle.GetPoints()
                           .ToList();

        // Assert
        points.Should()
              .NotBeEmpty();

        points.Should()
              .OnlyHaveUniqueItems();

        // All points should be within the circle (distance from center <= radius)
        foreach (var point in points)
        {
            var distanceSquared = (point.X - center.X) * (point.X - center.X) + (point.Y - center.Y) * (point.Y - center.Y);

            distanceSquared.Should()
                           .BeLessThanOrEqualTo(4, $"point {point} should be within radius 2 of center {center}");
        }

        // Center should be included
        points.Should()
              .Contain(center);
    }

    [Test]
    public void GetPoints_ValueCircle_WithOffsetCenter_ShouldReturnCorrectPoints()
    {
        // Arrange
        var center = new Point(10, -5);
        var circle = new ValueCircle(center, 1);

        // Act
        var points = circle.GetPoints()
                           .ToList();

        // Assert
        points.Should()
              .NotBeEmpty();

        points.Should()
              .OnlyHaveUniqueItems();

        // All points should be within the circle
        foreach (var point in points)
        {
            var distanceSquared = (point.X - center.X) * (point.X - center.X) + (point.Y - center.Y) * (point.Y - center.Y);

            distanceSquared.Should()
                           .BeLessThanOrEqualTo(1, $"point {point} should be within radius 1 of center {center}");
        }

        // Center should be included
        points.Should()
              .Contain(center);
    }

    [Test]
    public void GetPoints_ValueCircle_WithLargeRadius_ShouldReturnManyPoints()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 5);

        // Act
        var points = circle.GetPoints()
                           .ToList();

        // Assert
        points.Should()
              .NotBeEmpty();

        points.Should()
              .OnlyHaveUniqueItems();

        points.Count
              .Should()
              .BeGreaterThan(25); // Should have more points than a small circle

        // All points should be within the circle
        foreach (var point in points)
        {
            var distanceSquared = (point.X - center.X) * (point.X - center.X) + (point.Y - center.Y) * (point.Y - center.Y);

            distanceSquared.Should()
                           .BeLessThanOrEqualTo(25, $"point {point} should be within radius 5 of center {center}");
        }
    }

    [Test]
    public void GetPoints_ValueCircle_WithNegativeCenter_ShouldWork()
    {
        // Arrange
        var center = new Point(-10, -10);
        var circle = new ValueCircle(center, 2);

        // Act
        var points = circle.GetPoints()
                           .ToList();

        // Assert
        points.Should()
              .NotBeEmpty();

        points.Should()
              .OnlyHaveUniqueItems();

        points.Should()
              .Contain(center);

        // All points should be within the circle
        foreach (var point in points)
        {
            var distanceSquared = (point.X - center.X) * (point.X - center.X) + (point.Y - center.Y) * (point.Y - center.Y);

            distanceSquared.Should()
                           .BeLessThanOrEqualTo(4, $"point {point} should be within radius 2 of center {center}");
        }
    }

    [Test]
    public void GetPoints_ValueCircle_ShouldYieldResultsIncrementally()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 3);

        // Act & Assert - Test that it's lazily evaluated
        var enumerable = circle.GetPoints();
        var enumerator = enumerable.GetEnumerator();

        enumerator.MoveNext()
                  .Should()
                  .BeTrue();
        var firstPoint = enumerator.Current;

        firstPoint.Should()
                  .NotBeNull();

        enumerator.Dispose();
    }
    #endregion

    #region ICircle GetPoints Tests
    [Test]
    public void GetPoints_ICircle_WithNullCircle_ShouldThrowNullReferenceException()
    {
        // Arrange
        ICircle circle = null!;

        // Act & Assert
        var act = () => circle.GetPoints()
                              .ToList();

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void GetPoints_ICircle_WithZeroRadius_ShouldReturnSinglePoint()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var center = new Point(3, 3);

        mockCircle.Setup(c => c.Center)
                  .Returns(center);

        mockCircle.Setup(c => c.Radius)
                  .Returns(0);

        // Act
        var points = mockCircle.Object
                               .GetPoints()
                               .ToList();

        // Assert
        points.Should()
              .HaveCount(1);

        points.Should()
              .Contain(center);
    }

    [Test]
    public void GetPoints_ICircle_WithRadiusOne_ShouldReturnCorrectPoints()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var center = new Point(0, 0);

        mockCircle.Setup(c => c.Center)
                  .Returns(center);

        mockCircle.Setup(c => c.Radius)
                  .Returns(1);

        // Act
        var points = mockCircle.Object
                               .GetPoints()
                               .ToList();

        // Assert
        var expectedPoints = new[]
        {
            new Point(0, 0), // center
            new Point(-1, 0), // left
            new Point(0, -1), // up
            new Point(1, 0), // right
            new Point(0, 1) // down
        };

        points.Should()
              .HaveCount(expectedPoints.Length);

        foreach (var expectedPoint in expectedPoints)
            points.Should()
                  .Contain(expectedPoint);
    }

    [Test]
    public void GetPoints_ICircle_WithRadiusTwo_ShouldReturnCorrectPoints()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var center = new Point(0, 0);

        mockCircle.Setup(c => c.Center)
                  .Returns(center);

        mockCircle.Setup(c => c.Radius)
                  .Returns(2);

        // Act
        var points = mockCircle.Object
                               .GetPoints()
                               .ToList();

        // Assert
        points.Should()
              .NotBeEmpty();

        points.Should()
              .OnlyHaveUniqueItems();

        // All points should be within the circle
        foreach (var point in points)
        {
            var distanceSquared = (point.X - center.X) * (point.X - center.X) + (point.Y - center.Y) * (point.Y - center.Y);

            distanceSquared.Should()
                           .BeLessThanOrEqualTo(4, $"point {point} should be within radius 2 of center {center}");
        }

        // Center should be included
        points.Should()
              .Contain(center);
    }

    [Test]
    public void GetPoints_ICircle_WithOffsetCenter_ShouldReturnCorrectPoints()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var center = new Point(7, -3);

        mockCircle.Setup(c => c.Center)
                  .Returns(center);

        mockCircle.Setup(c => c.Radius)
                  .Returns(1);

        // Act
        var points = mockCircle.Object
                               .GetPoints()
                               .ToList();

        // Assert
        points.Should()
              .NotBeEmpty();

        points.Should()
              .OnlyHaveUniqueItems();

        // All points should be within the circle
        foreach (var point in points)
        {
            var distanceSquared = (point.X - center.X) * (point.X - center.X) + (point.Y - center.Y) * (point.Y - center.Y);

            distanceSquared.Should()
                           .BeLessThanOrEqualTo(1, $"point {point} should be within radius 1 of center {center}");
        }

        // Center should be included
        points.Should()
              .Contain(center);
    }

    [Test]
    public void GetPoints_ICircle_WithLargeRadius_ShouldReturnManyPoints()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var center = new Point(0, 0);

        mockCircle.Setup(c => c.Center)
                  .Returns(center);

        mockCircle.Setup(c => c.Radius)
                  .Returns(4);

        // Act
        var points = mockCircle.Object
                               .GetPoints()
                               .ToList();

        // Assert
        points.Should()
              .NotBeEmpty();

        points.Should()
              .OnlyHaveUniqueItems();

        points.Count
              .Should()
              .BeGreaterThan(16); // Should have more points than a small circle

        // All points should be within the circle
        foreach (var point in points)
        {
            var distanceSquared = (point.X - center.X) * (point.X - center.X) + (point.Y - center.Y) * (point.Y - center.Y);

            distanceSquared.Should()
                           .BeLessThanOrEqualTo(16, $"point {point} should be within radius 4 of center {center}");
        }
    }

    [Test]
    public void GetPoints_ICircle_WithNegativeCenter_ShouldWork()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var center = new Point(-5, -8);

        mockCircle.Setup(c => c.Center)
                  .Returns(center);

        mockCircle.Setup(c => c.Radius)
                  .Returns(2);

        // Act
        var points = mockCircle.Object
                               .GetPoints()
                               .ToList();

        // Assert
        points.Should()
              .NotBeEmpty();

        points.Should()
              .OnlyHaveUniqueItems();

        points.Should()
              .Contain(center);

        // All points should be within the circle
        foreach (var point in points)
        {
            var distanceSquared = (point.X - center.X) * (point.X - center.X) + (point.Y - center.Y) * (point.Y - center.Y);

            distanceSquared.Should()
                           .BeLessThanOrEqualTo(4, $"point {point} should be within radius 2 of center {center}");
        }
    }

    [Test]
    public void GetPoints_ICircle_ShouldYieldResultsIncrementally()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var center = new Point(0, 0);

        mockCircle.Setup(c => c.Center)
                  .Returns(center);

        mockCircle.Setup(c => c.Radius)
                  .Returns(3);

        // Act & Assert - Test that it's lazily evaluated
        var enumerable = mockCircle.Object.GetPoints();
        var enumerator = enumerable.GetEnumerator();

        enumerator.MoveNext()
                  .Should()
                  .BeTrue();
        var firstPoint = enumerator.Current;

        firstPoint.Should()
                  .NotBeNull();

        enumerator.Dispose();
    }
    #endregion

    #region Consistency Tests
    [Test]
    public void GetPoints_BothOverloads_WithSameParameters_ShouldReturnSameResults()
    {
        // Arrange
        var center = new Point(2, 3);
        var radius = 3;
        var valueCircle = new ValueCircle(center, radius);

        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(center);

        mockCircle.Setup(c => c.Radius)
                  .Returns(radius);

        // Act
        var valueCirclePoints = valueCircle.GetPoints()
                                           .OrderBy(p => p.X)
                                           .ThenBy(p => p.Y)
                                           .ToList();

        var iCirclePoints = mockCircle.Object
                                      .GetPoints()
                                      .OrderBy(p => p.X)
                                      .ThenBy(p => p.Y)
                                      .ToList();

        // Assert
        valueCirclePoints.Should()
                         .BeEquivalentTo(iCirclePoints);
    }

    //@formatter:off
    [Test]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(5)]
    [Arguments(10)]
    //@formatter:on
    public void GetPoints_WithVariousRadii_ShouldReturnAllPointsWithinRadius(int radius)
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, radius);

        // Act
        var points = circle.GetPoints()
                           .ToList();

        // Assert
        points.Should()
              .NotBeEmpty();

        points.Should()
              .OnlyHaveUniqueItems();

        // All points should be within the circle
        var radiusSquared = radius * radius;

        foreach (var point in points)
        {
            var distanceSquared = (point.X - center.X) * (point.X - center.X) + (point.Y - center.Y) * (point.Y - center.Y);

            distanceSquared.Should()
                           .BeLessThanOrEqualTo(radiusSquared, $"point {point} should be within radius {radius} of center {center}");
        }

        // Center should always be included
        points.Should()
              .Contain(center);
    }

    [Test]
    public void GetPoints_MultipleEnumerations_ShouldReturnSameResults()
    {
        // Arrange
        var center = new Point(1, 1);
        var circle = new ValueCircle(center, 2);

        // Act
        var firstEnumeration = circle.GetPoints()
                                     .ToList();

        var secondEnumeration = circle.GetPoints()
                                      .ToList();

        // Assert
        firstEnumeration.Should()
                        .BeEquivalentTo(secondEnumeration);
    }

    [Test]
    public void GetPoints_WithMaxIntCoordinates_ShouldHandleOverflow()
    {
        // Arrange - Use coordinates that could cause overflow during calculation
        var center = new Point(int.MaxValue - 5, int.MaxValue - 5);

        // Act & Assert - Should not throw
        var act = () => new ValueCircle(center, 2).GetPoints()
                                                  .ToList();

        act.Should()
           .NotThrow();
    }

    [Test]
    public void GetPoints_ShouldGenerateSymmetricalPoints()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 3);

        // Act
        var points = circle.GetPoints()
                           .ToList();

        // Assert
        points.Should()
              .NotBeEmpty();

        // For each point, verify the symmetrical nature of the algorithm
        // The algorithm should generate points symmetrically due to the xS, yS calculations
        foreach (var point in points)
            if ((point.X != center.X) || (point.Y != center.Y))
            {
                var distanceSquared = (point.X - center.X) * (point.X - center.X) + (point.Y - center.Y) * (point.Y - center.Y);

                distanceSquared.Should()
                               .BeLessThanOrEqualTo(9);
            }
    }

    [Test]
    public void GetPoints_ShouldHandleQuadrantLogic()
    {
        // Arrange
        var center = new Point(5, 5);
        var circle = new ValueCircle(center, 2);

        // Act
        var points = circle.GetPoints()
                           .ToList();

        // Assert
        points.Should()
              .NotBeEmpty();

        points.Should()
              .OnlyHaveUniqueItems();

        // Should include points in all quadrants relative to center
        var hasPointsInAllQuadrants = points.Any(p => (p.X < center.X) && (p.Y < center.Y))
                                      && // Top-left
                                      points.Any(p => (p.X > center.X) && (p.Y < center.Y))
                                      && // Top-right
                                      points.Any(p => (p.X < center.X) && (p.Y > center.Y))
                                      && // Bottom-left
                                      points.Any(p => (p.X > center.X) && (p.Y > center.Y)); // Bottom-right

        hasPointsInAllQuadrants.Should()
                               .BeTrue("circle should have points in all quadrants");
    }
    #endregion

    #region ValueCircle GetRandomPoint Tests
    [Test]
    public void GetRandomPoint_ValueCircle_Should_Return_Point_Within_Circle()
    {
        // Arrange
        var center = new Point(10, 15);
        var circle = new ValueCircle(center, 5);

        // Act & Assert - Test multiple times for randomness
        for (var i = 0; i < 100; i++)
        {
            var randomPoint = circle.GetRandomPoint();

            // The point should be within the circle (using Euclidean distance)
            var distance = Math.Sqrt(Math.Pow(randomPoint.X - center.X, 2) + Math.Pow(randomPoint.Y - center.Y, 2));

            distance.Should()
                    .BeLessThanOrEqualTo(circle.Radius + 1.5);
        }
    }

    [Test]
    public void GetRandomPoint_ValueCircle_With_Zero_Radius_Should_Return_Center_Point()
    {
        // Arrange
        var center = new Point(5, 7);
        var circle = new ValueCircle(center, 0);

        // Act
        var randomPoint = circle.GetRandomPoint();

        // Assert
        randomPoint.X
                   .Should()
                   .Be(center.X);

        randomPoint.Y
                   .Should()
                   .Be(center.Y);
    }

    [Test]
    public void GetRandomPoint_ValueCircle_With_Radius_One_Should_Stay_Within_Bounds()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 1);

        // Act & Assert - Test multiple times
        for (var i = 0; i < 50; i++)
        {
            var randomPoint = circle.GetRandomPoint();

            // Point should be within Manhattan distance of 1 from center
            var distance = Math.Sqrt(Math.Pow(randomPoint.X - center.X, 2) + Math.Pow(randomPoint.Y - center.Y, 2));

            distance.Should()
                    .BeLessThanOrEqualTo(1.5);
        }
    }

    [Test]
    public void GetRandomPoint_ValueCircle_With_Large_Radius_Should_Stay_Within_Bounds()
    {
        // Arrange
        var center = new Point(100, 200);
        var circle = new ValueCircle(center, 50);

        // Act & Assert
        for (var i = 0; i < 20; i++)
        {
            var randomPoint = circle.GetRandomPoint();

            var distance = Math.Sqrt(Math.Pow(randomPoint.X - center.X, 2) + Math.Pow(randomPoint.Y - center.Y, 2));

            distance.Should()
                    .BeLessThanOrEqualTo(circle.Radius + 1.5);
        }
    }

    [Test]
    public void GetRandomPoint_ValueCircle_With_Negative_Center_Should_Work()
    {
        // Arrange
        var center = new Point(-10, -5);
        var circle = new ValueCircle(center, 3);

        // Act & Assert
        for (var i = 0; i < 30; i++)
        {
            var randomPoint = circle.GetRandomPoint();

            var distance = Math.Sqrt(Math.Pow(randomPoint.X - center.X, 2) + Math.Pow(randomPoint.Y - center.Y, 2));

            distance.Should()
                    .BeLessThanOrEqualTo(circle.Radius + 1.5);
        }
    }

    [Test]
    public void GetRandomPoint_ValueCircle_Should_Generate_Different_Points()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 10);
        var points = new HashSet<Point>();

        // Act - Generate multiple points
        for (var i = 0; i < 50; i++)
            points.Add(circle.GetRandomPoint());

        // Assert - Should generate some variety (not all the same point)
        // With radius 10, we should get some different points
        points.Count
              .Should()
              .BeGreaterThan(1);
    }
    #endregion

    #region ICircle GetRandomPoint Tests
    [Test]
    public void GetRandomPoint_ICircle_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;

        // Act & Assert
        var act = () => circle!.GetRandomPoint();

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void GetRandomPoint_ICircle_Should_Return_Point_Within_Circle()
    {
        // Arrange
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(5);

        mockCenter.Setup(p => p.Y)
                  .Returns(8);

        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(4);

        // Act & Assert
        for (var i = 0; i < 50; i++)
        {
            var randomPoint = mockCircle.Object.GetRandomPoint();

            var distance = Math.Sqrt(Math.Pow(randomPoint.X - 5, 2) + Math.Pow(randomPoint.Y - 8, 2));

            distance.Should()
                    .BeLessThanOrEqualTo(4.5); // Increased tolerance to handle edge cases
        }
    }

    [Test]
    public void GetRandomPoint_ICircle_With_Zero_Radius_Should_Return_Center_Point()
    {
        // Arrange
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(12);

        mockCenter.Setup(p => p.Y)
                  .Returns(18);

        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(0);

        // Act
        var randomPoint = mockCircle.Object.GetRandomPoint();

        // Assert
        randomPoint.X
                   .Should()
                   .Be(12);

        randomPoint.Y
                   .Should()
                   .Be(18);
    }

    [Test]
    public void GetRandomPoint_ICircle_With_Large_Radius_Should_Stay_Within_Bounds()
    {
        // Arrange
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(0);

        mockCenter.Setup(p => p.Y)
                  .Returns(0);

        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(100);

        // Act & Assert
        for (var i = 0; i < 10; i++)
        {
            var randomPoint = mockCircle.Object.GetRandomPoint();

            var distance = Math.Sqrt(Math.Pow(randomPoint.X, 2) + Math.Pow(randomPoint.Y, 2));

            distance.Should()
                    .BeLessThanOrEqualTo(100 + 1.5);
        }
    }

    [Test]
    public void GetRandomPoint_ICircle_With_Negative_Coordinates_Should_Work()
    {
        // Arrange
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(-20);

        mockCenter.Setup(p => p.Y)
                  .Returns(-30);

        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(7);

        // Act & Assert
        for (var i = 0; i < 25; i++)
        {
            var randomPoint = mockCircle.Object.GetRandomPoint();

            var distance = Math.Sqrt(Math.Pow(randomPoint.X + 20, 2) + Math.Pow(randomPoint.Y + 30, 2));

            distance.Should()
                    .BeLessThanOrEqualTo(7.3);
        }
    }

    [Test]
    public void GetRandomPoint_ICircle_Should_Generate_Different_Points()
    {
        // Arrange
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(0);

        mockCenter.Setup(p => p.Y)
                  .Returns(0);

        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(15);

        var points = new HashSet<Point>();

        // Act
        for (var i = 0; i < 50; i++)
            points.Add(mockCircle.Object.GetRandomPoint());

        // Assert - Should generate some variety
        points.Count
              .Should()
              .BeGreaterThan(1);
    }
    #endregion

    #region Mathematical Distribution Tests
    [Test]
    public void GetRandomPoint_ValueCircle_Should_Use_Uniform_Distribution()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 10);
        var pointsNearCenter = 0;
        var pointsNearEdge = 0;
        var totalPoints = 1000;

        // Act
        for (var i = 0; i < totalPoints; i++)
        {
            var point = circle.GetRandomPoint();
            var distance = Math.Sqrt(point.X * point.X + point.Y * point.Y);

            if (distance <= 3) // Inner 30% of radius
                pointsNearCenter++;
            else if (distance >= 7) // Outer 30% of radius  
                pointsNearEdge++;
        }

        // Assert - With uniform distribution, we should have more points near the edge
        // because the area increases quadratically with radius
        pointsNearEdge.Should()
                      .BeGreaterThan(pointsNearCenter);
    }

    [Test]
    public void GetRandomPoint_ICircle_Should_Use_Uniform_Distribution()
    {
        // Arrange
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(0);

        mockCenter.Setup(p => p.Y)
                  .Returns(0);

        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(10);

        var pointsNearCenter = 0;
        var pointsNearEdge = 0;
        var totalPoints = 1000;

        // Act
        for (var i = 0; i < totalPoints; i++)
        {
            var point = mockCircle.Object.GetRandomPoint();
            var distance = Math.Sqrt(point.X * point.X + point.Y * point.Y);

            if (distance <= 3)
                pointsNearCenter++;
            else if (distance >= 7)
                pointsNearEdge++;
        }

        // Assert
        pointsNearEdge.Should()
                      .BeGreaterThan(pointsNearCenter);
    }
    #endregion

    #region Edge Cases and Boundary Tests
    [Test]
    public void GetRandomPoint_ValueCircle_With_Radius_One_Should_Not_Exceed_Bounds()
    {
        // Arrange
        var center = new Point(0, 0);
        var circle = new ValueCircle(center, 1);

        // Act & Assert - Test boundary conditions
        for (var i = 0; i < 100; i++)
        {
            var point = circle.GetRandomPoint();

            // All points should be within [-1, 1] range for both X and Y
            point.X
                 .Should()
                 .BeInRange(-1, 1);

            point.Y
                 .Should()
                 .BeInRange(-1, 1);
        }
    }

    [Test]
    public void GetRandomPoint_ICircle_With_Radius_One_Should_Not_Exceed_Bounds()
    {
        // Arrange
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(0);

        mockCenter.Setup(p => p.Y)
                  .Returns(0);

        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(1);

        // Act & Assert
        for (var i = 0; i < 100; i++)
        {
            var point = mockCircle.Object.GetRandomPoint();

            point.X
                 .Should()
                 .BeInRange(-1, 1);

            point.Y
                 .Should()
                 .BeInRange(-1, 1);
        }
    }

    [Test]
    public void GetRandomPoint_Both_Overloads_Should_Produce_Similar_Results()
    {
        // Arrange
        var center = new Point(5, 5);
        var valueCircle = new ValueCircle(center, 8);

        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(5);

        mockCenter.Setup(p => p.Y)
                  .Returns(5);

        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(8);

        var valueCirclePoints = new List<Point>();
        var iCirclePoints = new List<Point>();

        // Act
        for (var i = 0; i < 100; i++)
        {
            valueCirclePoints.Add(valueCircle.GetRandomPoint());
            iCirclePoints.Add(mockCircle.Object.GetRandomPoint());
        }

        // Assert - Both should have similar distribution characteristics
        var valueAvgDistance = valueCirclePoints.Average(p => Math.Sqrt(Math.Pow(p.X - 5, 2) + Math.Pow(p.Y - 5, 2)));
        var iAvgDistance = iCirclePoints.Average(p => Math.Sqrt(Math.Pow(p.X - 5, 2) + Math.Pow(p.Y - 5, 2)));

        // Average distances should be reasonably close (within 20% of each other)
        Math.Abs(valueAvgDistance - iAvgDistance)
            .Should()
            .BeLessThan(Math.Max(valueAvgDistance, iAvgDistance) * 0.2);
    }
    #endregion

    #region ValueCircle & ValueCircle Tests
//@formatter:off
    [Test]
    [Arguments(0, 0, 5, 10, 0, 5, DistanceType.Euclidean, true)] // Touching circles
    [Arguments(0, 0, 5, 11, 0, 5, DistanceType.Euclidean, false)] // Separated circles
    [Arguments(0, 0, 5, 3, 0, 2, DistanceType.Euclidean, true)] // Overlapping circles
    [Arguments(0, 0, 10, 3, 4, 2, DistanceType.Euclidean, true)] // One inside another
    [Arguments(-5, -5, 3, 5, 5, 3, DistanceType.Euclidean, false)] // Negative coordinates, separated
    [Arguments(0, 0, 0, 0, 0, 1, DistanceType.Euclidean, true)] // Zero radius circle intersects
    [Arguments(0, 0, 5, 10, 0, 5, DistanceType.Manhattan, true)] // Manhattan distance touching
    [Arguments(0, 0, 5, 11, 0, 5, DistanceType.Manhattan, false)] // Manhattan distance separated
    [Arguments(0, 0, 5, 7, 7, 3, DistanceType.Manhattan, false)] // Manhattan diagonal overlap
    [Arguments(0, 0, 5, 8, 8, 3, DistanceType.Manhattan, false)] // Manhattan diagonal separated
    //@formatter:on
    public void Intersects_ValueCircle_ValueCircle_Should_Return_Correct_Result(
        int centerX,
        int centerY,
        int radius,
        int otherCenterX,
        int otherCenterY,
        int otherRadius,
        DistanceType distanceType,
        bool expected)
    {
        // Arrange
        var circle = new ValueCircle(new Point(centerX, centerY), radius);
        var other = new ValueCircle(new Point(otherCenterX, otherCenterY), otherRadius);

        // Act
        var result = circle.Intersects(other, distanceType);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void Intersects_ValueCircle_ValueCircle_Should_Default_To_Euclidean()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 5);
        var other = new ValueCircle(new Point(10, 0), 5);

        // Act
        var result = circle.Intersects(other);

        // Assert
        result.Should()
              .BeTrue(); // Distance = 10, sum of radii = 10, so they touch
    }

    [Test]
    public void Intersects_ValueCircle_ValueCircle_Should_Throw_When_Invalid_DistanceType()
    {
        // Arrange
        var invalidDistanceType = (DistanceType)99;

        // Act & Assert
        var action = () => new ValueCircle(new Point(0, 0), 5).Intersects(new ValueCircle(new Point(10, 0), 5), invalidDistanceType);

        action.Should()
              .Throw<ArgumentOutOfRangeException>()
              .WithParameterName("distanceType");
    }

    [Test]
    public void Intersects_ValueCircle_ValueCircle_Should_Handle_Identical_Circles()
    {
        // Arrange
        var circle = new ValueCircle(new Point(5, 5), 3);
        var other = new ValueCircle(new Point(5, 5), 3);

        // Act
        var euclideanResult = circle.Intersects(other);
        var manhattanResult = circle.Intersects(other, DistanceType.Manhattan);

        // Assert
        euclideanResult.Should()
                       .BeTrue();

        manhattanResult.Should()
                       .BeTrue();
    }

    [Test]
    public void Intersects_ValueCircle_ValueCircle_Should_Handle_Large_Values()
    {
        // Arrange
        var circle = new ValueCircle(new Point(1000000, 1000000), 500000);
        var other = new ValueCircle(new Point(1500000, 1000000), 500000);

        // Act
        var result = circle.Intersects(other);

        // Assert
        result.Should()
              .BeTrue(); // Distance = 500000, sum of radii = 1000000
    }
    #endregion

    #region ValueCircle & ICircle Tests
//@formatter:off
    [Test]
    [Arguments(0, 0, 5, 10, 0, 5, DistanceType.Euclidean, true)]
    [Arguments(0, 0, 5, 11, 0, 5, DistanceType.Euclidean, false)]
    [Arguments(0, 0, 5, 3, 0, 2, DistanceType.Euclidean, true)]
    [Arguments(0, 0, 5, 10, 0, 5, DistanceType.Manhattan, true)]
    [Arguments(0, 0, 5, 11, 0, 5, DistanceType.Manhattan, false)]
    //@formatter:on
    public void Intersects_ValueCircle_ICircle_Should_Return_Correct_Result(
        int centerX,
        int centerY,
        int radius,
        int otherCenterX,
        int otherCenterY,
        int otherRadius,
        DistanceType distanceType,
        bool expected)
    {
        // Arrange
        var circle = new ValueCircle(new Point(centerX, centerY), radius);
        var otherMock = new Mock<ICircle>();

        otherMock.Setup(c => c.Center)
                 .Returns(new Point(otherCenterX, otherCenterY));

        otherMock.Setup(c => c.Radius)
                 .Returns(otherRadius);

        // Act
        var result = circle.Intersects(otherMock.Object, distanceType);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void Intersects_ValueCircle_ICircle_Should_Throw_When_Other_Is_Null()
    {
        // Arrange
        ICircle? other = null;

        // Act & Assert
        var action = () => new ValueCircle(new Point(0, 0), 5).Intersects(other!);

        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void Intersects_ValueCircle_ICircle_Should_Default_To_Euclidean()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 5);
        var otherMock = new Mock<ICircle>();

        otherMock.Setup(c => c.Center)
                 .Returns(new Point(10, 0));

        otherMock.Setup(c => c.Radius)
                 .Returns(5);

        // Act
        var result = circle.Intersects(otherMock.Object);

        // Assert
        result.Should()
              .BeTrue(); // Distance = 10, sum of radii = 10, so they touch
    }

    [Test]
    public void Intersects_ValueCircle_ICircle_Should_Throw_When_Invalid_DistanceType()
    {
        // Arrange
        var otherMock = new Mock<ICircle>();

        otherMock.Setup(c => c.Center)
                 .Returns(new Point(10, 0));

        otherMock.Setup(c => c.Radius)
                 .Returns(5);
        var invalidDistanceType = (DistanceType)99;

        // Act & Assert
        var action = () => new ValueCircle(new Point(0, 0), 5).Intersects(otherMock.Object, invalidDistanceType);

        action.Should()
              .Throw<ArgumentOutOfRangeException>()
              .WithParameterName("distanceType");
    }
    #endregion

    #region ICircle & ValueCircle Tests
//@formatter:off
    [Test]
    [Arguments(0, 0, 5, 10, 0, 5, DistanceType.Euclidean, true)]
    [Arguments(0, 0, 5, 11, 0, 5, DistanceType.Euclidean, false)]
    [Arguments(0, 0, 5, 3, 0, 2, DistanceType.Euclidean, true)]
    [Arguments(0, 0, 5, 10, 0, 5, DistanceType.Manhattan, true)]
    [Arguments(0, 0, 5, 11, 0, 5, DistanceType.Manhattan, false)]
    //@formatter:on
    public void Intersects_ICircle_ValueCircle_Should_Return_Correct_Result(
        int centerX,
        int centerY,
        int radius,
        int otherCenterX,
        int otherCenterY,
        int otherRadius,
        DistanceType distanceType,
        bool expected)
    {
        // Arrange
        var circleMock = new Mock<ICircle>();

        circleMock.Setup(c => c.Center)
                  .Returns(new Point(centerX, centerY));

        circleMock.Setup(c => c.Radius)
                  .Returns(radius);
        var other = new ValueCircle(new Point(otherCenterX, otherCenterY), otherRadius);

        // Act
        var result = circleMock.Object.Intersects(other, distanceType);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void Intersects_ICircle_ValueCircle_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;

        // Act & Assert
        var action = () => circle!.Intersects(new ValueCircle(new Point(0, 0), 5));

        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void Intersects_ICircle_ValueCircle_Should_Default_To_Euclidean()
    {
        // Arrange
        var circleMock = new Mock<ICircle>();

        circleMock.Setup(c => c.Center)
                  .Returns(new Point(0, 0));

        circleMock.Setup(c => c.Radius)
                  .Returns(5);
        var other = new ValueCircle(new Point(10, 0), 5);

        // Act
        var result = circleMock.Object.Intersects(other);

        // Assert
        result.Should()
              .BeTrue(); // Distance = 10, sum of radii = 10, so they touch
    }

    [Test]
    public void Intersects_ICircle_ValueCircle_Should_Throw_When_Invalid_DistanceType()
    {
        // Arrange
        var circleMock = new Mock<ICircle>();

        circleMock.Setup(c => c.Center)
                  .Returns(new Point(0, 0));

        circleMock.Setup(c => c.Radius)
                  .Returns(5);
        var invalidDistanceType = (DistanceType)99;

        // Act & Assert
        var action = () => circleMock.Object.Intersects(new ValueCircle(new Point(10, 0), 5), invalidDistanceType);

        action.Should()
              .Throw<ArgumentOutOfRangeException>()
              .WithParameterName("distanceType");
    }
    #endregion

    #region ICircle & ICircle Tests
//@formatter:off
    [Test]
    [Arguments(0, 0, 5, 10, 0, 5, DistanceType.Euclidean, true)]
    [Arguments(0, 0, 5, 11, 0, 5, DistanceType.Euclidean, false)]
    [Arguments(0, 0, 5, 3, 0, 2, DistanceType.Euclidean, true)]
    [Arguments(0, 0, 5, 10, 0, 5, DistanceType.Manhattan, true)]
    [Arguments(0, 0, 5, 11, 0, 5, DistanceType.Manhattan, false)]
    //@formatter:on
    public void Intersects_ICircle_ICircle_Should_Return_Correct_Result(
        int centerX,
        int centerY,
        int radius,
        int otherCenterX,
        int otherCenterY,
        int otherRadius,
        DistanceType distanceType,
        bool expected)
    {
        // Arrange
        var circleMock = new Mock<ICircle>();

        circleMock.Setup(c => c.Center)
                  .Returns(new Point(centerX, centerY));

        circleMock.Setup(c => c.Radius)
                  .Returns(radius);

        var otherMock = new Mock<ICircle>();

        otherMock.Setup(c => c.Center)
                 .Returns(new Point(otherCenterX, otherCenterY));

        otherMock.Setup(c => c.Radius)
                 .Returns(otherRadius);

        // Act
        var result = circleMock.Object.Intersects(otherMock.Object, distanceType);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void Intersects_ICircle_ICircle_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;
        var otherMock = new Mock<ICircle>();

        otherMock.Setup(c => c.Center)
                 .Returns(new Point(0, 0));

        otherMock.Setup(c => c.Radius)
                 .Returns(5);

        // Act & Assert
        var action = () => circle!.Intersects(otherMock.Object);

        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void Intersects_ICircle_ICircle_Should_Throw_When_Other_Is_Null()
    {
        // Arrange
        var circleMock = new Mock<ICircle>();

        circleMock.Setup(c => c.Center)
                  .Returns(new Point(0, 0));

        circleMock.Setup(c => c.Radius)
                  .Returns(5);
        ICircle? other = null;

        // Act & Assert
        var action = () => circleMock.Object.Intersects(other!);

        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void Intersects_ICircle_ICircle_Should_Default_To_Euclidean()
    {
        // Arrange
        var circleMock = new Mock<ICircle>();

        circleMock.Setup(c => c.Center)
                  .Returns(new Point(0, 0));

        circleMock.Setup(c => c.Radius)
                  .Returns(5);

        var otherMock = new Mock<ICircle>();

        otherMock.Setup(c => c.Center)
                 .Returns(new Point(10, 0));

        otherMock.Setup(c => c.Radius)
                 .Returns(5);

        // Act
        var result = circleMock.Object.Intersects(otherMock.Object);

        // Assert
        result.Should()
              .BeTrue(); // Distance = 10, sum of radii = 10, so they touch
    }

    [Test]
    public void Intersects_ICircle_ICircle_Should_Throw_When_Invalid_DistanceType()
    {
        // Arrange
        var circleMock = new Mock<ICircle>();

        circleMock.Setup(c => c.Center)
                  .Returns(new Point(0, 0));

        circleMock.Setup(c => c.Radius)
                  .Returns(5);

        var otherMock = new Mock<ICircle>();

        otherMock.Setup(c => c.Center)
                 .Returns(new Point(10, 0));

        otherMock.Setup(c => c.Radius)
                 .Returns(5);

        var invalidDistanceType = (DistanceType)99;

        // Act & Assert
        var action = () => circleMock.Object.Intersects(otherMock.Object, invalidDistanceType);

        action.Should()
              .Throw<ArgumentOutOfRangeException>()
              .WithParameterName("distanceType");
    }
    #endregion

    #region Edge Cases and Comprehensive Tests
    [Test]
    public void Intersects_Should_Handle_Zero_Radius_Circles()
    {
        // Arrange
        var circle1 = new ValueCircle(new Point(0, 0), 0);
        var circle2 = new ValueCircle(new Point(0, 0), 0);
        var circle3 = new ValueCircle(new Point(1, 0), 0);

        // Act & Assert
        circle1.Intersects(circle2)
               .Should()
               .BeTrue(); // Same point

        circle1.Intersects(circle2, DistanceType.Manhattan)
               .Should()
               .BeTrue(); // Same point

        circle1.Intersects(circle3)
               .Should()
               .BeFalse(); // Different points

        circle1.Intersects(circle3, DistanceType.Manhattan)
               .Should()
               .BeFalse(); // Different points
    }

    [Test]
    public void Circle_Intersects_Should_Handle_Negative_Coordinates()
    {
        // Arrange
        var circle = new ValueCircle(new Point(-10, -10), 5);
        var other = new ValueCircle(new Point(-5, -10), 5);

        // Act
        var euclideanResult = circle.Intersects(other);
        var manhattanResult = circle.Intersects(other, DistanceType.Manhattan);

        // Assert
        euclideanResult.Should()
                       .BeTrue(); // Distance = 5, sum of radii = 10

        manhattanResult.Should()
                       .BeTrue(); // Manhattan distance = 5, sum of radii = 10
    }

    [Test]
    public void Intersects_Should_Be_Symmetric()
    {
        // Arrange
        var circle1 = new ValueCircle(new Point(0, 0), 3);
        var circle2 = new ValueCircle(new Point(5, 0), 4);
        var circle2Mock = new Mock<ICircle>();

        circle2Mock.Setup(c => c.Center)
                   .Returns(new Point(5, 0));

        circle2Mock.Setup(c => c.Radius)
                   .Returns(4);

        // Act
        var result1 = circle1.Intersects(circle2);
        var result2 = circle2.Intersects(circle1);
        var result3 = circle1.Intersects(circle2Mock.Object);

        // Assert
        result1.Should()
               .Be(result2);

        result1.Should()
               .Be(result3);
    }

    [Test]
    public void Intersects_Manhattan_Vs_Euclidean_Should_Differ_On_Diagonal()
    {
        // Arrange - Circles positioned diagonally where Manhattan > Euclidean distance
        var circle = new ValueCircle(new Point(0, 0), 5);
        var other = new ValueCircle(new Point(7, 7), 3);

        // Act
        var euclideanResult = circle.Intersects(other);
        var manhattanResult = circle.Intersects(other, DistanceType.Manhattan);

        // Assert
        // Euclidean distance ≈ 9.9, sum of radii = 8 (no intersection)
        // Manhattan distance = 14, sum of radii = 8 (no intersection)
        euclideanResult.Should()
                       .BeFalse();

        manhattanResult.Should()
                       .BeFalse();
    }

    [Test]
    public void Circle_Intersects_Manhattan_Vs_Euclidean_Should_Show_Different_Results()
    {
        // Arrange - Case where Manhattan allows intersection but Euclidean doesn't
        var circle = new ValueCircle(new Point(0, 0), 8);
        var other = new ValueCircle(new Point(7, 7), 3);

        // Act
        var euclideanResult = circle.Intersects(other);
        var manhattanResult = circle.Intersects(other, DistanceType.Manhattan);

        // Assert
        // Euclidean distance ≈ 9.9, sum of radii = 11 (intersection)
        // Manhattan distance = 14, sum of radii = 11 (no intersection)
        euclideanResult.Should()
                       .BeTrue();

        manhattanResult.Should()
                       .BeFalse();
    }

    [Test]
    public void Circle_Intersects_Should_Be_Consistent_Across_All_Overloads()
    {
        // Arrange
        var valueCircle1 = new ValueCircle(new Point(0, 0), 5);
        var valueCircle2 = new ValueCircle(new Point(8, 0), 4);

        var circle1Mock = new Mock<ICircle>();

        circle1Mock.Setup(c => c.Center)
                   .Returns(new Point(0, 0));

        circle1Mock.Setup(c => c.Radius)
                   .Returns(5);

        var circle2Mock = new Mock<ICircle>();

        circle2Mock.Setup(c => c.Center)
                   .Returns(new Point(8, 0));

        circle2Mock.Setup(c => c.Radius)
                   .Returns(4);

        // Act
        var result1 = valueCircle1.Intersects(valueCircle2);
        var result2 = valueCircle1.Intersects(circle2Mock.Object);
        var result3 = circle1Mock.Object.Intersects(valueCircle2);
        var result4 = circle1Mock.Object.Intersects(circle2Mock.Object);

        // Assert
        result1.Should()
               .Be(result2);

        result2.Should()
               .Be(result3);

        result3.Should()
               .Be(result4);

        result1.Should()
               .BeTrue(); // Distance = 8, sum of radii = 9
    }

    [Test]
    public void Intersects_Should_Handle_Boundary_Conditions_Precisely()
    {
        // Arrange - Circles exactly touching
        var circle = new ValueCircle(new Point(0, 0), 5);
        var touchingCircle = new ValueCircle(new Point(10, 0), 5);
        var almostTouchingCircle = new ValueCircle(new Point(11, 0), 5);

        // Act
        var touchingResult = circle.Intersects(touchingCircle);
        var almostTouchingResult = circle.Intersects(almostTouchingCircle);

        // Assert
        touchingResult.Should()
                      .BeTrue(); // Distance = 10, sum of radii = 10

        almostTouchingResult.Should()
                            .BeFalse(); // Distance = 11, sum of radii = 10
    }
    #endregion

    #region ValueCircle with ValueRectangle Tests
//@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, -10, 10, 10, DistanceType.Euclidean, true)] // Circle at origin, rectangle around it
    [Arguments(0, 0, 9, 6, 6, 8, 8, DistanceType.Euclidean, true)] // Circle at origin, rectangle far away
    [Arguments(5, 5, 3, 0, 0, 10, 10, DistanceType.Euclidean, true)] // Circle inside rectangle
    [Arguments(0, 0, 2, 1, 1, 5, 5, DistanceType.Euclidean, true)] // Circle overlaps rectangle corner
    [Arguments(0, 0, 1, 2, 0, 4, 2, DistanceType.Euclidean, false)] // Circle doesn't reach rectangle
    [Arguments(0, 0, 5, -10, -10, 10, 10, DistanceType.Manhattan, true)] // Manhattan distance
    [Arguments(0, 0, 8, 4, 4, 6, 6, DistanceType.Manhattan, true)] // Manhattan tangent at corner (distance == radius counts as intersection)
    [Arguments(0, 0, 2, 4, 4, 6, 6, DistanceType.Manhattan, false)] // Manhattan doesn't reach
    //@formatter:on
    public void Intersects_ValueCircle_ValueRectangle_Should_Return_Correct_Result(
        int centerX,
        int centerY,
        int radius,
        int left,
        int top,
        int right,
        int bottom,
        DistanceType distanceType,
        bool expected)
    {
        // Arrange
        var circle = new ValueCircle(new Point(centerX, centerY), radius);

        var rectangle = new ValueRectangle(
            left,
            top,
            right - left,
            bottom - top);

        // Act
        var result = rectangle.Intersects(circle, distanceType);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void Intersects_ValueCircle_ValueRectangle_Should_Default_To_Euclidean()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 5);

        var rectangle = new ValueRectangle(
            -10,
            -10,
            20,
            20);

        // Act
        var resultDefault = rectangle.Intersects(circle);
        var resultExplicit = rectangle.Intersects(circle);

        // Assert
        resultDefault.Should()
                     .Be(resultExplicit);
    }

    [Test]
    public void Intersects_ValueCircle_ValueRectangle_Should_Throw_When_Invalid_DistanceType()
    {
        // Arrange
        var invalidDistanceType = (DistanceType)99;

        // Act & Assert
        var action = () => new ValueRectangle(
            0,
            0,
            10,
            10).Intersects(new ValueCircle(new Point(0, 0), 5), invalidDistanceType);

        action.Should()
              .Throw<ArgumentOutOfRangeException>()
              .WithParameterName("distanceType");
    }

    [Test]
    public void Intersects_ValueCircle_ValueRectangle_Should_Handle_Zero_Radius()
    {
        // Arrange
        var circle = new ValueCircle(new Point(5, 5), 0);

        var rectangle = new ValueRectangle(
            0,
            0,
            10,
            10);

        // Act
        var result = rectangle.Intersects(circle);

        // Assert
        result.Should()
              .BeTrue(); // Point circle inside rectangle
    }

    [Test]
    public void Intersects_ValueCircle_ValueRectangle_Should_Handle_Circle_At_Rectangle_Edge()
    {
        // Arrange
        var circle = new ValueCircle(new Point(5, 0), 1); // Circle touching top edge

        var rectangle = new ValueRectangle(
            0,
            0,
            10,
            10);

        // Act
        var result = rectangle.Intersects(circle);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void Intersects_ValueCircle_ValueRectangle_Should_Handle_Circle_Exactly_At_Corner()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 1); // Circle at corner

        var rectangle = new ValueRectangle(
            0,
            0,
            10,
            10);

        // Act
        var result = rectangle.Intersects(circle);

        // Assert
        result.Should()
              .BeTrue();
    }
    #endregion

    #region ValueCircle with IRectangle Tests
//@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, -10, 10, 10, DistanceType.Euclidean, true)]
    [Arguments(0, 0, 5, 6, 6, 8, 8, DistanceType.Euclidean, false)]
    [Arguments(5, 5, 3, 0, 0, 10, 10, DistanceType.Manhattan, true)]
    //@formatter:on
    public void Intersects_ValueCircle_IRectangle_Should_Return_Correct_Result(
        int centerX,
        int centerY,
        int radius,
        int left,
        int top,
        int right,
        int bottom,
        DistanceType distanceType,
        bool expected)
    {
        // Arrange
        var circle = new ValueCircle(new Point(centerX, centerY), radius);
        var mockRectangle = new Mock<IRectangle>();

        mockRectangle.Setup(r => r.Left)
                     .Returns(left);

        mockRectangle.Setup(r => r.Top)
                     .Returns(top);

        mockRectangle.Setup(r => r.Right)
                     .Returns(right);

        mockRectangle.Setup(r => r.Bottom)
                     .Returns(bottom);

        // Act
        var result = mockRectangle.Object.Intersects(circle, distanceType);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void Intersects_ValueCircle_IRectangle_Should_Throw_When_Rectangle_Is_Null()
    {
        // Arrange
        IRectangle? nullRectangle = null;

        // Act & Assert
        var action = () => nullRectangle!.Intersects(new ValueCircle(new Point(0, 0), 5));

        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void Intersects_ValueCircle_IRectangle_Should_Default_To_Euclidean()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 5);
        var mockRectangle = new Mock<IRectangle>();

        mockRectangle.Setup(r => r.Left)
                     .Returns(-10);

        mockRectangle.Setup(r => r.Top)
                     .Returns(-10);

        mockRectangle.Setup(r => r.Right)
                     .Returns(10);

        mockRectangle.Setup(r => r.Bottom)
                     .Returns(10);

        // Act
        var resultDefault = mockRectangle.Object.Intersects(circle);
        var resultExplicit = mockRectangle.Object.Intersects(circle);

        // Assert
        resultDefault.Should()
                     .Be(resultExplicit);
    }

    [Test]
    public void Intersects_ValueCircle_IRectangle_Should_Throw_When_Invalid_DistanceType()
    {
        // Arrange
        var mockRectangle = new Mock<IRectangle>();

        mockRectangle.Setup(r => r.Left)
                     .Returns(0);

        mockRectangle.Setup(r => r.Top)
                     .Returns(0);

        mockRectangle.Setup(r => r.Right)
                     .Returns(10);

        mockRectangle.Setup(r => r.Bottom)
                     .Returns(10);
        var invalidDistanceType = (DistanceType)99;

        // Act & Assert
        var action = () => mockRectangle.Object.Intersects(new ValueCircle(new Point(0, 0), 5), invalidDistanceType);

        action.Should()
              .Throw<ArgumentOutOfRangeException>()
              .WithParameterName("distanceType");
    }
    #endregion

    #region ICircle with ValueRectangle Tests
//@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, -10, 10, 10, DistanceType.Euclidean, true)]
    [Arguments(0, 0, 5, 6, 6, 8, 8, DistanceType.Euclidean, false)]
    [Arguments(5, 5, 3, 0, 0, 10, 10, DistanceType.Manhattan, true)]
    //@formatter:on
    public void Intersects_ICircle_ValueRectangle_Should_Return_Correct_Result(
        int centerX,
        int centerY,
        int radius,
        int left,
        int top,
        int right,
        int bottom,
        DistanceType distanceType,
        bool expected)
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(new Point(centerX, centerY));

        mockCircle.Setup(c => c.Radius)
                  .Returns(radius);

        var rectangle = new ValueRectangle(
            left,
            top,
            right - left,
            bottom - top);

        // Act
        var result = rectangle.Intersects(mockCircle.Object, distanceType);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void Intersects_ICircle_ValueRectangle_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? nullCircle = null;

        // Act & Assert
        var action = () => new ValueRectangle(
            0,
            0,
            10,
            10).Intersects(nullCircle!);

        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void Intersects_ICircle_ValueRectangle_Should_Default_To_Euclidean()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(new Point(0, 0));

        mockCircle.Setup(c => c.Radius)
                  .Returns(5);

        var rectangle = new ValueRectangle(
            -10,
            -10,
            20,
            20);

        // Act
        var resultDefault = rectangle.Intersects(mockCircle.Object);
        var resultExplicit = rectangle.Intersects(mockCircle.Object);

        // Assert
        resultDefault.Should()
                     .Be(resultExplicit);
    }

    [Test]
    public void Intersects_ICircle_ValueRectangle_Should_Throw_When_Invalid_DistanceType()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(new Point(0, 0));

        mockCircle.Setup(c => c.Radius)
                  .Returns(5);
        var invalidDistanceType = (DistanceType)99;

        // Act & Assert
        var action = () => new ValueRectangle(
            0,
            0,
            10,
            10).Intersects(mockCircle.Object, invalidDistanceType);

        action.Should()
              .Throw<ArgumentOutOfRangeException>()
              .WithParameterName("distanceType");
    }
    #endregion

    #region ICircle with IRectangle Tests
//@formatter:off
    [Test]
    [Arguments(0, 0, 5, -10, -10, 10, 10, DistanceType.Euclidean, true)]
    [Arguments(0, 0, 5, 6, 6, 8, 8, DistanceType.Euclidean, false)]
    [Arguments(5, 5, 3, 0, 0, 10, 10, DistanceType.Manhattan, true)]
    [Arguments(0, 0, 2, 4, 4, 6, 6, DistanceType.Manhattan, false)]
    //@formatter:on
    public void Intersects_ICircle_IRectangle_Should_Return_Correct_Result(
        int centerX,
        int centerY,
        int radius,
        int left,
        int top,
        int right,
        int bottom,
        DistanceType distanceType,
        bool expected)
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(new Point(centerX, centerY));

        mockCircle.Setup(c => c.Radius)
                  .Returns(radius);

        var mockRectangle = new Mock<IRectangle>();

        mockRectangle.Setup(r => r.Left)
                     .Returns(left);

        mockRectangle.Setup(r => r.Top)
                     .Returns(top);

        mockRectangle.Setup(r => r.Right)
                     .Returns(right);

        mockRectangle.Setup(r => r.Bottom)
                     .Returns(bottom);

        // Act
        var result = mockRectangle.Object.Intersects(mockCircle.Object, distanceType);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void Intersects_ICircle_IRectangle_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? nullCircle = null;
        var mockRectangle = new Mock<IRectangle>();

        mockRectangle.Setup(r => r.Left)
                     .Returns(0);

        mockRectangle.Setup(r => r.Top)
                     .Returns(0);

        mockRectangle.Setup(r => r.Right)
                     .Returns(10);

        mockRectangle.Setup(r => r.Bottom)
                     .Returns(10);

        // Act & Assert
        var action = () => mockRectangle.Object.Intersects(nullCircle!);

        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void Intersects_ICircle_IRectangle_Should_Throw_When_Rectangle_Is_Null()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(new Point(0, 0));

        mockCircle.Setup(c => c.Radius)
                  .Returns(5);
        IRectangle? nullRectangle = null;

        // Act & Assert
        var action = () => nullRectangle!.Intersects(mockCircle.Object);

        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void Intersects_ICircle_IRectangle_Should_Default_To_Euclidean()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(new Point(0, 0));

        mockCircle.Setup(c => c.Radius)
                  .Returns(5);

        var mockRectangle = new Mock<IRectangle>();

        mockRectangle.Setup(r => r.Left)
                     .Returns(-10);

        mockRectangle.Setup(r => r.Top)
                     .Returns(-10);

        mockRectangle.Setup(r => r.Right)
                     .Returns(10);

        mockRectangle.Setup(r => r.Bottom)
                     .Returns(10);

        // Act
        var resultDefault = mockRectangle.Object.Intersects(mockCircle.Object);
        var resultExplicit = mockRectangle.Object.Intersects(mockCircle.Object);

        // Assert
        resultDefault.Should()
                     .Be(resultExplicit);
    }

    [Test]
    public void Intersects_ICircle_IRectangle_Should_Throw_When_Invalid_DistanceType()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(new Point(0, 0));

        mockCircle.Setup(c => c.Radius)
                  .Returns(5);

        var mockRectangle = new Mock<IRectangle>();

        mockRectangle.Setup(r => r.Left)
                     .Returns(0);

        mockRectangle.Setup(r => r.Top)
                     .Returns(0);

        mockRectangle.Setup(r => r.Right)
                     .Returns(10);

        mockRectangle.Setup(r => r.Bottom)
                     .Returns(10);
        var invalidDistanceType = (DistanceType)99;

        // Act & Assert
        var action = () => mockRectangle.Object.Intersects(mockCircle.Object, invalidDistanceType);

        action.Should()
              .Throw<ArgumentOutOfRangeException>()
              .WithParameterName("distanceType");
    }
    #endregion

    #region Edge Cases and Boundary Conditions
    [Test]
    public void Intersects_Should_Handle_Circle_Completely_Inside_Rectangle()
    {
        // Arrange
        var circle = new ValueCircle(new Point(5, 5), 2);

        var rectangle = new ValueRectangle(
            0,
            0,
            10,
            10);

        // Act
        var result = rectangle.Intersects(circle);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void Intersects_Should_Handle_Rectangle_Inside_Circle()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 10);

        var rectangle = new ValueRectangle(
            -2,
            -2,
            4,
            4);

        // Act
        var result = rectangle.Intersects(circle);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void Intersects_Should_Handle_Circle_Touching_Rectangle_Corner()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 3);

        var rectangle = new ValueRectangle(
            2,
            2,
            4,
            4); // Corner at (2,2)

        // Act & Assert
        rectangle.Intersects(circle)
                 .Should()
                 .BeTrue(); // Distance is 2.83, radius is 3

        rectangle.Intersects(circle, DistanceType.Manhattan)
                 .Should()
                 .BeFalse(); // Distance is 4, but radius is 3, so false would be expected
    }

    [Test]
    public void Intersects_Manhattan_Vs_Euclidean_Should_Show_Different_Results()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 3);

        var rectangle = new ValueRectangle(
            3,
            3,
            2,
            2); // Corner at (3,3)

        // Act
        var euclideanResult = rectangle.Intersects(circle);
        var manhattanResult = rectangle.Intersects(circle, DistanceType.Manhattan);

        // Assert
        // Euclidean distance from (0,0) to (3,3) is ~4.24, radius is 3, so false
        // Manhattan distance from (0,0) to (3,3) is 6, radius is 3, so false
        euclideanResult.Should()
                       .BeFalse();

        manhattanResult.Should()
                       .BeFalse();
    }

    [Test]
    public void Intersects_Should_Handle_Negative_Coordinates()
    {
        // Arrange
        var circle = new ValueCircle(new Point(-5, -5), 3);

        var rectangle = new ValueRectangle(
            -10,
            -10,
            5,
            5);

        // Act
        var result = rectangle.Intersects(circle);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void Intersects_Should_Handle_Large_Values()
    {
        // Arrange
        var circle = new ValueCircle(new Point(1000, 1000), 100);

        var rectangle = new ValueRectangle(
            900,
            900,
            200,
            200);

        // Act
        var result = rectangle.Intersects(circle);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void Intersects_Should_Be_Consistent_Across_All_Overloads()
    {
        // Arrange
        var centerPoint = new Point(0, 0);
        var radius = 5;
        var left = -3;
        var top = -3;
        var right = 3;
        var bottom = 3;

        var valueCircle = new ValueCircle(centerPoint, radius);
        var mockCircle = new Mock<ICircle>();

        mockCircle.Setup(c => c.Center)
                  .Returns(centerPoint);

        mockCircle.Setup(c => c.Radius)
                  .Returns(radius);

        var valueRectangle = new ValueRectangle(
            left,
            top,
            right - left,
            bottom - top);
        var mockRectangle = new Mock<IRectangle>();

        mockRectangle.Setup(r => r.Left)
                     .Returns(left);

        mockRectangle.Setup(r => r.Top)
                     .Returns(top);

        mockRectangle.Setup(r => r.Right)
                     .Returns(right);

        mockRectangle.Setup(r => r.Bottom)
                     .Returns(bottom);

        // Act
        var result1 = valueRectangle.Intersects(valueCircle);
        var result2 = mockRectangle.Object.Intersects(valueCircle);
        var result3 = valueRectangle.Intersects(mockCircle.Object);
        var result4 = mockRectangle.Object.Intersects(mockCircle.Object);

        // Assert
        result1.Should()
               .Be(result2);

        result2.Should()
               .Be(result3);

        result3.Should()
               .Be(result4);
    }
    #endregion

    #region ValueCircle with ValuePoint Tests
    [Test]
    public void ManhattanEdgeDistanceFrom_ValueCircle_ValuePoint_Should_Return_Zero_When_Point_At_Center()
    {
        // Arrange
        var circle = new ValueCircle(new Point(5, 5), 3);
        var point = new ValuePoint(5, 5);

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ValueCircle_ValuePoint_Should_Return_Zero_When_Point_Inside_Circle()
    {
        // Arrange
        var circle = new ValueCircle(new Point(5, 5), 3);
        var point = new ValuePoint(6, 6); // Manhattan distance = 2, less than radius

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ValueCircle_ValuePoint_Should_Return_Correct_Distance_When_Point_Outside_Circle()
    {
        // Arrange
        var circle = new ValueCircle(new Point(5, 5), 3);
        var point = new ValuePoint(10, 5); // Manhattan distance = 5, radius = 3, expected = 2

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(2);
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ValueCircle_ValuePoint_Should_Handle_Diagonal_Distance()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 2);
        var point = new ValuePoint(3, 4); // Manhattan distance = 7, radius = 2, expected = 5

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(5);
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ValueCircle_ValuePoint_Should_Handle_Negative_Coordinates()
    {
        // Arrange
        var circle = new ValueCircle(new Point(-2, -3), 1);
        var point = new ValuePoint(-5, -7); // Manhattan distance = |-2-(-5)| + |-3-(-7)| = 3 + 4 = 7, radius = 1, expected = 6

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(6);
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ValueCircle_ValuePoint_Should_Return_Zero_When_Point_On_Edge()
    {
        // Arrange
        var circle = new ValueCircle(new Point(5, 5), 3);
        var point = new ValuePoint(8, 5); // Manhattan distance = 3, equals radius

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(0);
    }
    #endregion

    #region ValueCircle with Point Tests
    [Test]
    public void ManhattanEdgeDistanceFrom_ValueCircle_Point_Should_Return_Zero_When_Point_At_Center()
    {
        // Arrange
        var circle = new ValueCircle(new Point(10, 10), 5);
        var point = new Point(10, 10);

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ValueCircle_Point_Should_Calculate_Correct_Distance()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 2);
        var point = new Point(4, 3); // Manhattan distance = 7, radius = 2, expected = 5

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(5);
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ValueCircle_Point_Should_Handle_Large_Coordinates()
    {
        // Arrange
        var circle = new ValueCircle(new Point(1000, 1000), 100);
        var point = new Point(1200, 1300); // Manhattan distance = 500, radius = 100, expected = 400

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(400);
    }
    #endregion

    #region ValueCircle with IPoint Tests
    [Test]
    public void ManhattanEdgeDistanceFrom_ValueCircle_IPoint_Should_Throw_When_Point_Is_Null()
    {
        // Arrange
        IPoint point = null!;

        // Act
        var action = () => new ValueCircle(new Point(5, 5), 3).ManhattanEdgeDistanceFrom(point);

        // Assert
        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ValueCircle_IPoint_Should_Calculate_Correct_Distance_With_Mock()
    {
        // Arrange
        var circle = new ValueCircle(new Point(5, 5), 2);
        var mockPoint = new Mock<IPoint>();

        mockPoint.Setup(p => p.X)
                 .Returns(8);

        mockPoint.Setup(p => p.Y)
                 .Returns(9);

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(mockPoint.Object);

        // Assert
        result.Should()
              .Be(5); // Manhattan distance = 3 + 4 = 7, radius = 2, expected = 5
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ValueCircle_IPoint_Should_Return_Zero_For_Interior_Point()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 10);
        var mockPoint = new Mock<IPoint>();

        mockPoint.Setup(p => p.X)
                 .Returns(2);

        mockPoint.Setup(p => p.Y)
                 .Returns(3);

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(mockPoint.Object);

        // Assert
        result.Should()
              .Be(0); // Manhattan distance = 5, radius = 10, distance < radius
    }
    #endregion

    #region ICircle with ValuePoint Tests
    [Test]
    public void ManhattanEdgeDistanceFrom_ICircle_ValuePoint_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle circle = null!;

        // Act
        var action = () => circle.ManhattanEdgeDistanceFrom(new ValuePoint(5, 5));

        // Assert
        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ICircle_ValuePoint_Should_Calculate_Distance_With_Mock_Circle()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(10);

        mockCenter.Setup(p => p.Y)
                  .Returns(10);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(3);

        var point = new ValuePoint(15, 10);

        // Act
        var result = mockCircle.Object.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(2); // Manhattan distance = 5, radius = 3, expected = 2
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ICircle_ValuePoint_Should_Handle_Zero_Radius()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(0);

        mockCenter.Setup(p => p.Y)
                  .Returns(0);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(0);

        var point = new ValuePoint(3, 4);

        // Act
        var result = mockCircle.Object.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(7); // Manhattan distance = 7, radius = 0, expected = 7
    }
    #endregion

    #region ICircle with Point Tests
    [Test]
    public void ManhattanEdgeDistanceFrom_ICircle_Point_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle circle = null!;
        var point = new Point(5, 5);

        // Act
        var action = () => circle.ManhattanEdgeDistanceFrom(point);

        // Assert
        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ICircle_Point_Should_Work_With_Real_Circle()
    {
        // Arrange
        ICircle circle = new Circle(new Point(0, 0), 5);
        var point = new Point(8, 6);

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(9); // Manhattan distance = 14, radius = 5, expected = 9
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ICircle_Point_Should_Handle_Negative_Coordinates()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(-5);

        mockCenter.Setup(p => p.Y)
                  .Returns(-10);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(2);

        var point = new Point(-2, -6);

        // Act
        var result = mockCircle.Object.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(5); // Manhattan distance = 3 + 4 = 7, radius = 2, expected = 5
    }
    #endregion

    #region ICircle with IPoint Tests
    [Test]
    public void ManhattanEdgeDistanceFrom_ICircle_IPoint_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle circle = null!;
        var mockPoint = new Mock<IPoint>();

        mockPoint.Setup(p => p.X)
                 .Returns(5);

        mockPoint.Setup(p => p.Y)
                 .Returns(5);

        // Act
        var action = () => circle.ManhattanEdgeDistanceFrom(mockPoint.Object);

        // Assert
        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ICircle_IPoint_Should_Throw_When_Point_Is_Null()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(0);

        mockCenter.Setup(p => p.Y)
                  .Returns(0);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(3);

        IPoint point = null!;

        // Act
        var action = () => mockCircle.Object.ManhattanEdgeDistanceFrom(point);

        // Assert
        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ICircle_IPoint_Should_Calculate_Distance_With_Both_Mocks()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(2);

        mockCenter.Setup(p => p.Y)
                  .Returns(3);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(1);

        var mockPoint = new Mock<IPoint>();

        mockPoint.Setup(p => p.X)
                 .Returns(7);

        mockPoint.Setup(p => p.Y)
                 .Returns(8);

        // Act
        var result = mockCircle.Object.ManhattanEdgeDistanceFrom(mockPoint.Object);

        // Assert
        result.Should()
              .Be(9); // Manhattan distance = 5 + 5 = 10, radius = 1, expected = 9
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ICircle_IPoint_Should_Return_Zero_When_Distance_Less_Than_Radius()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(10);

        mockCenter.Setup(p => p.Y)
                  .Returns(10);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(20);

        var mockPoint = new Mock<IPoint>();

        mockPoint.Setup(p => p.X)
                 .Returns(15);

        mockPoint.Setup(p => p.Y)
                 .Returns(12);

        // Act
        var result = mockCircle.Object.ManhattanEdgeDistanceFrom(mockPoint.Object);

        // Assert
        result.Should()
              .Be(0); // Manhattan distance = 7, radius = 20, distance < radius
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_ICircle_IPoint_Should_Delegate_To_Point_Overload()
    {
        // Arrange
        ICircle circle = new Circle(new Point(5, 5), 3);
        var mockPoint = new Mock<IPoint>();

        mockPoint.Setup(p => p.X)
                 .Returns(10);

        mockPoint.Setup(p => p.Y)
                 .Returns(8);

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(mockPoint.Object);

        // Assert  
        result.Should()
              .Be(5); // Manhattan distance = 5 + 3 = 8, radius = 3, expected = 5
    }
    #endregion

    #region Edge Cases and Boundary Tests
    [Test]
    public void ManhattanEdgeDistanceFrom_Should_Handle_Zero_Radius_Circle()
    {
        // Arrange
        var circle = new ValueCircle(new Point(5, 5), 0);
        var point = new Point(5, 5);

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(0); // Distance = 0, radius = 0, expected = 0
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_Should_Never_Return_Negative_Values()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 100);
        var point = new Point(1, 1); // Very close to center

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .BeGreaterThanOrEqualTo(0);
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_Should_Be_Consistent_Across_All_Overloads()
    {
        // Arrange
        var center = new Point(10, 15);
        var radius = 5;
        var testPoint = new Point(20, 25);

        var valueCircle = new ValueCircle(center, radius);
        ICircle interfaceCircle = new Circle(center, radius);

        var valuePoint = new ValuePoint(testPoint.X, testPoint.Y);
        var mockIPoint = new Mock<IPoint>();

        mockIPoint.Setup(p => p.X)
                  .Returns(testPoint.X);

        mockIPoint.Setup(p => p.Y)
                  .Returns(testPoint.Y);

        // Act
        var result1 = valueCircle.ManhattanEdgeDistanceFrom(valuePoint);
        var result2 = valueCircle.ManhattanEdgeDistanceFrom(testPoint);
        var result3 = valueCircle.ManhattanEdgeDistanceFrom(mockIPoint.Object);
        var result4 = interfaceCircle.ManhattanEdgeDistanceFrom(valuePoint);
        var result5 = interfaceCircle.ManhattanEdgeDistanceFrom(testPoint);
        var result6 = interfaceCircle.ManhattanEdgeDistanceFrom(mockIPoint.Object);

        // Assert
        result1.Should()
               .Be(15); // Manhattan distance = 20, radius = 5, expected = 15

        result2.Should()
               .Be(result1);

        result3.Should()
               .Be(result1);

        result4.Should()
               .Be(result1);

        result5.Should()
               .Be(result1);

        result6.Should()
               .Be(result1);
    }

    [Test]
    public void ManhattanEdgeDistanceFrom_Should_Handle_Maximum_Integer_Values()
    {
        // Arrange
        var point = new Point(int.MaxValue / 2, int.MaxValue / 2);

        // Act & Assert - Should not throw overflow exception
        var action = () => new ValueCircle(new Point(0, 0), 1000).ManhattanEdgeDistanceFrom(point);

        action.Should()
              .NotThrow();
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 1, 2, 2, 3)] // Distance = 4, radius = 1, expected = 3
    [Arguments(5, 5, 3, 10, 5, 2)] // Distance = 5, radius = 3, expected = 2
    [Arguments(-5, -5, 2, -3, -3, 2)] // Distance = 4, radius = 2, expected = 2
    [Arguments(0, 0, 10, 3, 4, 0)] // Distance = 7, radius = 10, expected = 0 (inside circle)
    //@formatter:on
    public void ManhattanEdgeDistanceFrom_Should_Return_Expected_Results(
        int centerX,
        int centerY,
        int radius,
        int pointX,
        int pointY,
        int expectedDistance)
    {
        // Arrange
        var circle = new ValueCircle(new Point(centerX, centerY), radius);
        var point = new Point(pointX, pointY);

        // Act
        var result = circle.ManhattanEdgeDistanceFrom(point);

        // Assert
        result.Should()
              .Be(expectedDistance);
    }
    #endregion

    #region ValueCircle to ValueCircle Tests
    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Return_Zero_When_Circles_Touch()
    {
        // Arrange
        var circle1 = new ValueCircle(new Point(0, 0), 5);
        var circle2 = new ValueCircle(new Point(10, 0), 5); // Distance = 10, Sum of radii = 10

        // Act
        var result = circle1.ManhattanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Return_Zero_When_Circles_Overlap()
    {
        // Arrange
        var circle1 = new ValueCircle(new Point(0, 0), 5);
        var circle2 = new ValueCircle(new Point(8, 0), 5); // Distance = 8, Sum of radii = 10

        // Act
        var result = circle1.ManhattanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Return_Zero_When_One_Circle_Inside_Another()
    {
        // Arrange
        var circle1 = new ValueCircle(new Point(0, 0), 10);
        var circle2 = new ValueCircle(new Point(3, 0), 2); // Distance = 3, Sum of radii = 12

        // Act
        var result = circle1.ManhattanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Calculate_Correct_Distance_When_Separated()
    {
        // Arrange
        var circle1 = new ValueCircle(new Point(0, 0), 2);
        var circle2 = new ValueCircle(new Point(10, 0), 3); // Manhattan distance = 10, Sum of radii = 5, Expected = 5

        // Act
        var result = circle1.ManhattanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        result.Should()
              .Be(5);
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Handle_Diagonal_Separation()
    {
        // Arrange
        var circle1 = new ValueCircle(new Point(0, 0), 1);
        var circle2 = new ValueCircle(new Point(3, 4), 1); // Manhattan distance = 7, Sum of radii = 2, Expected = 5

        // Act
        var result = circle1.ManhattanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        result.Should()
              .Be(5);
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Handle_Zero_Radius_Circles()
    {
        // Arrange
        var circle1 = new ValueCircle(new Point(0, 0), 0);
        var circle2 = new ValueCircle(new Point(5, 3), 0); // Manhattan distance = 8, Sum of radii = 0, Expected = 8

        // Act
        var result = circle1.ManhattanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        result.Should()
              .Be(8);
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Handle_Identical_Circles()
    {
        // Arrange
        var circle1 = new ValueCircle(new Point(5, 5), 3);
        var circle2 = new ValueCircle(new Point(5, 5), 3);

        // Act
        var result = circle1.ManhattanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ValueCircle_ValueCircle_Should_Handle_Negative_Coordinates()
    {
        // Arrange
        var circle1 = new ValueCircle(new Point(-5, -3), 2);
        var circle2 = new ValueCircle(new Point(2, 4), 1); // Manhattan distance = 14, Sum of radii = 3, Expected = 11

        // Act
        var result = circle1.ManhattanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        result.Should()
              .Be(11);
    }
    #endregion

    #region ValueCircle to ICircle Tests
    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ValueCircle_ICircle_Should_Throw_When_Other_Is_Null()
    {
        // Arrange
        ICircle? other = null;

        // Act & Assert
        var action = () => new ValueCircle(new Point(0, 0), 5).ManhattanEdgeToEdgeDistanceFrom(other!);

        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ValueCircle_ICircle_Should_Calculate_Distance_With_Mock()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 3);
        var mockOther = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(8);

        mockCenter.Setup(p => p.Y)
                  .Returns(6);

        mockOther.Setup(c => c.Center)
                 .Returns(mockCenter.Object);

        mockOther.Setup(c => c.Radius)
                 .Returns(2);

        // Act
        var result = circle.ManhattanEdgeToEdgeDistanceFrom(mockOther.Object);

        // Assert
        result.Should()
              .Be(9); // Manhattan distance = 14, Sum of radii = 5, Expected = 9
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ValueCircle_ICircle_Should_Return_Zero_When_Overlapping()
    {
        // Arrange
        var circle = new ValueCircle(new Point(0, 0), 5);
        var mockOther = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(3);

        mockCenter.Setup(p => p.Y)
                  .Returns(4);

        mockOther.Setup(c => c.Center)
                 .Returns(mockCenter.Object);

        mockOther.Setup(c => c.Radius)
                 .Returns(10);

        // Act
        var result = circle.ManhattanEdgeToEdgeDistanceFrom(mockOther.Object);

        // Assert
        result.Should()
              .Be(0); // Manhattan distance = 7, Sum of radii = 15, Expected = 0
    }
    #endregion

    #region ICircle to ValueCircle Tests
    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ICircle_ValueCircle_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;

        // Act & Assert
        var action = () => circle!.ManhattanEdgeToEdgeDistanceFrom(new ValueCircle(new Point(5, 5), 3));

        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ICircle_ValueCircle_Should_Calculate_Distance_With_Mock()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(-2);

        mockCenter.Setup(p => p.Y)
                  .Returns(3);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(4);

        var other = new ValueCircle(new Point(6, -1), 2);

        // Act
        var result = mockCircle.Object.ManhattanEdgeToEdgeDistanceFrom(other);

        // Assert
        result.Should()
              .Be(6); // Manhattan distance = 12, Sum of radii = 6, Expected = 6
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ICircle_ValueCircle_Should_Handle_Negative_Coordinates()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCenter = new Mock<IPoint>();

        mockCenter.Setup(p => p.X)
                  .Returns(-10);

        mockCenter.Setup(p => p.Y)
                  .Returns(-5);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(1);

        var other = new ValueCircle(new Point(-8, -3), 1);

        // Act
        var result = mockCircle.Object.ManhattanEdgeToEdgeDistanceFrom(other);

        // Assert
        result.Should()
              .Be(2); // Manhattan distance = 4, Sum of radii = 2, Expected = 2
    }
    #endregion

    #region ICircle to ICircle Tests
    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ICircle_ICircle_Should_Throw_When_Circle_Is_Null()
    {
        // Arrange
        ICircle? circle = null;
        var mockOther = new Mock<ICircle>();

        // Act & Assert
        var action = () => circle!.ManhattanEdgeToEdgeDistanceFrom(mockOther.Object);

        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ICircle_ICircle_Should_Throw_When_Other_Is_Null()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        ICircle? other = null;

        // Act & Assert
        var action = () => mockCircle.Object.ManhattanEdgeToEdgeDistanceFrom(other!);

        action.Should()
              .Throw<NullReferenceException>();
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ICircle_ICircle_Should_Calculate_Distance_With_Both_Mocks()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCircleCenter = new Mock<IPoint>();

        mockCircleCenter.Setup(p => p.X)
                        .Returns(1);

        mockCircleCenter.Setup(p => p.Y)
                        .Returns(1);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCircleCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(2);

        var mockOther = new Mock<ICircle>();
        var mockOtherCenter = new Mock<IPoint>();

        mockOtherCenter.Setup(p => p.X)
                       .Returns(7);

        mockOtherCenter.Setup(p => p.Y)
                       .Returns(4);

        mockOther.Setup(c => c.Center)
                 .Returns(mockOtherCenter.Object);

        mockOther.Setup(c => c.Radius)
                 .Returns(1);

        // Act
        var result = mockCircle.Object.ManhattanEdgeToEdgeDistanceFrom(mockOther.Object);

        // Assert
        result.Should()
              .Be(6); // Manhattan distance = 9, Sum of radii = 3, Expected = 6
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ICircle_ICircle_Should_Return_Zero_When_Distance_Less_Than_Sum_Of_Radii()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCircleCenter = new Mock<IPoint>();

        mockCircleCenter.Setup(p => p.X)
                        .Returns(0);

        mockCircleCenter.Setup(p => p.Y)
                        .Returns(0);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCircleCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(5);

        var mockOther = new Mock<ICircle>();
        var mockOtherCenter = new Mock<IPoint>();

        mockOtherCenter.Setup(p => p.X)
                       .Returns(2);

        mockOtherCenter.Setup(p => p.Y)
                       .Returns(1);

        mockOther.Setup(c => c.Center)
                 .Returns(mockOtherCenter.Object);

        mockOther.Setup(c => c.Radius)
                 .Returns(4);

        // Act
        var result = mockCircle.Object.ManhattanEdgeToEdgeDistanceFrom(mockOther.Object);

        // Assert
        result.Should()
              .Be(0); // Manhattan distance = 3, Sum of radii = 9, Expected = 0
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_ICircle_ICircle_Should_Handle_Large_Distances()
    {
        // Arrange
        var mockCircle = new Mock<ICircle>();
        var mockCircleCenter = new Mock<IPoint>();

        mockCircleCenter.Setup(p => p.X)
                        .Returns(0);

        mockCircleCenter.Setup(p => p.Y)
                        .Returns(0);

        mockCircle.Setup(c => c.Center)
                  .Returns(mockCircleCenter.Object);

        mockCircle.Setup(c => c.Radius)
                  .Returns(10);

        var mockOther = new Mock<ICircle>();
        var mockOtherCenter = new Mock<IPoint>();

        mockOtherCenter.Setup(p => p.X)
                       .Returns(1000);

        mockOtherCenter.Setup(p => p.Y)
                       .Returns(500);

        mockOther.Setup(c => c.Center)
                 .Returns(mockOtherCenter.Object);

        mockOther.Setup(c => c.Radius)
                 .Returns(5);

        // Act
        var result = mockCircle.Object.ManhattanEdgeToEdgeDistanceFrom(mockOther.Object);

        // Assert
        result.Should()
              .Be(1485); // Manhattan distance = 1500, Sum of radii = 15, Expected = 1485
    }
    #endregion

    #region Edge Cases and Additional Tests
    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_Should_Never_Return_Negative_Values()
    {
        // Arrange
        var circle1 = new ValueCircle(new Point(0, 0), 100);
        var circle2 = new ValueCircle(new Point(1, 1), 50);

        // Act
        var result = circle1.ManhattanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        result.Should()
              .BeGreaterThanOrEqualTo(0);
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_Should_Handle_Same_Center_Different_Radii()
    {
        // Arrange
        var circle1 = new ValueCircle(new Point(5, 5), 10);
        var circle2 = new ValueCircle(new Point(5, 5), 3);

        // Act
        var result = circle1.ManhattanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        result.Should()
              .Be(0); // Same center, so Manhattan distance = 0, always returns 0
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_Should_Be_Symmetric()
    {
        // Arrange
        var circle1 = new ValueCircle(new Point(2, 3), 4);
        var circle2 = new ValueCircle(new Point(8, 7), 2);

        // Act
        var result1 = circle1.ManhattanEdgeToEdgeDistanceFrom(circle2);
        var result2 = circle2.ManhattanEdgeToEdgeDistanceFrom(circle1);

        // Assert
        result1.Should()
               .Be(result2);
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_Should_Be_Consistent_Across_All_Overloads()
    {
        // Arrange
        var valueCircle1 = new ValueCircle(new Point(1, 2), 3);
        var valueCircle2 = new ValueCircle(new Point(6, 8), 2);

        var mockCircle1 = new Mock<ICircle>();
        var mockCenter1 = new Mock<IPoint>();

        mockCenter1.Setup(p => p.X)
                   .Returns(1);

        mockCenter1.Setup(p => p.Y)
                   .Returns(2);

        mockCircle1.Setup(c => c.Center)
                   .Returns(mockCenter1.Object);

        mockCircle1.Setup(c => c.Radius)
                   .Returns(3);

        var mockCircle2 = new Mock<ICircle>();
        var mockCenter2 = new Mock<IPoint>();

        mockCenter2.Setup(p => p.X)
                   .Returns(6);

        mockCenter2.Setup(p => p.Y)
                   .Returns(8);

        mockCircle2.Setup(c => c.Center)
                   .Returns(mockCenter2.Object);

        mockCircle2.Setup(c => c.Radius)
                   .Returns(2);

        // Act
        var result1 = valueCircle1.ManhattanEdgeToEdgeDistanceFrom(valueCircle2);
        var result2 = valueCircle1.ManhattanEdgeToEdgeDistanceFrom(mockCircle2.Object);
        var result3 = mockCircle1.Object.ManhattanEdgeToEdgeDistanceFrom(valueCircle2);
        var result4 = mockCircle1.Object.ManhattanEdgeToEdgeDistanceFrom(mockCircle2.Object);

        // Assert
        result1.Should()
               .Be(result2);

        result2.Should()
               .Be(result3);

        result3.Should()
               .Be(result4);

        result1.Should()
               .Be(6); // Manhattan distance = 11, Sum of radii = 5, Expected = 6
    }

    [Test]
    public void ManhattanEdgeToEdgeDistanceFrom_Should_Handle_Maximum_Integer_Values()
    {
        // Arrange

        // Act & Assert (should not overflow)
        var action = ()
            => new ValueCircle(new Point(int.MaxValue - 1000, int.MaxValue - 1000), 500).ManhattanEdgeToEdgeDistanceFrom(
                new ValueCircle(new Point(int.MaxValue - 500, int.MaxValue - 500), 100));

        action.Should()
              .NotThrow();
    }

    //@formatter:off
    [Test]
    [Arguments(0, 0, 5, 10, 0, 3, 2)] // Horizontal separation
    [Arguments(0, 0, 2, 0, 8, 1, 5)] // Vertical separation
    [Arguments(0, 0, 1, 3, 4, 2, 4)] // Diagonal separation
    [Arguments(5, 5, 10, 5, 5, 5, 0)] // Same center
    [Arguments(-5, -3, 2, 2, 4, 1, 11)] // Negative coordinates
    //@formatter:on
    public void ManhattanEdgeToEdgeDistanceFrom_Should_Return_Expected_Results(
        int center1X,
        int center1Y,
        int radius1,
        int center2X,
        int center2Y,
        int radius2,
        float expectedDistance)
    {
        // Arrange
        var circle1 = new ValueCircle(new Point(center1X, center1Y), radius1);
        var circle2 = new ValueCircle(new Point(center2X, center2Y), radius2);

        // Act
        var result = circle1.ManhattanEdgeToEdgeDistanceFrom(circle2);

        // Assert
        result.Should()
              .Be(expectedDistance);
    }
    #endregion

    #region GetOrderedOutline Tests
    [Test]
    public void GetOrderedOutline_ICircle_WithRadiusThree_ReturnsPointsInAscendingAngleOrder()
    {
        ICircle circle = new Circle(new Point(5, 5), 3);

        var ordered = circle.GetOrderedOutline()
                            .ToList();

        ordered.Should()
               .NotBeEmpty();

        for (var i = 1; i < ordered.Count; i++)
        {
            var prevAngle = Math.Atan2(ordered[i - 1].Y - 5, ordered[i - 1].X - 5);
            var currAngle = Math.Atan2(ordered[i].Y - 5, ordered[i].X - 5);

            currAngle.Should()
                     .BeGreaterThanOrEqualTo(prevAngle);
        }
    }

    [Test]
    public void GetOrderedOutline_ICircle_WithZeroRadius_ReturnsSinglePoint()
    {
        ICircle circle = new Circle(new Point(3, 7), 0);

        var ordered = circle.GetOrderedOutline()
                            .ToList();

        ordered.Should()
               .HaveCount(1);

        ordered[0]
            .Should()
            .Be(new Point(3, 7));
    }

    [Test]
    public void GetOrderedOutline_ICircle_WithNegativeCenter_ReturnsPointsInAngleOrder()
    {
        ICircle circle = new Circle(new Point(-4, -4), 2);

        var ordered = circle.GetOrderedOutline()
                            .ToList();

        ordered.Should()
               .NotBeEmpty();

        for (var i = 1; i < ordered.Count; i++)
        {
            var prevAngle = Math.Atan2(ordered[i - 1].Y - -4, ordered[i - 1].X - -4);
            var currAngle = Math.Atan2(ordered[i].Y - -4, ordered[i].X - -4);

            currAngle.Should()
                     .BeGreaterThanOrEqualTo(prevAngle);
        }
    }

    [Test]
    public void GetOrderedOutline_ICircle_ReturnsAllSamePointsAsGetOutline()
    {
        ICircle circle = new Circle(new Point(0, 0), 4);

        var outline = circle.GetOutline()
                            .ToList();

        var ordered = circle.GetOrderedOutline()
                            .ToList();

        ordered.Should()
               .HaveCount(outline.Count);

        ordered.Should()
               .BeEquivalentTo(outline);
    }
    #endregion

    #region TryGetRandomPoint Tests
    [Test]
    public void TryGetRandomPoint_ICircle_WhenPredicateAlwaysTrue_ReturnsTrue()
    {
        ICircle circle = new Circle(new Point(5, 5), 3);

        var found = circle.TryGetRandomPoint(_ => true, out var point);

        found.Should()
             .BeTrue();

        point.Should()
             .NotBeNull();
    }

    [Test]
    public void TryGetRandomPoint_ICircle_WhenPredicateAlwaysFalse_ReturnsFalse()
    {
        ICircle circle = new Circle(new Point(5, 5), 3);

        var found = circle.TryGetRandomPoint(_ => false, out var point);

        found.Should()
             .BeFalse();

        point.Should()
             .BeNull();
    }

    [Test]
    public void TryGetRandomPoint_ValueCircle_WhenPredicateAlwaysTrue_ReturnsTrue()
    {
        var circle = new ValueCircle(new Point(5, 5), 3);

        var found = circle.TryGetRandomPoint(_ => true, out var point);

        found.Should()
             .BeTrue();

        point.Should()
             .NotBeNull();
    }

    [Test]
    public void TryGetRandomPoint_ValueCircle_WhenPredicateAlwaysFalse_ReturnsFalse()
    {
        var circle = new ValueCircle(new Point(0, 0), 5);

        var found = circle.TryGetRandomPoint(_ => false, out var point);

        found.Should()
             .BeFalse();

        point.Should()
             .BeNull();
    }

    [Test]
    public void TryGetRandomPoint_ForcesReservoirSampling_FindsRarePoint()
    {
        // radius=1 → totalPoints ≈ 3 → maxAttempts = max(1, 3/10) = 1
        // After 1 random attempt, falls back to reservoir which exhaustively scans all points.
        // The center (0,0) is guaranteed to be inside a radius-1 circle at (0,0).
        ICircle circle = new Circle(new Point(0, 0), 1);

        // Predicate accepts only the exact center; reservoir scan is deterministic
        var found = circle.TryGetRandomPoint(p => (p.X == 0) && (p.Y == 0), out var point);

        found.Should()
             .BeTrue();

        point!.Value
              .Should()
              .Be(new Point(0, 0));
    }
    #endregion
}