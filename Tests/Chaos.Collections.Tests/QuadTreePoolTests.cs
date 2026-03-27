#region
using Chaos.Collections.ObjectPool;
using Chaos.Collections.Specialized;
using Chaos.Geometry;
using FluentAssertions;
#endregion

namespace Chaos.Collections.Tests;

public sealed class QuadTreePoolTests
{
    private QuadTreePool<Point> _pool = null!;

    [Test]
    public void Constructor_ShouldCreatePoolWithSpecifiedCapacity()
    {
        // Arrange & Act
        var pool = new QuadTreePool<Point>(new QuadTreePoolPolicy<Point>(), 5);

        // Assert
        pool.Should()
            .NotBeNull();
    }

    [Test]
    public void Get_AfterReturn_ShouldReturnCleanInstance()
    {
        // Arrange
        var bounds = new Rectangle(
            0,
            0,
            100,
            100);
        var quadTree = _pool.Get(bounds);

        // Add some items and verify they exist
        quadTree.Insert(new Point(10, 10));
        quadTree.Insert(new Point(20, 20));

        quadTree.Count
                .Should()
                .Be(2);

        // Return to pool
        _pool.Return(quadTree);

        // Act - Get new instance (should be cleaned)
        var newQuadTree = _pool.Get(bounds);

        // Assert
        newQuadTree.Count
                   .Should()
                   .Be(0);
    }

    [Test]
    public void Get_WithBounds_OutOfBounds_ShouldNotInsertItem()
    {
        // Arrange
        var bounds = new Rectangle(
            0,
            0,
            10,
            10);
        var quadTree = _pool.Get(bounds);
        var outOfBoundsPoint = new Point(20, 20); // Point outside bounds

        // Act
        var inserted = quadTree.Insert(outOfBoundsPoint);

        // Assert
        inserted.Should()
                .BeFalse();

        quadTree.Count
                .Should()
                .Be(0);
    }

    [Test]
    public void Get_WithBounds_ShouldReturnQuadTreeWithSpecifiedBounds()
    {
        // Arrange
        var bounds = new Rectangle(
            10,
            20,
            100,
            200);

        // Act
        var quadTree = _pool.Get(bounds);

        // Assert
        quadTree.Should()
                .NotBeNull();

        quadTree.Should()
                .BeOfType<QuadTree<Point>>();

        quadTree.Count
                .Should()
                .Be(0);
    }

    [Test]
    public void Get_WithBounds_ShouldSetBoundsOnReturnedQuadTree()
    {
        // Arrange
        var bounds = new Rectangle(
            5,
            10,
            50,
            100);

        // Act
        var quadTree = _pool.Get(bounds);

        // Assert
        quadTree.Should()
                .NotBeNull();

        // We can't directly verify bounds since it's protected, but we can verify functionality
        var testPoint = new Point(25, 50); // Point within bounds

        quadTree.Insert(testPoint)
                .Should()
                .BeTrue();

        quadTree.Count
                .Should()
                .Be(1);
    }

    [Test]
    public void Get_WithDifferentBounds_ShouldOverwritePreviousBounds()
    {
        // Arrange
        var initialBounds = new Rectangle(
            0,
            0,
            50,
            50);

        var newBounds = new Rectangle(
            100,
            100,
            200,
            200);

        var quadTree = _pool.Get(initialBounds);

        // Verify initial bounds work
        quadTree.Insert(new Point(25, 25))
                .Should()
                .BeTrue();
        _pool.Return(quadTree);

        // Act - Get with new bounds
        var reusedQuadTree = _pool.Get(newBounds);

        // Assert - Old bounds should not work, new bounds should work
        reusedQuadTree.Insert(new Point(25, 25))
                      .Should()
                      .BeFalse(); // Outside new bounds

        reusedQuadTree.Insert(new Point(150, 150))
                      .Should()
                      .BeTrue(); // Inside new bounds
    }

    [Test]
    public void Get_WithoutParameters_ShouldReturnQuadTreeInstance()
    {
        // Arrange & Act
        var quadTree = _pool.Get();

        // Assert
        quadTree.Should()
                .NotBeNull();

        quadTree.Should()
                .BeOfType<QuadTree<Point>>();

        quadTree.Count
                .Should()
                .Be(0);
    }

    [Test]
    public void MultipleGet_ShouldReturnDifferentInstancesWhenPoolNotExhausted()
    {
        // Arrange & Act
        var quadTree1 = _pool.Get();
        var quadTree2 = _pool.Get();

        // Assert
        quadTree1.Should()
                 .NotBeNull();

        quadTree2.Should()
                 .NotBeNull();

        quadTree1.Should()
                 .NotBeSameAs(quadTree2);
    }

    [Test]
    public void Pool_ShouldHandleNegativeBounds()
    {
        // Arrange
        var negativeBounds = new Rectangle(
            -100,
            -100,
            50,
            50);

        // Act
        var quadTree = _pool.Get(negativeBounds);

        // Assert
        quadTree.Should()
                .NotBeNull();

        quadTree.Insert(new Point(-75, -75))
                .Should()
                .BeTrue(); // Point within negative bounds

        quadTree.Insert(new Point(0, 0))
                .Should()
                .BeFalse(); // Point outside bounds
    }

    [Test]
    public void PoolReuse_ShouldReuseReturnedInstances()
    {
        // Arrange
        var firstQuadTree = _pool.Get();
        var firstReference = firstQuadTree;

        // Act - Return the first instance
        _pool.Return(firstQuadTree);

        // Get another instance - should be the same reference due to pooling
        var secondQuadTree = _pool.Get();

        // Assert
        secondQuadTree.Should()
                      .BeSameAs(firstReference);
    }

    [Test]
    public void Return_ShouldAcceptQuadTreeBack()
    {
        // Arrange
        var quadTree = _pool.Get(
            new Rectangle(
                0,
                0,
                2,
                2));

        quadTree.Insert(new Point(1, 1))
                .Should()
                .BeTrue();

        quadTree.Count
                .Should()
                .Be(1);

        // Act
        _pool.Return(quadTree);

        // Assert
        quadTree.Count
                .Should()
                .Be(0); // Should be cleared when returned
    }

    [Before(Test)]
    public void Setup() => _pool = new QuadTreePool<Point>(new QuadTreePoolPolicy<Point>(), 10);
}