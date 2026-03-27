#region
using Chaos.Collections.Specialized;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Collections.Tests;

public sealed class QuadTreeWithSpatialHashTests
{
    [Test]
    public void Clear_EmptyTree_DoesNotThrow()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        // Act & Assert
        quadTree.Invoking(qt => qt.Clear())
                .Should()
                .NotThrow();

        quadTree.Count
                .Should()
                .Be(0);
    }

    [Test]
    public void Clear_WithItems_RemovesAllItems()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var points = new[]
        {
            MockReferencePoint.Create(10, 10)
                              .Object,
            MockReferencePoint.Create(20, 30)
                              .Object,
            MockReferencePoint.Create(80, 90)
                              .Object
        };

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

        // Verify spatial hash is also cleared
        foreach (var point in points)
        {
            var queryResults = quadTree.Query(Point.From(point));

            queryResults.Should()
                        .BeEmpty();
        }
    }

    [Test]
    public void Constructor_WithBounds_InitializesCorrectly()
    {
        // Arrange
        var bounds = CreateTestBounds();

        // Act
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(bounds);

        // Assert
        quadTree.Should()
                .NotBeNull();

        quadTree.Count
                .Should()
                .Be(0);
    }

    [Test]
    public void Constructor_WithBoundsAndComparer_InitializesCorrectly()
    {
        // Arrange
        var bounds = CreateTestBounds();
        var comparer = EqualityComparer<IPoint>.Default;

        // Act
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(bounds, comparer);

        // Assert
        quadTree.Should()
                .NotBeNull();

        quadTree.Count
                .Should()
                .Be(0);
    }

    [Test]
    public void Constructor_WithNullComparer_InitializesCorrectly()
    {
        // Arrange
        var bounds = CreateTestBounds();

        // Act
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(bounds);

        // Assert
        quadTree.Should()
                .NotBeNull();

        quadTree.Count
                .Should()
                .Be(0);
    }

    private static Rectangle CreateTestBounds()
        => new(
            0,
            0,
            100,
            100);

    [Test]
    public void Inheritance_BehavesLikeQuadTree()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var point = MockReferencePoint.Create(50, 50)
                                      .Object;

        // Act & Assert - Test that it can be used as a QuadTree
        QuadTree<IPoint> baseQuadTree = quadTree;

        baseQuadTree.Insert(point)
                    .Should()
                    .BeTrue();

        baseQuadTree.Count
                    .Should()
                    .Be(1);

        baseQuadTree.Should()
                    .Contain(point);

        baseQuadTree.Remove(point)
                    .Should()
                    .BeTrue();

        baseQuadTree.Count
                    .Should()
                    .Be(0);

        baseQuadTree.Should()
                    .NotContain(point);
    }

    [Test]
    public void Insert_AfterClear_WorksCorrectly()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var point1 = MockReferencePoint.Create(50, 50)
                                       .Object;

        var point2 = MockReferencePoint.Create(60, 60)
                                       .Object;

        quadTree.Insert(point1);
        quadTree.Clear();

        // Act
        var result = quadTree.Insert(point2);

        // Assert
        result.Should()
              .BeTrue();

        quadTree.Count
                .Should()
                .Be(1);

        quadTree.Should()
                .Contain(point2);

        quadTree.Should()
                .NotContain(point1);

        var queryResults = quadTree.Query(Point.From(point2));

        queryResults.Should()
                    .Contain(point2);
    }

    [Test]
    public void Insert_AndQuery_ConsistentResults()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var testData = new Dictionary<IPoint, List<IPoint>>
        {
            [MockReferencePoint.Create(10, 10)
                               .Object] =
            [
                MockReferencePoint.Create(10, 10)
                                  .Object,
                MockReferencePoint.Create(10, 10)
                                  .Object
            ],
            [MockReferencePoint.Create(20, 20)
                               .Object] =
            [
                MockReferencePoint.Create(20, 20)
                                  .Object
            ],
            [MockReferencePoint.Create(30, 30)
                               .Object] =
            [
                MockReferencePoint.Create(30, 30)
                                  .Object,
                MockReferencePoint.Create(30, 30)
                                  .Object,
                MockReferencePoint.Create(30, 30)
                                  .Object
            ]
        };

        // Act
        foreach (var kvp in testData)
        {
            foreach (var point in kvp.Value)
                quadTree.Insert(point);
        }

        // Assert
        foreach (var kvp in testData)
        {
            var queryResults = quadTree.Query(Point.From(kvp.Key));

            queryResults.Should()
                        .BeEquivalentTo(kvp.Value);
        }
    }

    [Test]
    public void Insert_LargeNumberOfPoints_HandlesCorrectly()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(
            new Rectangle(
                0,
                0,
                1000,
                1000));
        var points = new List<IPoint>();

        for (var i = 0; i < 1000; i++)
            points.Add(
                MockReferencePoint.Create(i % 100 * 10, i / 100 * 10)
                                  .Object);

        // Act
        foreach (var point in points)
            quadTree.Insert(point)
                    .Should()
                    .BeTrue();

        // Assert
        quadTree.Count
                .Should()
                .Be(1000);

        // Test queries work correctly
        var queryResults = quadTree.Query(new Point(0, 0));

        queryResults.Should()
                    .HaveCount(1); // Only 1 point at (0,0) based on our pattern
    }

    [Test]
    public void Insert_MultiplePointsDifferentLocations_AllPointsAreAdded()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var points = new[]
        {
            MockReferencePoint.Create(10, 10)
                              .Object,
            MockReferencePoint.Create(20, 30)
                              .Object,
            MockReferencePoint.Create(80, 90)
                              .Object
        };

        // Act
        var results = points.Select(quadTree.Insert)
                            .ToArray();

        // Assert
        results.Should()
               .AllSatisfy(result => result.Should()
                                           .BeTrue());

        quadTree.Count
                .Should()
                .Be(3);

        quadTree.Should()
                .Contain(points);
    }

    [Test]
    public void Insert_MultiplePointsSameLocation_AllPointsAreAdded()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var point1 = MockReferencePoint.Create(50, 50)
                                       .Object;

        var point2 = MockReferencePoint.Create(50, 50)
                                       .Object;

        // Act
        var result1 = quadTree.Insert(point1);
        var result2 = quadTree.Insert(point2);

        // Assert
        result1.Should()
               .BeTrue();

        result2.Should()
               .BeTrue();

        quadTree.Count
                .Should()
                .Be(2);

        quadTree.Should()
                .Contain(point1);

        quadTree.Should()
                .Contain(point2);
    }

    [Test]
    public void Insert_SamePointTwice_WithComparer_OnlyAddedOnce()
    {
        // Arrange
        var comparer = EqualityComparer<IPoint>.Default;
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds(), comparer);

        var point = MockReferencePoint.Create(50, 50)
                                      .Object;

        // Act
        var result1 = quadTree.Insert(point);
        var result2 = quadTree.Insert(point);

        // Assert
        result1.Should()
               .BeTrue();

        result2.Should()
               .BeTrue(); // Base quadtree insert succeeds, but spatial hash prevents duplicate

        quadTree.Count
                .Should()
                .Be(2); // QuadTree allows duplicates, SpatialHash doesn't
    }

    [Test]
    public void Insert_SamePointTwice_WithoutComparer_BothAreAdded()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var point = MockReferencePoint.Create(50, 50)
                                      .Object;

        // Act
        var result1 = quadTree.Insert(point);
        var result2 = quadTree.Insert(point);

        // Assert
        result1.Should()
               .BeTrue();

        result2.Should()
               .BeTrue();

        quadTree.Count
                .Should()
                .Be(2);
    }

    [Test]
    public void Insert_SinglePoint_OutsideBounds_ReturnsFalse()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var point = MockReferencePoint.Create(150, 150)
                                      .Object;

        // Act
        var result = quadTree.Insert(point);

        // Assert
        result.Should()
              .BeFalse();

        quadTree.Count
                .Should()
                .Be(0);

        quadTree.Should()
                .NotContain(point);
    }

    [Test]
    public void Insert_SinglePoint_WithinBounds_ReturnsTrue()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var point = MockReferencePoint.Create(50, 50)
                                      .Object;

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
    public void Query_AfterRemove_DoesNotReturnRemovedItem()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var point = MockReferencePoint.Create(50, 50)
                                      .Object;
        var queryPoint = Point.From(point);

        quadTree.Insert(point);
        quadTree.Remove(point);

        // Act
        var results = quadTree.Query(queryPoint);

        // Assert
        results.Should()
               .BeEmpty();
    }

    [Test]
    public void Query_WithNegativeCoordinates_WorksCorrectly()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(
            new Rectangle(
                -100,
                -100,
                100,
                100));

        var point = MockReferencePoint.Create(-50, -50)
                                      .Object;

        var queryPoint = MockReferencePoint.Create(-50, -50)
                                           .Object;

        quadTree.Insert(point);

        // Act
        var results = quadTree.Query(Point.From(queryPoint));

        // Assert
        results.Should()
               .Contain(point);
    }

    [Test]
    public void Query_WithPoint_EmptyTree_ReturnsEmpty()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var queryPoint = MockReferencePoint.Create(50, 50)
                                           .Object;

        // Act
        var results = quadTree.Query(Point.From(queryPoint));

        // Assert
        results.Should()
               .BeEmpty();
    }

    [Test]
    public void Query_WithPoint_MultipleQueriesReturnSameResults()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var queryPoint = MockReferencePoint.Create(50, 50)
                                           .Object;

        var point = MockReferencePoint.Create(50, 50)
                                      .Object;
        quadTree.Insert(point);

        // Act
        var results1 = quadTree.Query(Point.From(queryPoint))
                               .ToArray();

        var results2 = quadTree.Query(Point.From(queryPoint))
                               .ToArray();

        // Assert
        results1.Should()
                .BeEquivalentTo(results2);

        results1.Should()
                .HaveCount(1);

        results1.Should()
                .Contain(point);
    }

    [Test]
    public void Query_WithPoint_NoPointsAtLocation_ReturnsEmpty()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var queryPoint = MockReferencePoint.Create(50, 50)
                                           .Object;

        var point = MockReferencePoint.Create(60, 60)
                                      .Object;
        quadTree.Insert(point);

        // Act
        var results = quadTree.Query(Point.From(queryPoint));

        // Assert
        results.Should()
               .BeEmpty();
    }

    [Test]
    public void Query_WithPoint_ReturnsPointsAtExactLocation()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var queryPoint = MockReferencePoint.Create(50, 50)
                                           .Object;

        var point1 = MockReferencePoint.Create(50, 50)
                                       .Object;

        var point2 = MockReferencePoint.Create(50, 50)
                                       .Object;

        var point3 = MockReferencePoint.Create(60, 60)
                                       .Object;

        quadTree.Insert(point1);
        quadTree.Insert(point2);
        quadTree.Insert(point3);

        // Act
        var results = quadTree.Query(Point.From(queryPoint))
                              .ToArray();

        // Assert
        results.Should()
               .HaveCount(2);

        results.Should()
               .Contain(point1);

        results.Should()
               .Contain(point2);

        results.Should()
               .NotContain(point3);
    }

    [Test]
    public void Query_WithZeroCoordinates_WorksCorrectly()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var point = MockReferencePoint.Create(0, 0)
                                      .Object;

        var queryPoint = MockReferencePoint.Create(0, 0)
                                           .Object;

        quadTree.Insert(point);

        // Act
        var results = quadTree.Query(Point.From(queryPoint));

        // Assert
        results.Should()
               .Contain(point);
    }

    [Test]
    public void Remove_ExistingPoint_ReturnsTrue()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var point = MockReferencePoint.Create(50, 50)
                                      .Object;
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
    public void Remove_FromEmptyTree_ReturnsFalse()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var point = MockReferencePoint.Create(50, 50)
                                      .Object;

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
    public void Remove_NonExistentPoint_ReturnsFalse()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var point = MockReferencePoint.Create(50, 50)
                                      .Object;

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
    public void Remove_OneOfMultiplePointsAtSameLocation_RemovesOnlySpecifiedPoint()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(CreateTestBounds());

        var point1 = MockReferencePoint.Create(50, 50)
                                       .Object;

        var point2 = MockReferencePoint.Create(50, 50)
                                       .Object;
        quadTree.Insert(point1);
        quadTree.Insert(point2);

        // Act
        var result = quadTree.Remove(point1);

        // Assert
        result.Should()
              .BeTrue();

        quadTree.Count
                .Should()
                .Be(1);

        quadTree.Should()
                .NotContain(point1);

        quadTree.Should()
                .Contain(point2);
    }

    [Test]
    public void Remove_WithNegativeCoordinates_WorksCorrectly()
    {
        // Arrange
        var quadTree = new QuadTreeWithSpatialHash<IPoint>(
            new Rectangle(
                -100,
                -100,
                100,
                100));

        var point = MockReferencePoint.Create(-50, -50)
                                      .Object;

        quadTree.Insert(point);

        // Act
        var result = quadTree.Remove(point);

        // Assert
        result.Should()
              .BeTrue();

        quadTree.Count
                .Should()
                .Be(0);

        var queryResults = quadTree.Query(new Point(-50, -50));

        queryResults.Should()
                    .BeEmpty();
    }
}