#region
using Chaos.Collections.Specialized;
using Chaos.Geometry;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Collections.Tests;

/// <summary>
///     Unit tests for the <see cref="QuadTree{T}" /> class.
/// </summary>
public sealed class QuadTreeTests
{
    private readonly IRectangle TestBounds = new Rectangle(
        0,
        0,
        100,
        100);

    [Test]
    public void Clear_RemovesAllItems()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);

        var points = Enumerable.Range(0, 50)
                               .Select(i => new Point(i % 10, i / 10))
                               .ToList();

        foreach (var point in points)
            quadTree.Insert(point);

        // Act
        quadTree.Clear();

        // Assert
        quadTree.Count
                .Should()
                .Be(0);

        quadTree.Should()
                .BeEmpty();
    }

    [Test]
    public void Constructor_Default_InitializesWithEmptyRectangle()
    {
        // Arrange & Act
        var quadTree = new QuadTree<Point>();

        // Assert
        quadTree.Count
                .Should()
                .Be(0);

        quadTree.Should()
                .BeEmpty();
    }

    [Test]
    public void Constructor_WithBounds_InitializesCorrectly()
    {
        // Arrange & Act
        var quadTree = new QuadTree<Point>(TestBounds, true);

        // Assert
        quadTree.Count
                .Should()
                .Be(0);

        quadTree.Should()
                .BeEmpty();
    }

    [Test]
    public void Count_UpdatesCorrectlyDuringOperations()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);

        // Act & Assert
        quadTree.Count
                .Should()
                .Be(0);

        var point1 = new Point(10, 10);
        quadTree.Insert(point1);

        quadTree.Count
                .Should()
                .Be(1);

        var point2 = new Point(20, 20);
        quadTree.Insert(point2);

        quadTree.Count
                .Should()
                .Be(2);

        quadTree.Remove(point1);

        quadTree.Count
                .Should()
                .Be(1);

        quadTree.Clear();

        quadTree.Count
                .Should()
                .Be(0);
    }

    [Test]
    public void Enumeration_EmptyTree_ReturnsEmpty()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);

        // Act
        var enumeratedPoints = quadTree.ToList();

        // Assert
        enumeratedPoints.Should()
                        .BeEmpty();
    }

    [Test]
    public void Enumeration_IteratesAllItems()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);

        var points = Enumerable.Range(0, 50)
                               .Select(i => new Point(i % 10, i / 10))
                               .ToList();

        foreach (var point in points)
            quadTree.Insert(point);

        // Act
        var enumeratedPoints = quadTree.ToList();

        // Assert
        enumeratedPoints.Should()
                        .HaveCount(points.Count);

        enumeratedPoints.Should()
                        .BeEquivalentTo(points);
    }

    [Test]
    public void Insert_InSmallBounds_DoesNotSubdivide()
    {
        // Arrange
        var smallBounds = new Rectangle(
            0,
            0,
            1,
            1);
        var quadTree = new QuadTree<Point>(smallBounds, true);

        var points = Enumerable.Range(0, QuadTree<Point>.Capacity + 5)
                               .Select(_ => new Point(0, 0))
                               .ToList();

        // Act
        foreach (var point in points)
            quadTree.Insert(point);

        // Assert
        quadTree.Count
                .Should()
                .Be(points.Count);
    }

    [Test]
    public void Insert_LargeNumberOfPoints_HandlesCorrectly()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(
            new Rectangle(
                0,
                0,
                1000,
                1000),
            true);
        var random = new Random(42);

        var points = Enumerable.Range(0, 1000)
                               .Select(_ => new Point(random.Next(0, 1000), random.Next(0, 1000)))
                               .ToList();

        // Act
        foreach (var point in points)
            quadTree.Insert(point);

        // Assert
        quadTree.Count
                .Should()
                .Be(points.Count);

        foreach (var point in points)
            quadTree.Should()
                    .Contain(point);
    }

    [Test]
    public void Insert_MultiplePoints_WithinCapacity_DoesNotSubdivide()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);

        var points = Enumerable.Range(0, QuadTree<Point>.Capacity)
                               .Select(i => new Point(i, i))
                               .ToList();

        // Act
        foreach (var point in points)
            quadTree.Insert(point);

        // Assert
        quadTree.Count
                .Should()
                .Be(points.Count);

        foreach (var point in points)
            quadTree.Should()
                    .Contain(point);
    }

    [Test]
    public void Insert_PointsExceedingCapacity_TriggersSubdivision()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);

        var points = Enumerable.Range(0, QuadTree<Point>.Capacity + 5)
                               .Select(i => new Point(i % 10, i / 10))
                               .ToList();

        // Act
        foreach (var point in points)
            quadTree.Insert(point);

        // Assert
        quadTree.Count
                .Should()
                .Be(points.Count);

        foreach (var point in points)
            quadTree.Should()
                    .Contain(point);
    }

    [Test]
    public void Insert_SinglePoint_OutsideBounds_ReturnsFalse()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);
        var point = new Point(200, 200);

        // Act
        var result = quadTree.Insert(point);

        // Assert
        result.Should()
              .BeFalse();

        quadTree.Count
                .Should()
                .Be(0);

        quadTree.Should()
                .BeEmpty();
    }

    [Test]
    public void Insert_SinglePoint_WithinBounds_ReturnsTrue()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);
        var point = new Point(10, 10);

        // Act
        var result = quadTree.Insert(point);

        // Assert
        result.Should()
              .BeTrue();

        quadTree.Count
                .Should()
                .Be(1);

        quadTree.Should()
                .Contain(point);
    }

    [Test]
    public void Query_EmptyTree_ReturnsEmpty()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);

        var queryBounds = new Rectangle(
            0,
            0,
            50,
            50);

        // Act
        var result = quadTree.Query(queryBounds)
                             .ToList();

        // Assert
        result.Should()
              .BeEmpty();
    }

    [Test]
    public void Query_LargeTree_PerformsEfficiently()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(
            new Rectangle(
                0,
                0,
                1000,
                1000),
            true);
        var random = new Random(42);

        var points = Enumerable.Range(0, 1000)
                               .Distinct()
                               .Select(_ => new Point(random.Next(0, 1000), random.Next(0, 1000)))
                               .ToList();

        foreach (var point in points)
            quadTree.Insert(point)
                    .Should()
                    .BeTrue();

        var queryBounds = new Rectangle(
            100,
            100,
            200,
            200);

        // Act
        var queryResults = quadTree.Query(queryBounds)
                                   .ToList();

        // Assert
        queryResults.Should()
                    .OnlyContain(p => queryBounds.ContainsPoint(p));

        // Verify all points in bounds are returned
        var expectedPoints = points.Where(p => queryBounds.ContainsPoint(p))
                                   .ToList();

        queryResults.Should()
                    .BeEquivalentTo(expectedPoints);
    }

    [Test]
    public void Query_WithCircle_ReturnsPointsWithinBounds()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);

        var points = new[]
        {
            new Point(10, 10),
            new Point(12, 12),
            new Point(30, 30),
            new Point(40, 40)
        };

        foreach (var point in points)
            quadTree.Insert(point);

        var queryBounds = new Circle(new Point(10, 10), 5);

        // Act
        var result = quadTree.Query(queryBounds)
                             .ToList();

        // Assert
        result.Should()
              .HaveCount(2);

        result.Should()
              .Contain(points[0]);

        result.Should()
              .Contain(points[1]);

        result.Should()
              .NotContain(points[2]);

        result.Should()
              .NotContain(points[3]);
    }

    [Test]
    public void Query_WithPoint_ReturnsExactMatches()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);

        var points = new[]
        {
            new Point(10, 10),
            new Point(10, 10), // Duplicate
            new Point(15, 15),
            new Point(20, 20)
        };

        foreach (var point in points)
            quadTree.Insert(point);

        var queryPoint = new Point(10, 10);

        // Act
        var result = quadTree.Query(queryPoint)
                             .ToList();

        // Assert
        result.Should()
              .HaveCount(2); // Both points at (10,10)

        result.Should()
              .AllSatisfy(p => p.Should()
                                .Be(queryPoint));
    }

    [Test]
    public void Query_WithRectangle_ReturnsPointsWithinBounds()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);

        var points = new[]
        {
            new Point(5, 5),
            new Point(15, 15),
            new Point(25, 25),
            new Point(35, 35)
        };

        foreach (var point in points)
            quadTree.Insert(point);

        var queryBounds = new Rectangle(
            0,
            0,
            20,
            20);

        // Act
        var result = quadTree.Query(queryBounds)
                             .ToList();

        // Assert
        result.Should()
              .HaveCount(2);

        result.Should()
              .Contain(points[0]);

        result.Should()
              .Contain(points[1]);

        result.Should()
              .NotContain(points[2]);

        result.Should()
              .NotContain(points[3]);
    }

    [Test]
    public void Remove_ExistingPoint_ReturnsTrue()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);
        var point = new Point(10, 10);
        quadTree.Insert(point);

        // Act
        var result = quadTree.Remove(point);

        // Assert
        result.Should()
              .BeTrue();

        quadTree.Count
                .Should()
                .Be(0);

        quadTree.Should()
                .NotContain(point);
    }

    [Test]
    public void Remove_FromSubdividedTree_TriggersLinking()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);

        var points = Enumerable.Range(0, QuadTree<Point>.Capacity + 5)
                               .Select(i => new Point(i % 10, i / 10))
                               .ToList();

        foreach (var point in points)
            quadTree.Insert(point);

        // Act - Remove points to trigger merge
        var pointsToRemove = points.Take(10)
                                   .ToList();

        foreach (var point in pointsToRemove)
            quadTree.Remove(point);

        // Assert
        quadTree.Count
                .Should()
                .Be(points.Count - pointsToRemove.Count);

        foreach (var removedPoint in pointsToRemove)
            quadTree.Should()
                    .NotContain(removedPoint);

        foreach (var remainingPoint in points.Except(pointsToRemove))
            quadTree.Should()
                    .Contain(remainingPoint);
    }

    [Test]
    public void Remove_NonExistentPoint_ReturnsFalse()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);
        var point = new Point(10, 10);

        // Act
        var result = quadTree.Remove(point);

        // Assert
        result.Should()
              .BeFalse();

        quadTree.Count
                .Should()
                .Be(0);
    }

    [Test]
    public void Remove_PointOutsideBounds_ReturnsFalse()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);
        var point = new Point(200, 200);

        // Act
        var result = quadTree.Remove(point);

        // Assert
        result.Should()
              .BeFalse();

        quadTree.Count
                .Should()
                .Be(0);
    }

    [Test]
    public void SetBounds_UpdatesBounds()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);

        var newBounds = new Rectangle(
            50,
            50,
            100,
            100);

        // Act
        quadTree.SetBounds(newBounds);

        // Assert
        // Verify by inserting a point that would be valid in new bounds but not old bounds
        var point = new Point(60, 60);
        var result = quadTree.Insert(point);

        result.Should()
              .BeTrue();

        quadTree.Should()
                .Contain(point);
    }

    [Test]
    public void SetPoolCapacity_UpdatesPoolCapacity()
    {
        // Arrange
        var initialCapacity = 100;
        var newCapacity = 200;

        // Act & Assert
        var act1 = () => QuadTree<Point>.SetPoolCapacity(initialCapacity);
        var act2 = () => QuadTree<Point>.SetPoolCapacity(newCapacity);

        act1.Should()
            .NotThrow();

        act2.Should()
            .NotThrow();
    }

    [Test]
    public void TryReset_ClearsTreeAndReturnsTrue()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);

        var points = Enumerable.Range(0, 10)
                               .Select(i => new Point(i, i))
                               .ToList();

        foreach (var point in points)
            quadTree.Insert(point);

        // Act
        var result = quadTree.TryReset();

        // Assert
        result.Should()
              .BeTrue();

        quadTree.Count
                .Should()
                .Be(0);

        quadTree.Should()
                .BeEmpty();
    }
}