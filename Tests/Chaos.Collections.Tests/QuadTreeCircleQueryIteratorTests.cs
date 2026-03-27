#region
using Chaos.Collections.Specialized;
using Chaos.Geometry;
using FluentAssertions;
#endregion

namespace Chaos.Collections.Tests;

public sealed class QuadTreeCircleQueryIteratorTests
{
    private static readonly Rectangle TestBounds = new(
        0,
        0,
        100,
        100);

    [Test]
    public void Query_ICircle_DegenerateRadiusZero_MatchesOnlyCenter()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);
        var center = new Point(20, 20);
        var centerDup = new Point(20, 20);
        var neighbor = new Point(21, 20);
        quadTree.Insert(center);
        quadTree.Insert(centerDup);
        quadTree.Insert(neighbor);

        var circle = new Circle(center, 0);

        // Act
        var results = quadTree.Query(circle)
                              .ToList();

        // Assert
        results.Should()
               .HaveCount(2);

        results.Should()
               .OnlyContain(p => p.Equals(center));
    }

    [Test]
    public void Query_ICircle_LargeTree_Grid_PicksExpectedAndBoundaryOnly()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(
            new Rectangle(
                0,
                0,
                1000,
                1000),
            true);

        for (var x = 0; x <= 1000; x += 10)
            for (var y = 0; y <= 1000; y += 10)
                quadTree.Insert(new Point(x, y));

        var center = new Point(500, 500);
        var circle = new Circle(center, 100);

        // Act
        var results = quadTree.Query(circle)
                              .ToList();

        // Build expected set from same grid logic
        var expected = new List<Point>();

        for (var x = 0; x <= 1000; x += 10)
            for (var y = 0; y <= 1000; y += 10)
            {
                var dist = Math.Abs(x - center.X) + Math.Abs(y - center.Y);

                if (dist <= 100)
                    expected.Add(new Point(x, y));
            }

        // Assert equality (order-independent)
        results.Should()
               .BeEquivalentTo(expected);

        // Boundary inclusions
        results.Should()
               .Contain(new Point(600, 500));

        results.Should()
               .Contain(new Point(400, 500));

        results.Should()
               .Contain(new Point(500, 600));

        results.Should()
               .Contain(new Point(500, 400));

        // Exclusions outside boundary
        results.Should()
               .NotContain(new Point(610, 500));

        results.Should()
               .NotContain(new Point(500, 610));
    }

    [Test]
    public void Query_ICircle_MultiNode_VariousSpatialRelationships_ReturnsOnlyMatches()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);
        var center = new Point(50, 50);

        // inside (Manhattan distance <= 10)
        var inside1 = new Point(55, 50);
        var inside2 = new Point(50, 60);

        // boundary
        var boundary = new Point(60, 50);

        // outside
        var outside1 = new Point(61, 50);
        var outside2 = new Point(10, 10);

        foreach (var p in new[]
                 {
                     inside1,
                     inside2,
                     boundary,
                     outside1,
                     outside2
                 })
            quadTree.Insert(p);

        var circle = new Circle(center, 10);

        // Act
        var results = quadTree.Query(circle)
                              .ToList();

        // Assert
        results.Should()
               .Contain(inside1);

        results.Should()
               .Contain(inside2);

        results.Should()
               .Contain(boundary); // boundary included

        results.Should()
               .NotContain(outside1);

        results.Should()
               .NotContain(outside2);
    }

    [Test]
    public void Query_ICircle_Reset_Allows_Reenumeration_From_Start()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);

        var pts = new[]
        {
            new Point(0, 0),
            new Point(10, 0),
            new Point(5, 5)
        };

        foreach (var p in pts)
            quadTree.Insert(p);
        var circle = new Circle(new Point(0, 0), 10);

        // Act
        var enumerable = quadTree.Query(circle);
        var pass1 = enumerable.ToList();
        var pass2 = enumerable.ToList();

        // Assert
        pass1.Should()
             .BeEquivalentTo(pass2);

        pass1.Should()
             .OnlyContain(p => (Math.Abs(p.X - 0) + Math.Abs(p.Y - 0)) <= 10);
    }

    [Test]
    public void Query_ICircle_SingleNode_ManhattanBoundaryPoint_IsIncluded()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);
        var boundary = new Point(15, 10); // |dx|+|dy| == 5
        quadTree.Insert(boundary);
        var circle = new Circle(new Point(10, 10), 5);

        // Act
        var results = quadTree.Query(circle)
                              .ToList();

        // Assert
        results.Should()
               .HaveCount(1);

        results.Should()
               .Contain(boundary);
    }

    [Test]
    public void Query_ICircle_SingleNode_ManhattanOutsidePoint_IsExcluded()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);
        var outside = new Point(16, 10); // |dx|+|dy| == 6 > 5
        quadTree.Insert(outside);
        var circle = new Circle(new Point(10, 10), 5);

        // Act
        var results = quadTree.Query(circle)
                              .ToList();

        // Assert
        results.Should()
               .BeEmpty();
    }

    [Test]
    public void Query_WithCircle_OnEmptyTree_IsEmptyOnRepeatedEnumeration()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);
        var circle = new Circle(new Point(10, 10), 5);

        // Act
        var enumerable = quadTree.Query(circle);
        var firstPass = enumerable.ToList();
        var secondPass = enumerable.ToList();

        // Assert
        firstPass.Should()
                 .BeEmpty();

        secondPass.Should()
                  .BeEmpty();
    }

    [Test]
    public void Query_WithCircle_OnEmptyTree_ReturnsEmpty()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);
        var circle = new Circle(new Point(10, 10), 5);

        // Act
        var result = quadTree.Query(circle)
                             .ToList();

        // Assert
        result.Should()
              .BeEmpty();
    }

    [Test]
    public void Single_node_inside_circle_is_returned_and_reenumerable()
    {
        // Arrange
        var quadTree = new QuadTree<Point>(TestBounds, true);
        var inside = new Point(10, 10);
        quadTree.Insert(inside);
        var circle = new Circle(new Point(10, 10), 5);

        // Act
        var enumerable = quadTree.Query(circle);
        var first = enumerable.ToList();
        var second = enumerable.ToList();

        // Assert
        first.Should()
             .HaveCount(1);

        first.Should()
             .Contain(inside);

        second.Should()
              .HaveCount(1);

        second.Should()
              .Contain(inside);
    }
}