#region
using Chaos.Geometry.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Geometry.Tests;

public sealed class PolygonTests
{
    [Test]
    public void Polygon_Constructor_CreatesPolygonWithEmptyVertices()
    {
        // Act
        var polygon = new Polygon();

        // Assert
        polygon.Vertices
               .Should()
               .NotBeNull();

        polygon.Vertices
               .Should()
               .BeEmpty();
    }

    [Test]
    public void Polygon_Constructor_CreatesPolygonWithGivenVertices()
    {
        // Arrange
        var vertices = new List<IPoint>
        {
            new Point(10, 20),
            new Point(30, 40),
            new Point(50, 60)
        };

        // Act
        var polygon = new Polygon(vertices);

        // Assert
        polygon.Vertices
               .Should()
               .HaveCount(3);

        polygon.Vertices
               .Should()
               .ContainInOrder(vertices);
    }

    [Test]
    public void Polygon_Equals_ReturnsFalseWhenComparingWithDifferentType()
    {
        // Arrange
        var vertices = new List<IPoint>
        {
            new Point(10, 20),
            new Point(30, 40),
            new Point(50, 60)
        };

        var polygon = new Polygon(vertices);
        var otherObject = new object();

        // Act
        var result = polygon.Equals(otherObject);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void Polygon_Equals_ReturnsFalseWhenPolygonsAreNotEqual()
    {
        // Arrange
        var vertices1 = new List<IPoint>
        {
            new Point(10, 20),
            new Point(30, 40),
            new Point(50, 60)
        };

        var vertices2 = new List<IPoint>
        {
            new Point(50, 60),
            new Point(30, 40),
            new Point(10, 20)
        };

        var polygon1 = new Polygon(vertices1);
        var polygon2 = new Polygon(vertices2);

        // Act
        var result = polygon1.Equals(polygon2);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void Polygon_Equals_ReturnsFalseWhenPolygonsHaveDifferentVertexCounts()
    {
        // Arrange
        var vertices1 = new List<IPoint>
        {
            new Point(10, 20),
            new Point(30, 40),
            new Point(50, 60)
        };

        var vertices2 = new List<IPoint>
        {
            new Point(10, 20),
            new Point(30, 40)
        };

        var polygon1 = new Polygon(vertices1);
        var polygon2 = new Polygon(vertices2);

        // Act
        var result1 = polygon1.Equals(polygon2);
        var result2 = polygon2.Equals(polygon1);

        // Assert
        result1.Should()
               .BeFalse();

        result2.Should()
               .BeFalse();
    }

    [Test]
    public void Polygon_Equals_ReturnsFalseWhenPolygonsHaveDifferentVertices()
    {
        // Arrange
        var vertices1 = new List<IPoint>
        {
            new Point(10, 20),
            new Point(30, 40),
            new Point(50, 60)
        };

        var vertices2 = new List<IPoint>
        {
            new Point(70, 80),
            new Point(90, 100),
            new Point(110, 120)
        };

        var polygon1 = new Polygon(vertices1);
        var polygon2 = new Polygon(vertices2);

        // Act
        var result1 = polygon1.Equals(polygon2);
        var result2 = polygon2.Equals(polygon1);

        // Assert
        result1.Should()
               .BeFalse();

        result2.Should()
               .BeFalse();
    }

    [Test]
    public void Polygon_Equals_ReturnsTrueWhenPolygonsAreEqual()
    {
        // Arrange
        var vertices = new List<IPoint>
        {
            new Point(10, 20),
            new Point(30, 40),
            new Point(50, 60)
        };

        var polygon1 = new Polygon(vertices);
        var polygon2 = new Polygon(vertices);

        // Act
        var result = polygon1.Equals(polygon2);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void Polygon_Equals_ReturnsTrueWhenPolygonsHaveSameVerticesInDifferentOrder()
    {
        // Arrange
        var vertices1 = new List<IPoint>
        {
            new Point(10, 20),
            new Point(30, 40),
            new Point(50, 60)
        };

        var vertices2 = new List<IPoint>
        {
            new Point(50, 60),
            new Point(10, 20),
            new Point(30, 40)
        };

        var polygon1 = new Polygon(vertices1);
        var polygon2 = new Polygon(vertices2);

        // Act
        var result1 = polygon1.Equals(polygon2);
        var result2 = polygon2.Equals(polygon1);

        // Assert
        result1.Should()
               .BeTrue();

        result2.Should()
               .BeTrue();
    }

    [Test]
    public void Polygon_GetEnumerator_ReturnsVerticesEnumerator()
    {
        // Arrange
        var vertices = new List<IPoint>
        {
            new Point(10, 20),
            new Point(30, 40),
            new Point(50, 60)
        };

        var polygon = new Polygon(vertices);

        // Act
        var actualVertices = polygon.ToList();

        // Assert
        actualVertices.Should()
                      .ContainInOrder(vertices);
    }

    [Test]
    public void Polygon_GetHashCode_ReturnsConsistentHashCode()
    {
        // Arrange
        var vertices = new List<IPoint>
        {
            new Point(10, 20),
            new Point(30, 40),
            new Point(50, 60)
        };

        var polygon = new Polygon(vertices);
        var expectedHashCode = new HashCode();

        foreach (var vertex in vertices)
            expectedHashCode.Add(vertex);

        // Act
        var hashCode1 = polygon.GetHashCode();
        var hashCode2 = polygon.GetHashCode();

        // Assert
        hashCode1.Should()
                 .Be(expectedHashCode.ToHashCode());

        hashCode2.Should()
                 .Be(expectedHashCode.ToHashCode());
    }
}