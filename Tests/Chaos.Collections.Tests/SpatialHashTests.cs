#region
using Chaos.Collections.Specialized;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Collections.Tests;

public sealed class SpatialHashTests
{
    [Test]
    public void Add_LargeNumberOfItems_PerformsCorrectly()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        const int ITEM_COUNT = 10000;

        // Act
        for (var i = 0; i < ITEM_COUNT; i++)
        {
            var point = new TestPoint(i % 100, i / 100, $"Point{i}");
            spatialHash.Add(point);
        }

        // Assert
        var results = spatialHash.Query(new Point(50, 50));

        results.Should()
               .NotBeEmpty();

        // Verify we can query different locations
        spatialHash.Query(new Point(0, 0))
                   .Should()
                   .NotBeEmpty();

        spatialHash.Query(new Point(99, 99))
                   .Should()
                   .NotBeEmpty();
    }

    [Test]
    public void Add_MultipleItemsDifferentLocations_ItemsAreAddedToCorrectLocations()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point1 = new TestPoint(1, 1, "Point1");
        var point2 = new TestPoint(2, 2, "Point2");
        var point3 = new TestPoint(3, 3, "Point3");

        // Act
        spatialHash.Add(point1);
        spatialHash.Add(point2);
        spatialHash.Add(point3);

        // Assert
        spatialHash.Query(new Point(1, 1))
                   .Should()
                   .ContainSingle()
                   .Which
                   .Should()
                   .Be(point1);

        spatialHash.Query(new Point(2, 2))
                   .Should()
                   .ContainSingle()
                   .Which
                   .Should()
                   .Be(point2);

        spatialHash.Query(new Point(3, 3))
                   .Should()
                   .ContainSingle()
                   .Which
                   .Should()
                   .Be(point3);
    }

    [Test]
    public void Add_MultipleItemsSameLocation_AllItemsAreAdded()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point1 = new TestPoint(5, 10, "Point1");
        var point2 = new TestPoint(5, 10, "Point2");
        var point3 = new TestPoint(5, 10, "Point3");

        // Act
        spatialHash.Add(point1);
        spatialHash.Add(point2);
        spatialHash.Add(point3);

        // Assert
        var results = spatialHash.Query(new Point(5, 10))
                                 .ToList();

        results.Should()
               .HaveCount(3);

        results.Should()
               .Contain(point1);

        results.Should()
               .Contain(point2);

        results.Should()
               .Contain(point3);
    }

    [Test]
    public void Add_NegativeCoordinates_WorksCorrectly()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point = new TestPoint(-5, -10, "NegativePoint");

        // Act
        spatialHash.Add(point);

        // Assert
        var results = spatialHash.Query(new Point(-5, -10));

        results.Should()
               .ContainSingle()
               .Which
               .Should()
               .Be(point);
    }

    [Test]
    public void Add_SameItemTwice_ItemIsAddedOnce()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point = new TestPoint(5, 10, "TestPoint");

        // Act
        spatialHash.Add(point);
        spatialHash.Add(point);

        // Assert
        var results = spatialHash.Query(new Point(5, 10));

        results.Should()
               .ContainSingle()
               .Which
               .Should()
               .Be(point);
    }

    [Test]
    public void Add_SingleItem_ItemIsAdded()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point = new TestPoint(5, 10, "TestPoint");

        // Act
        spatialHash.Add(point);

        // Assert
        var results = spatialHash.Query(new Point(5, 10));

        results.Should()
               .ContainSingle()
               .Which
               .Should()
               .Be(point);
    }

    [Test]
    public void Add_WithCustomComparer_UsesComparerForDuplicateDetection()
    {
        // Arrange
        var comparer = new TestPointComparer();
        var spatialHash = new SpatialHash<TestPoint>(comparer);
        var point1 = new TestPoint(5, 10, "Point1");
        var point2 = new TestPoint(5, 10, "Point2"); // Same coordinates, different name

        // Act
        spatialHash.Add(point1);
        spatialHash.Add(point2);

        // Assert
        var results = spatialHash.Query(new Point(5, 10));

        results.Should()
               .ContainSingle(); // Should only contain one due to custom comparer
    }

    [Test]
    public void Add_ZeroCoordinates_WorksCorrectly()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point = new TestPoint(0, 0, "OriginPoint");

        // Act
        spatialHash.Add(point);

        // Assert
        var results = spatialHash.Query(new Point(0, 0));

        results.Should()
               .ContainSingle()
               .Which
               .Should()
               .Be(point);
    }

    [Test]
    public void Clear_EmptySpatialHash_DoesNotThrow()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();

        // Act & Assert
        var action = () => spatialHash.Clear();

        action.Should()
              .NotThrow();
    }

    [Test]
    public void Clear_WithItems_RemovesAllItems()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point1 = new TestPoint(1, 1, "Point1");
        var point2 = new TestPoint(2, 2, "Point2");
        var point3 = new TestPoint(3, 3, "Point3");
        spatialHash.Add(point1);
        spatialHash.Add(point2);
        spatialHash.Add(point3);

        // Act
        spatialHash.Clear();

        // Assert
        spatialHash.Query(new Point(1, 1))
                   .Should()
                   .BeEmpty();

        spatialHash.Query(new Point(2, 2))
                   .Should()
                   .BeEmpty();

        spatialHash.Query(new Point(3, 3))
                   .Should()
                   .BeEmpty();
    }

    [Test]
    public void Constructor_WithComparer_InitializesCorrectly()
    {
        // Arrange
        var comparer = new TestPointComparer();

        // Act
        var spatialHash = new SpatialHash<TestPoint>(comparer);

        // Assert
        spatialHash.Should()
                   .NotBeNull();
    }

    [Test]
    public void Constructor_WithoutComparer_InitializesCorrectly()
    {
        // Act
        var spatialHash = new SpatialHash<TestPoint>();

        // Assert
        spatialHash.Should()
                   .NotBeNull();
    }

    [Test]
    public void Query_EmptySpatialHash_ReturnsEmptyCollection()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();

        // Act
        var results = spatialHash.Query(new Point(5, 10));

        // Assert
        results.Should()
               .BeEmpty();
    }

    [Test]
    public void Query_ExistingLocation_ReturnsCorrectItems()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point1 = new TestPoint(5, 10, "Point1");
        var point2 = new TestPoint(5, 10, "Point2");
        spatialHash.Add(point1);
        spatialHash.Add(point2);

        // Act
        var results = spatialHash.Query(new Point(5, 10))
                                 .ToList();

        // Assert
        results.Should()
               .HaveCount(2);

        results.Should()
               .Contain(point1);

        results.Should()
               .Contain(point2);
    }

    [Test]
    public void Query_NegativeCoordinates_WorksCorrectly()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point = new TestPoint(-5, -10, "NegativePoint");
        spatialHash.Add(point);

        // Act
        var results = spatialHash.Query(new Point(-5, -10));

        // Assert
        results.Should()
               .ContainSingle()
               .Which
               .Should()
               .Be(point);
    }

    [Test]
    public void Query_NonExistentLocation_ReturnsEmptyCollection()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point = new TestPoint(5, 10, "Point");
        spatialHash.Add(point);

        // Act
        var results = spatialHash.Query(new Point(1, 1));

        // Assert
        results.Should()
               .BeEmpty();
    }

    [Test]
    public void Query_ReturnsEnumerableThatCanBeIteratedMultipleTimes()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point1 = new TestPoint(5, 10, "Point1");
        var point2 = new TestPoint(5, 10, "Point2");
        spatialHash.Add(point1);
        spatialHash.Add(point2);

        // Act
        var results = spatialHash.Query(new Point(5, 10))
                                 .ToList();

        // Assert
        results.Should()
               .HaveCount(2);

        results.Should()
               .HaveCount(2); // Second iteration should work

        var firstIteration = results.ToList();
        var secondIteration = results.ToList();

        firstIteration.Should()
                      .BeEquivalentTo(secondIteration);
    }

    [Test]
    public void Remove_ExistingItem_RemovesItemAndReturnsTrue()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point = new TestPoint(5, 10, "TestPoint");
        spatialHash.Add(point);

        // Act
        var removed = spatialHash.Remove(point);

        // Assert
        removed.Should()
               .BeTrue();

        spatialHash.Query(new Point(5, 10))
                   .Should()
                   .BeEmpty();
    }

    [Test]
    public void Remove_ItemAtNonExistentLocation_ReturnsFalse()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point = new TestPoint(5, 10, "TestPoint");

        // Act
        var removed = spatialHash.Remove(point);

        // Assert
        removed.Should()
               .BeFalse();
    }

    [Test]
    public void Remove_LastItemAtLocation_RemovesLocationFromMap()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point = new TestPoint(5, 10, "TestPoint");
        spatialHash.Add(point);

        // Act
        var removed = spatialHash.Remove(point);

        // Assert
        removed.Should()
               .BeTrue();

        spatialHash.Query(new Point(5, 10))
                   .Should()
                   .BeEmpty();
    }

    [Test]
    public void Remove_NegativeCoordinates_WorksCorrectly()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point = new TestPoint(-5, -10, "NegativePoint");
        spatialHash.Add(point);

        // Act
        var removed = spatialHash.Remove(point);

        // Assert
        removed.Should()
               .BeTrue();

        spatialHash.Query(new Point(-5, -10))
                   .Should()
                   .BeEmpty();
    }

    [Test]
    public void Remove_NonExistentItem_ReturnsFalse()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point1 = new TestPoint(5, 10, "Point1");
        var point2 = new TestPoint(1, 1, "Point2");
        spatialHash.Add(point1);

        // Act
        var removed = spatialHash.Remove(point2);

        // Assert
        removed.Should()
               .BeFalse();

        spatialHash.Query(new Point(5, 10))
                   .Should()
                   .ContainSingle()
                   .Which
                   .Should()
                   .Be(point1);
    }

    [Test]
    public void Remove_OneOfMultipleItemsAtSameLocation_RemovesOnlySpecifiedItem()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var point1 = new TestPoint(5, 10, "Point1");
        var point2 = new TestPoint(5, 10, "Point2");
        var point3 = new TestPoint(5, 10, "Point3");
        spatialHash.Add(point1);
        spatialHash.Add(point2);
        spatialHash.Add(point3);

        // Act
        var removed = spatialHash.Remove(point2);

        // Assert
        removed.Should()
               .BeTrue();

        var results = spatialHash.Query(new Point(5, 10))
                                 .ToList();

        results.Should()
               .HaveCount(2);

        results.Should()
               .Contain(point1);

        results.Should()
               .Contain(point3);

        results.Should()
               .NotContain(point2);
    }

    [Test]
    public void Remove_WithCustomComparer_UsesComparerCorrectly()
    {
        // Arrange
        var comparer = new TestPointComparer();
        var spatialHash = new SpatialHash<TestPoint>(comparer);
        var point1 = new TestPoint(5, 10, "Point1");
        var point2 = new TestPoint(5, 10, "Point2"); // Same coordinates, different name

        spatialHash.Add(point1);

        // Act
        var removed = spatialHash.Remove(point2); // Should be considered equal due to comparer

        // Assert
        removed.Should()
               .BeTrue();

        spatialHash.Query(new Point(5, 10))
                   .Should()
                   .BeEmpty();
    }

    [Test]
    public void SpatialHash_ConcurrentOperations_ThreadSafe()
    {
        // Arrange
        var spatialHash = new SpatialHash<TestPoint>();
        var tasks = new List<Task>();

        // Act
        for (var i = 0; i < 100; i++)
        {
            var point = new TestPoint(i % 10, i / 10, $"Point{i}");

            tasks.Add(Task.Run(() => spatialHash.Add(point)));
        }

        Task.WaitAll(tasks.ToArray());

        // Assert
        for (var x = 0; x < 10; x++)
        {
            for (var y = 0; y < 10; y++)
            {
                var results = spatialHash.Query(new Point(x, y));

                results.Should()
                       .NotBeEmpty();
            }
        }
    }

    private sealed class TestPoint : IPoint, IEquatable<TestPoint>
    {
        private string Name { get; }
        public int X { get; }
        public int Y { get; }

        public TestPoint(int x, int y, string name = "")
        {
            X = x;
            Y = y;
            Name = name;
        }

        public bool Equals(TestPoint? other)
        {
            if (other is null)
                return false;

            return (X == other.X) && (Y == other.Y) && (Name == other.Name);
        }

        public override bool Equals(object? obj) => obj is TestPoint other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y, Name);

        public override string ToString() => $"({X}, {Y}) - {Name}";
    }

    private sealed class TestPointComparer : IEqualityComparer<TestPoint>
    {
        public bool Equals(TestPoint? x, TestPoint? y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x is null || y is null)
                return false;

            return (x.X == y.X) && (x.Y == y.Y); // Ignore name for comparison
        }

        public int GetHashCode(TestPoint obj) => HashCode.Combine(obj.X, obj.Y);
    }
}