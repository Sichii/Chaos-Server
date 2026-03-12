#region
using System.Collections;
using Chaos.Geometry.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Geometry.Tests;

public sealed class RectangleTests
{
    [Test]
    public void Ctor_FromCenter_Sets_Coordinates_As_Expected()
    {
        var center = new Point(5, 5);
        var rect = new Rectangle(center, 3, 5); // odd dimensions

        rect.Left
            .Should()
            .Be(4);

        rect.Top
            .Should()
            .Be(3);

        rect.Right
            .Should()
            .Be(6);

        rect.Bottom
            .Should()
            .Be(7);
    }

    [Test]
    public void Equals_IRectangle_DifferentHeight_ReturnsFalse()
    {
        IRectangle a = new Rectangle(
            0,
            0,
            2,
            2);

        IRectangle diffHeight = new Rectangle(
            0,
            0,
            2,
            3);

        a.Equals(diffHeight)
         .Should()
         .BeFalse();
    }

    [Test]
    public void Equals_IRectangle_DifferentTop_ReturnsFalse()
    {
        IRectangle a = new Rectangle(
            0,
            0,
            2,
            2);

        IRectangle diffTop = new Rectangle(
            0,
            1,
            2,
            2);

        a.Equals(diffTop)
         .Should()
         .BeFalse();
    }

    [Test]
    public void Equals_IRectangle_DifferentWidth_ReturnsFalse()
    {
        IRectangle a = new Rectangle(
            0,
            0,
            2,
            2);

        IRectangle diffWidth = new Rectangle(
            0,
            0,
            3,
            2);

        a.Equals(diffWidth)
         .Should()
         .BeFalse();
    }

    [Test]
    public void Equals_IRectangle_Paths()
    {
        IRectangle a = new Rectangle(
            0,
            0,
            2,
            2);

        IRectangle same = new Rectangle(
            0,
            0,
            2,
            2);

        IRectangle diff = new Rectangle(
            1,
            0,
            2,
            2);

        a.Equals(null!)
         .Should()
         .BeFalse();

        a!.Equals(a)
          .Should()
          .BeTrue();

        a.Equals(same)
         .Should()
         .BeTrue();

        a.Equals(diff)
         .Should()
         .BeFalse();
    }

    [Test]
    public void Equals_Object_Paths()
    {
        var a = new Rectangle(
            0,
            0,
            2,
            2);

        object sameTypeSame = new Rectangle(
            0,
            0,
            2,
            2);

        object sameTypeDiff = new Rectangle(
            0,
            1,
            2,
            2);

        a.Equals(null!)
         .Should()
         .BeFalse();

        a!.Equals((object)a)
          .Should()
          .BeTrue();

        a.Equals(sameTypeSame)
         .Should()
         .BeTrue();

        a.Equals(sameTypeDiff)
         .Should()
         .BeFalse();

        a.Equals(new object())
         .Should()
         .BeFalse();
    }

    [Test]
    public void GetEnumerator_And_Enumerable_Enumerator_Work()
    {
        var rect = new Rectangle(
            0,
            0,
            2,
            2);

        rect.GetEnumerator()
            .Should()
            .NotBeNull();

        ((IEnumerable)rect).GetEnumerator()
                           .Should()
                           .NotBeNull();
    }

    [Test]
    public void Vertices_Are_Generated_In_Expected_Order()
    {
        var rect = new Rectangle(
            1,
            2,
            3,
            2); // Right=3, Bottom=3
        var verts = rect.Vertices;

        verts.Should()
             .BeEquivalentTo(
                 new List<IPoint>
                 {
                     new Point(1, 2),
                     new Point(3, 2),
                     new Point(3, 3),
                     new Point(1, 3)
                 },
                 o => o.WithStrictOrdering());
    }

    [Test]
    public void Vertices_ShouldReturnCachedInstance_OnSecondAccess()
    {
        var rect = new Rectangle(
            1,
            2,
            3,
            2);

        var first = rect.Vertices;
        var second = rect.Vertices;

        ReferenceEquals(first, second)
            .Should()
            .BeTrue();
    }
}