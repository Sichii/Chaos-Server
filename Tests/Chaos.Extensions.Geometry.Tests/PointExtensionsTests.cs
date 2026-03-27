// ReSharper disable ArrangeAttributes

#region
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Extensions.Geometry.Tests;

public sealed class PointExtensionsTests
{
    //formatter:off
    [Test]
    [Arguments(
        Direction.Up,
        new[]
        {
            -1,
            -1,
            0,
            -1,
            1,
            -1,
            -2,
            -2,
            -1,
            -2,
            0,
            -2,
            1,
            -2,
            2,
            -2
        })]
    [Arguments(
        Direction.Right,
        new[]
        {
            1,
            -1,
            1,
            0,
            1,
            1,
            2,
            -2,
            2,
            -1,
            2,
            0,
            2,
            1,
            2,
            2
        })]
    [Arguments(
        Direction.Down,
        new[]
        {
            1,
            1,
            0,
            1,
            -1,
            1,
            2,
            2,
            1,
            2,
            0,
            2,
            -1,
            2,
            -2,
            2
        })]
    [Arguments(
        Direction.Left,
        new[]
        {
            -1,
            -1,
            -1,
            0,
            -1,
            1,
            -2,
            -2,
            -2,
            -1,
            -2,
            0,
            -2,
            1,
            -2,
            2
        })]
    public void ConalSearch_MaxDistanceGreaterThanOne_ReturnsAllPointsWithinCone(Direction direction, params IEnumerable<int> expected)

        //formatter:on
    {
        var startingPoint = new Point(0, 0);

        var points = startingPoint.ConalSearch(direction, 2)
                                  .ToList();

        points.Should()
              .HaveCount(8);

        points.Should()
              .BeEquivalentTo(
                  expected.Chunk(2)
                          .Select(p => new Point(p[0], p[1])));
    }

    //formatter:off
    [Test]
    [Arguments(
        Direction.Up,
        new[]
        {
            -1,
            -1,
            0,
            -1,
            1,
            -1
        })]
    [Arguments(
        Direction.Right,
        new[]
        {
            1,
            -1,
            1,
            0,
            1,
            1
        })]
    [Arguments(
        Direction.Down,
        new[]
        {
            -1,
            1,
            0,
            1,
            1,
            1
        })]
    [Arguments(
        Direction.Left,
        new[]
        {
            -1,
            -1,
            -1,
            0,
            -1,
            1
        })]
    public void ConalSearch_MaxDistanceOne_ReturnsThreePointsInSpecifiedDirection(Direction direction, params IEnumerable<int> expected)

        //formatter:on
    {
        var startingPoint = new Point(0, 0);

        var points = startingPoint.ConalSearch(direction, 1)
                                  .ToList();

        points.Should()
              .HaveCount(3);

        points.Should()
              .BeEquivalentTo(
                  expected.Chunk(2)
                          .Select(p => new Point(p[0], p[1])));
    }

    //formatter:off
    [Test]
    [Arguments(Direction.Up)]
    [Arguments(Direction.Right)]
    [Arguments(Direction.Down)]
    [Arguments(Direction.Left)]
    public void ConalSearch_MaxDistanceZero_ReturnsEmpty(Direction direction)

        //formatter:on
    {
        var startingPoint = new Point(0, 0);

        var points = startingPoint.ConalSearch(direction, 0)
                                  .ToList();

        points.Should()
              .BeEmpty();
    }

    [Test]
    public void ConalSearch_Point_All_GeneratesPoints()
    {
        var point = new Point(0, 0);

        var result = point.ConalSearch(Direction.All, 1)
                          .ToList();

        result.Should()
              .NotBeEmpty(); // Direction.All generates points for all directions
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        Direction.All,
        0)] // Zero distance for All
    [Arguments(
        1,
        1,
        Direction.Invalid,
        1)] // Invalid direction (empty)
    [Arguments(
        5,
        5,
        Direction.Up,
        3)] // Up from non-origin
    [Arguments(
        5,
        5,
        Direction.Right,
        3)] // Right from non-origin  
    [Arguments(
        5,
        5,
        Direction.Down,
        3)] // Down from non-origin
    [Arguments(
        5,
        5,
        Direction.Left,
        3)] // Left from non-origin
    public void ConalSearch_Point_EdgeCases(
            int x,
            int y,
            Direction direction,
            int distance)

        //formatter:on
    {
        var point = new Point(x, y);

        if (direction == Direction.Invalid)
        {
            #pragma warning disable CA1806

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Action act = () => point.ConalSearch(direction, distance)
                                    .ToList();
            #pragma warning restore CA1806
            act.Should()
               .Throw<ArgumentOutOfRangeException>();
        } else if (distance == 0)
        {
            var result = point.ConalSearch(direction, distance)
                              .ToList();

            result.Should()
                  .BeEmpty();
        } else
        {
            var result = point.ConalSearch(direction, distance)
                              .ToList();

            result.Should()
                  .NotBeEmpty();
        }
    }

    //formatter:off
    [Test]
    public void ConalSearch_Point_ThrowsForInvalidDirection()

        //formatter:on
    {
        var point = new Point(0, 0);

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        #pragma warning disable CA1806
        Action act = () => point.ConalSearch(Direction.Invalid, 1)
                                .ToList();
        #pragma warning restore CA1806
        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void ConalSearch_ShouldThrow_WhenDirectionIsInvalid()
    {
        var start = new Point(0, 0);

        var act = () => start.ConalSearch(Direction.Invalid, 1)
                             .ToList();

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void ConalSearch_ShouldThrow_WhenStartIsNull_IPointOverload()
    {
        IPoint start = null!;

        var act = () => start.ConalSearch(Direction.Up, 1)
                             .ToList();

        act.Should()
           .Throw<NullReferenceException>();
    }

    //formatter:off
    [Test]
    [Arguments(
        Direction.Up,
        new[]
        {
            -1,
            -1,
            0,
            -1,
            1,
            -1
        })]
    [Arguments(
        Direction.Right,
        new[]
        {
            1,
            -1,
            1,
            0,
            1,
            1
        })]
    [Arguments(
        Direction.Down,
        new[]
        {
            -1,
            1,
            0,
            1,
            1,
            1
        })]
    [Arguments(
        Direction.Left,
        new[]
        {
            -1,
            -1,
            -1,
            0,
            -1,
            1
        })]
    public void ConalSearch_ValuePoint_Direction_Distance1(Direction direction, params IEnumerable<int> expected)

        //formatter:on
    {
        var v = new ValuePoint(0, 0);

        var pts = v.ConalSearch(direction, 1)
                   .ToList();

        pts.Should()
           .BeEquivalentTo(
               expected.Chunk(2)
                       .Select(p => new Point(p[0], p[1])));
    }

    [Test]
    public void DirectionalOffset_IPoint_Invalid_Throws()
    {
        IPoint p = new Point(0, 0);

        var act = () => p.DirectionalOffset(Direction.Invalid);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
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
    public void DirectionalOffset_ShouldThrow_WhenStartIsNull_IPointOverload()
    {
        IPoint start = null!;

        var act = () => start.DirectionalOffset(Direction.Up);

        act.Should()
           .Throw<NullReferenceException>();
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

    [Test]
    public void DirectionalOffset_ThrowsForAllDirection()
    {
        var point = new Point(0, 0);
        IPoint ipoint = new Point(0, 0);

        Action act1 = () => point.DirectionalOffset(Direction.All);

        var act2 = () =>
        {
            var valuePoint = new ValuePoint(0, 0);
            _ = valuePoint.DirectionalOffset(Direction.All);
        };
        Action act3 = () => ipoint.DirectionalOffset(Direction.All);

        act1.Should()
            .Throw<ArgumentOutOfRangeException>();

        act2.Should()
            .Throw<ArgumentOutOfRangeException>();

        act3.Should()
            .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void DirectionalOffset_ValuePoint_Invalid_Throws()
    {
        var act = () =>
        {
            var v = new ValuePoint(1, 1);
            _ = v.DirectionalOffset(Direction.Invalid);
        };

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    [Arguments(Direction.Up, 0, -1)]
    [Arguments(Direction.Right, 1, 0)]
    [Arguments(Direction.Down, 0, 1)]
    [Arguments(Direction.Left, -1, 0)]
    public void DirectionalOffset_ValuePoint_Normal(Direction dir, int dx, int dy)
    {
        var vp = new ValuePoint(10, 10);
        var p = vp.DirectionalOffset(dir);

        p.Should()
         .Be(new Point(10 + dx, 10 + dy));
    }

    //formatter:off
    [Test]
    [Arguments(
        5,
        0,
        0,
        1,
        Direction.Right)] // x dominates, x> -> Right
    [Arguments(
        2,
        0,
        0,
        10,
        Direction.Up)] // y dominates, y> -> Up
    [Arguments(
        -5,
        0,
        0,
        1,
        Direction.Left)] // x dominates, x< -> Left
    [Arguments(
        -1,
        10,
        1,
        0,
        Direction.Down)] // y dominates, y< -> Down
    public void DirectionalRelationTo_IPoint_IPoint_NonTie(
            int ax,
            int ay,
            int bx,
            int by,
            Direction expected)

        //formatter:on
    {
        IPoint a = new Point(ax, ay);
        IPoint b = new Point(bx, by);

        a.DirectionalRelationTo(b)
         .Should()
         .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(
        1,
        -1,
        Direction.Up,
        Direction.Right)]
    [Arguments(
        1,
        1,
        Direction.Down,
        Direction.Right)]
    [Arguments(
        -1,
        1,
        Direction.Down,
        Direction.Left)]
    [Arguments(
        -1,
        -1,
        Direction.Up,
        Direction.Left)]
    public void DirectionalRelationTo_IPoint_IPoint_Tie_AllCorners(
            int dx,
            int dy,
            Direction expected1,
            Direction expected2)

        //formatter:on
    {
        IPoint a = new Point(dx, dy);
        IPoint b = new Point(0, 0);

        var seen = new HashSet<Direction>();

        for (var i = 0; (i < 200) && (seen.Count < 2); i++)
            seen.Add(a.DirectionalRelationTo(b));

        seen.Should()
            .Contain(expected1);

        seen.Should()
            .Contain(expected2);
    }

    [Test]
    public void DirectionalRelationTo_IPoint_IPoint_Tie_ExercisesBothOutcomes()
    {
        IPoint a = new Point(1, 1);
        IPoint b = new Point(0, 0);

        var seen = new HashSet<Direction>();

        for (var i = 0; (i < 500) && (seen.Count < 2); i++)
            seen.Add(a.DirectionalRelationTo(b));

        seen.Should()
            .Contain(Direction.Down);

        seen.Should()
            .Contain(Direction.Right);
    }

    [Test]
    [Arguments(
        0,
        0,
        0,
        1,
        new[]
        {
            Direction.Up
        })]
    [Arguments(
        0,
        0,
        1,
        0,
        new[]
        {
            Direction.Left,
            Direction.Up,
            Direction.Down
        })]
    public void DirectionalRelationTo_IPoint_IPoint_Ties_ReturnExpectedSet(
        int ax,
        int ay,
        int bx,
        int by,
        IEnumerable<Direction> expected)
    {
        IPoint a = new Point(ax, ay);
        IPoint b = new Point(bx, by);
        var dir = a.DirectionalRelationTo(b);

        expected.Should()
                .Contain(dir);
    }

    //formatter:off
    [Test]
    [Arguments(
        1,
        -1,
        Direction.Up,
        Direction.Right)]
    [Arguments(
        1,
        1,
        Direction.Down,
        Direction.Right)]
    [Arguments(
        -1,
        1,
        Direction.Down,
        Direction.Left)]
    [Arguments(
        -1,
        -1,
        Direction.Up,
        Direction.Left)]
    public void DirectionalRelationTo_IPoint_Point_Tie_AllCorners(
            int dx,
            int dy,
            Direction expected1,
            Direction expected2)

        //formatter:on
    {
        IPoint a = new Point(dx, dy);
        var b = new Point(0, 0);

        var seen = new HashSet<Direction>();

        for (var i = 0; (i < 100) && (seen.Count < 2); i++)
            seen.Add(a.DirectionalRelationTo(b));

        seen.Should()
            .Contain(expected1);

        seen.Should()
            .Contain(expected2);
    }

    [Test]
    public void DirectionalRelationTo_IPoint_Point_Tie_ExercisesBothOutcomes()
    {
        IPoint a = new Point(1, 1);
        var b = new Point(0, 0);

        var seen = new HashSet<Direction>();

        for (var i = 0; (i < 500) && (seen.Count < 2); i++)
            seen.Add(a.DirectionalRelationTo(b));

        seen.Should()
            .Contain(Direction.Down);

        seen.Should()
            .Contain(Direction.Right);
    }

    [Test]
    [Arguments(
        0,
        0,
        0,
        1,
        new[]
        {
            Direction.Up
        })]
    [Arguments(
        0,
        0,
        1,
        0,
        new[]
        {
            Direction.Left,
            Direction.Up,
            Direction.Down
        })]
    public void DirectionalRelationTo_IPoint_ValuePoint_Ties_ReturnExpectedSet(
        int ax,
        int ay,
        int bx,
        int by,
        IEnumerable<Direction> expected)
    {
        IPoint a = new Point(ax, ay);
        var b = new ValuePoint(bx, by);
        var dir = a.DirectionalRelationTo(b);

        expected.Should()
                .Contain(dir);
    }

    //formatter:off
    [Test]
    [Arguments(
        5,
        0,
        0,
        1,
        Direction.Right)] // x dominates, x> -> Right
    [Arguments(
        2,
        0,
        0,
        10,
        Direction.Up)] // y dominates, y> -> Up
    [Arguments(
        -5,
        0,
        0,
        1,
        Direction.Left)] // x dominates, x< -> Left
    [Arguments(
        -1,
        10,
        1,
        0,
        Direction.Down)] // y dominates, y< -> Down
    public void DirectionalRelationTo_Point_IPoint_NonTie(
            int ax,
            int ay,
            int bx,
            int by,
            Direction expected)

        //formatter:on
    {
        var a = new Point(ax, ay);
        IPoint b = new Point(bx, by);

        a.DirectionalRelationTo(b)
         .Should()
         .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(
        1,
        -1,
        Direction.Up,
        Direction.Right)]
    [Arguments(
        1,
        1,
        Direction.Down,
        Direction.Right)]
    [Arguments(
        -1,
        1,
        Direction.Down,
        Direction.Left)]
    [Arguments(
        -1,
        -1,
        Direction.Up,
        Direction.Left)]
    public void DirectionalRelationTo_Point_IPoint_Tie_AllCorners(
            int dx,
            int dy,
            Direction expected1,
            Direction expected2)

        //formatter:on
    {
        var a = new Point(dx, dy);
        IPoint b = new Point(0, 0);

        var seen = new HashSet<Direction>();

        for (var i = 0; (i < 100) && (seen.Count < 2); i++)
            seen.Add(a.DirectionalRelationTo(b));

        seen.Should()
            .Contain(expected1);

        seen.Should()
            .Contain(expected2);
    }

    [Test]
    public void DirectionalRelationTo_Point_IPoint_Tie_ExercisesBothOutcomes()
    {
        var a = new Point(1, 1);
        IPoint b = new Point(0, 0);

        var seen = new HashSet<Direction>();

        for (var i = 0; (i < 500) && (seen.Count < 2); i++)
            seen.Add(a.DirectionalRelationTo(b));

        seen.Should()
            .Contain(Direction.Down);

        seen.Should()
            .Contain(Direction.Right);
    }

    [Test]
    [Arguments(
        0,
        0,
        0,
        1,
        new[]
        {
            Direction.Up
        })]
    [Arguments(
        0,
        0,
        1,
        0,
        new[]
        {
            Direction.Left,
            Direction.Up,
            Direction.Down
        })]
    public void DirectionalRelationTo_Point_IPoint_Ties_ReturnExpectedSet(
        int ax,
        int ay,
        int bx,
        int by,
        IEnumerable<Direction> expected)
    {
        var a = new Point(ax, ay);
        IPoint b = new Point(bx, by);
        var dir = a.DirectionalRelationTo(b);

        expected.Should()
                .Contain(dir);
    }

    //@formatter:off
    [Test]
    [Arguments( 5, 3, 2, 1, new[] { Direction.Right })]
    [Arguments(0, 0, 0, 0, new [] { Direction.Invalid })] // Same point
    [Arguments( 2, 5, 1, 3, new[] { Direction.Down })]
    [Arguments( 0, 0, -3, 0, new [] { Direction.Right })] // Pure west
    [Arguments( 0, 0, 3, 0, new [] { Direction.Left })] // Pure east
    [Arguments( 0, 0, 0, -3, new [] { Direction.Down })] // Pure south  
    [Arguments( 0, 0, 0, 3, new [] { Direction.Up })] // Pure north
    //@formatter:on
    public void DirectionalRelationTo_Point_Point_ExtendedCases(
        int px,
        int py,
        int ox,
        int oy,
        params IEnumerable<Direction> expected)

    {
        var point = new Point(px, py);
        var other = new Point(ox, oy);
        var direction = point.DirectionalRelationTo(other);

        expected.Should()
                .Contain(direction);
    }

    //formatter:off
    [Test]
    [Arguments(
        1,
        -1,
        Direction.Up,
        Direction.Right)]
    [Arguments(
        1,
        1,
        Direction.Down,
        Direction.Right)]
    [Arguments(
        -1,
        1,
        Direction.Down,
        Direction.Left)]
    [Arguments(
        -1,
        -1,
        Direction.Up,
        Direction.Left)]
    public void DirectionalRelationTo_Point_ValuePoint_Tie_AllCorners(
            int dx,
            int dy,
            Direction expected1,
            Direction expected2)

        //formatter:on
    {
        var a = new Point(dx, dy);
        var b = new ValuePoint(0, 0);

        var seen = new HashSet<Direction>();

        for (var i = 0; (i < 100) && (seen.Count < 2); i++)
            seen.Add(a.DirectionalRelationTo(b));

        seen.Should()
            .Contain(expected1);

        seen.Should()
            .Contain(expected2);
    }

    [Test]
    public void DirectionalRelationTo_Point_ValuePoint_Tie_ExercisesBothOutcomes()
    {
        var a = new Point(1, 1);
        var b = new ValuePoint(0, 0);

        var seen = new HashSet<Direction>();

        for (var i = 0; (i < 500) && (seen.Count < 2); i++)
            seen.Add(a.DirectionalRelationTo(b));

        seen.Should()
            .Contain(Direction.Down);

        seen.Should()
            .Contain(Direction.Right);
    }

    [Test]
    [Arguments(
        0,
        0,
        0,
        1,
        new[]
        {
            Direction.Up
        })]
    [Arguments(
        0,
        0,
        1,
        0,
        new[]
        {
            Direction.Left,
            Direction.Up,
            Direction.Down
        })]
    public void DirectionalRelationTo_Point_ValuePoint_Ties_ReturnExpectedSet(
        int ax,
        int ay,
        int bx,
        int by,
        IEnumerable<Direction> expected)
    {
        var a = new Point(ax, ay);
        var b = new ValuePoint(bx, by);
        var dir = a.DirectionalRelationTo(b);

        expected.Should()
                .Contain(dir);
    }

    [Test]
    public void DirectionalRelationTo_ShouldThrow_WhenFirstParamIsNull_IPointOverload()
    {
        IPoint start = null!;
        IPoint other = new Point(1, 1);

        var act = () => start.DirectionalRelationTo(other);

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void DirectionalRelationTo_ShouldThrow_WhenSecondParamIsNull_IPointOverload()
    {
        IPoint start = new Point(0, 0);
        IPoint other = null!;

        var act = () => start.DirectionalRelationTo(other);

        act.Should()
           .Throw<NullReferenceException>();
    }

    //formatter:off
    [Test]
    [Arguments(
        5,
        0,
        0,
        1,
        Direction.Right)] // x dominates, x> -> Right
    [Arguments(
        2,
        0,
        0,
        10,
        Direction.Up)] // y dominates, y> -> Up
    [Arguments(
        -5,
        0,
        0,
        1,
        Direction.Left)] // x dominates, x< -> Left
    [Arguments(
        -1,
        10,
        1,
        0,
        Direction.Down)] // y dominates, y< -> Down
    public void DirectionalRelationTo_ValuePoint_IPoint_NonTie(
            int ax,
            int ay,
            int bx,
            int by,
            Direction expected)

        //formatter:on
    {
        var a = new ValuePoint(ax, ay);
        IPoint b = new Point(bx, by);

        a.DirectionalRelationTo(b)
         .Should()
         .Be(expected);
    }

    [Test]
    [Arguments(
        0,
        0,
        0,
        1,
        new[]
        {
            Direction.Up
        })]
    [Arguments(
        0,
        0,
        1,
        0,
        new[]
        {
            Direction.Left,
            Direction.Up,
            Direction.Down
        })]
    public void DirectionalRelationTo_ValuePoint_IPoint_Ties_ReturnExpectedSet(
        int ax,
        int ay,
        int bx,
        int by,
        IEnumerable<Direction> expected)
    {
        var a = new ValuePoint(ax, ay);
        IPoint b = new Point(bx, by);
        var dir = a.DirectionalRelationTo(b);

        expected.Should()
                .Contain(dir);
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        0,
        2,
        Direction.Up)]
    [Arguments(
        0,
        0,
        0,
        -2,
        Direction.Down)]
    [Arguments(
        0,
        0,
        2,
        0,
        Direction.Left)]
    [Arguments(
        0,
        0,
        -2,
        0,
        Direction.Right)]
    public void DirectionalRelationTo_ValuePoint_Point_NonTie_Exact(
            int ax,
            int ay,
            int bx,
            int by,
            Direction expected)

        //formatter:on
    {
        var a = new ValuePoint(ax, ay);
        var b = new Point(bx, by);

        a.DirectionalRelationTo(b)
         .Should()
         .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(
        1,
        -1,
        Direction.Up,
        Direction.Right)]
    [Arguments(
        1,
        1,
        Direction.Down,
        Direction.Right)]
    [Arguments(
        -1,
        1,
        Direction.Down,
        Direction.Left)]
    [Arguments(
        -1,
        -1,
        Direction.Up,
        Direction.Left)]
    public void DirectionalRelationTo_ValuePoint_Point_Tie_AllCorners(
            int dx,
            int dy,
            Direction expected1,
            Direction expected2)

        //formatter:on
    {
        var a = new ValuePoint(dx, dy);
        var b = new Point(0, 0);

        var seen = new HashSet<Direction>();

        for (var i = 0; (i < 100) && (seen.Count < 2); i++)
            seen.Add(a.DirectionalRelationTo(b));

        seen.Should()
            .Contain(expected1);

        seen.Should()
            .Contain(expected2);
    }

    [Test]
    public void DirectionalRelationTo_ValuePoint_Point_Tie_ExercisesBothOutcomes()
    {
        var a = new ValuePoint(1, 1);
        var b = new Point(0, 0);

        var seen = new HashSet<Direction>();

        for (var i = 0; (i < 500) && (seen.Count < 2); i++)
            seen.Add(a.DirectionalRelationTo(b));

        seen.Should()
            .Contain(Direction.Down);

        seen.Should()
            .Contain(Direction.Right);
    }

    [Test]
    [Arguments(
        0,
        0,
        0,
        1,
        new[]
        {
            Direction.Up
        })]
    [Arguments(
        0,
        0,
        1,
        0,
        new[]
        {
            Direction.Left,
            Direction.Up,
            Direction.Down
        })]
    public void DirectionalRelationTo_ValuePoint_Point_Ties_ReturnExpectedSet(
        int ax,
        int ay,
        int bx,
        int by,
        IEnumerable<Direction> expected)
    {
        var a = new ValuePoint(ax, ay);
        var b = new Point(bx, by);
        var dir = a.DirectionalRelationTo(b);

        expected.Should()
                .Contain(dir);
    }

    //formatter:off
    [Test]
    [Arguments(
        1,
        -1,
        Direction.Up,
        Direction.Right)] // NE
    [Arguments(
        1,
        1,
        Direction.Down,
        Direction.Right)] // SE
    [Arguments(
        -1,
        1,
        Direction.Down,
        Direction.Left)] // SW
    [Arguments(
        -1,
        -1,
        Direction.Up,
        Direction.Left)] // NW
    public void DirectionalRelationTo_ValuePoint_ValuePoint_Tie_AllCorners(
            int dx,
            int dy,
            Direction expected1,
            Direction expected2)

        //formatter:on
    {
        var a = new ValuePoint(dx, dy);
        var b = new ValuePoint(0, 0);

        var seen = new HashSet<Direction>();

        for (var i = 0; (i < 100) && (seen.Count < 2); i++)
            seen.Add(a.DirectionalRelationTo(b));

        seen.Should()
            .Contain(expected1);

        seen.Should()
            .Contain(expected2);
    }

    [Test]
    public void DirectionalRelationTo_ValuePoint_ValuePoint_Tie_ExercisesBothOutcomes()
    {
        var a = new ValuePoint(1, 1);
        var b = new ValuePoint(0, 0);

        var seen = new HashSet<Direction>();

        for (var i = 0; (i < 500) && (seen.Count < 2); i++)
            seen.Add(a.DirectionalRelationTo(b));

        seen.Should()
            .Contain(Direction.Down);

        seen.Should()
            .Contain(Direction.Right);
    }

    [Test]
    [Arguments(
        0,
        0,
        0,
        1,
        new[]
        {
            Direction.Up
        })]
    [Arguments(
        0,
        0,
        1,
        0,
        new[]
        {
            Direction.Left,
            Direction.Up,
            Direction.Down
        })]
    public void DirectionalRelationTo_ValuePoint_ValuePoint_Ties_ReturnExpectedSet(
        int ax,
        int ay,
        int bx,
        int by,
        IEnumerable<Direction> expected)
    {
        var a = new ValuePoint(ax, ay);
        var b = new ValuePoint(bx, by);
        var dir = a.DirectionalRelationTo(b);

        expected.Should()
                .Contain(dir);
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

    [Test]
    public void EuclideanDistanceFrom_IPoint_ShouldThrow_WhenEitherArgNull()
    {
        IPoint a = null!;
        IPoint b = new Point(0, 0);

        var a1 = a;
        var b1 = b;
        Action act1 = () => a1.EuclideanDistanceFrom(b1);

        act1.Should()
            .Throw<NullReferenceException>();

        a = new Point(0, 0);
        b = null!;
        Action act2 = () => a.EuclideanDistanceFrom(b);

        act2.Should()
            .Throw<NullReferenceException>();
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

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        0,
        0,
        0.0)] // Same point
    [Arguments(
        0,
        0,
        3,
        0,
        3.0)] // Horizontal
    [Arguments(
        0,
        0,
        0,
        4,
        4.0)] // Vertical
    [Arguments(
        0,
        0,
        3,
        4,
        5.0)] // 3-4-5 triangle
    [Arguments(
        1,
        1,
        4,
        5,
        5.0)] // Another diagonal
    public void EuclideanDistanceFrom_ValuePoint_AllOverloads(
            int x1,
            int y1,
            int x2,
            int y2,
            double expectedDistance)

        //formatter:on
    {
        var vp1 = new ValuePoint(x1, y1);
        var vp2 = new ValuePoint(x2, y2);
        var p2 = new Point(x2, y2);
        IPoint ip2 = new Point(x2, y2);

        vp1.EuclideanDistanceFrom(vp2)
           .Should()
           .BeApproximately(expectedDistance, 0.0001);

        vp1.EuclideanDistanceFrom(p2)
           .Should()
           .BeApproximately(expectedDistance, 0.0001);

        vp1.EuclideanDistanceFrom(ip2)
           .Should()
           .BeApproximately(expectedDistance, 0.0001);

        var p1 = new Point(x1, y1);

        p1.EuclideanDistanceFrom(vp2)
          .Should()
          .BeApproximately(expectedDistance, 0.0001);

        p1.EuclideanDistanceFrom(p2)
          .Should()
          .BeApproximately(expectedDistance, 0.0001);

        p1.EuclideanDistanceFrom(ip2)
          .Should()
          .BeApproximately(expectedDistance, 0.0001);

        IPoint ip1 = new Point(x1, y1);

        ip1.EuclideanDistanceFrom(vp2)
           .Should()
           .BeApproximately(expectedDistance, 0.0001);

        ip1.EuclideanDistanceFrom(p2)
           .Should()
           .BeApproximately(expectedDistance, 0.0001);
    }

    [Test]
    public void FindCenterMost_Returns_Point_Closest_To_Centroid()
    {
        var points = new List<Point>
        {
            new(0, 0),
            new(2, 0),
            new(1, 1)
        };

        var centerMost = points.FindCenterMost();

        centerMost.Should()
                  .Be(new Point(1, 1));
    }

    [Test]
    public void FindCenterMost_ShouldThrow_WhenNullOrEmpty()
    {
        ICollection<Point> points = null!;
        var points1 = points;
        Action act1 = () => points1.FindCenterMost();

        act1.Should()
            .Throw<ArgumentException>()
            .WithMessage("*cannot be null or empty*");

        points = new List<Point>();
        Action act2 = () => points.FindCenterMost();

        act2.Should()
            .Throw<ArgumentException>()
            .WithMessage("*cannot be null or empty*");
    }

    [Test]
    public void FloodFill_Generic_WorksWithIPointImplementations()
    {
        var points = new TestPoint[]
        {
            new(0, 0),
            new(1, 0),
            new(0, 1),
            new(1, 1),
            new(3, 3) // Disconnected
        };

        var start = new TestPoint(0, 0);

        var result = points.FloodFill(start)
                           .ToList();

        result.Should()
              .HaveCount(4);

        result.Should()
              .NotContain(p => (p.X == 3) && (p.Y == 3));
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
    public void GenerateCardinalPoints_All_Radius1_ReturnsExpectedPoints()
    {
        var startPoint = new Point(0, 0);

        var expectedPoints = new[]
        {
            new Point(0, 1),
            new Point(1, 0),
            new Point(0, -1),
            new Point(-1, 0)
        };

        var result = startPoint.GenerateCardinalPoints();

        result.Should()
              .BeEquivalentTo(expectedPoints);
    }

    // ========== Additional Comprehensive Tests for Better Branch Coverage ==========

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        Direction.Invalid,
        1,
        new int[0])] // Invalid direction
    [Arguments(
        0,
        0,
        Direction.Up,
        1,
        new[]
        {
            0,
            -1
        })]
    [Arguments(
        0,
        0,
        Direction.Right,
        1,
        new[]
        {
            1,
            0
        })]
    [Arguments(
        0,
        0,
        Direction.Down,
        1,
        new[]
        {
            0,
            1
        })]
    [Arguments(
        0,
        0,
        Direction.Left,
        1,
        new[]
        {
            -1,
            0
        })]
    [Arguments(
        0,
        0,
        Direction.All,
        1,
        new[]
        {
            0,
            -1,
            1,
            0,
            0,
            1,
            -1,
            0
        })]
    [Arguments(
        5,
        5,
        Direction.Up,
        2,
        new[]
        {
            5,
            4,
            5,
            3
        })]
    [Arguments(
        5,
        5,
        Direction.Right,
        2,
        new[]
        {
            6,
            5,
            7,
            5
        })]
    [Arguments(
        5,
        5,
        Direction.Down,
        2,
        new[]
        {
            5,
            6,
            5,
            7
        })]
    [Arguments(
        5,
        5,
        Direction.Left,
        2,
        new[]
        {
            4,
            5,
            3,
            5
        })]
    [Arguments(
        5,
        5,
        Direction.All,
        2,
        new[]
        {
            5,
            4,
            6,
            5,
            5,
            6,
            4,
            5,
            5,
            3,
            7,
            5,
            5,
            7,
            3,
            5
        })]
    public void GenerateCardinalPoints_Point_AllDirectionsAndRadius(
            int x,
            int y,
            Direction direction,
            int radius,
            params IEnumerable<int> expected)

        //formatter:on
    {
        var point = new Point(x, y);

        var result = point.GenerateCardinalPoints(direction, radius)
                          .ToList();

        result.Should()
              .BeEquivalentTo(
                  expected.Chunk(2)
                          .Select(p => new Point(p[0], p[1])));
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

    //formatter:off
    [Test]
    [Arguments(
        Direction.Up,
        new[]
        {
            0,
            -1,
            0,
            -2,
            0,
            -3
        })]
    [Arguments(
        Direction.Right,
        new[]
        {
            1,
            0,
            2,
            0,
            3,
            0
        })]
    [Arguments(
        Direction.Down,
        new[]
        {
            0,
            1,
            0,
            2,
            0,
            3
        })]
    [Arguments(
        Direction.Left,
        new[]
        {
            -1,
            0,
            -2,
            0,
            -3,
            0
        })]
    public void GenerateCardinalPoints_ShouldGeneratePointsInSingleDirection_WhenDirectionIsNotAll(
            Direction direction,
            params IEnumerable<int> expected)

        //formatter:on
    {
        // Arrange
        var startPoint = new Point(2, 2);

        var expectedPoints = expected.Chunk(2)
                                     .Select(p => new Point(2 + p[0], 2 + p[1]));

        // Act
        var result = startPoint.GenerateCardinalPoints(direction, 3);

        // Assert
        result.Should()
              .BeEquivalentTo(expectedPoints);
    }

    [Test]
    public void GenerateCardinalPoints_ShouldThrow_WhenStartIsNull_IPointOverload()
    {
        IPoint start = null!;

        var act = () => start.GenerateCardinalPoints(Direction.Up)
                             .ToList();

        act.Should()
           .Throw<NullReferenceException>();
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
            .WithMessage("*must be a non-negative and non-zero value*");
    }

    [Test]
    public void GenerateCardinalPoints_Up_Radius3_ReturnsExpectedPoints()
    {
        var startPoint = new Point(2, 2);

        var expectedPoints = new[]
        {
            new Point(2, 1),
            new Point(2, 0),
            new Point(2, -1)
        };

        var result = startPoint.GenerateCardinalPoints(Direction.Up, 3);

        result.Should()
              .BeEquivalentTo(expectedPoints);
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        Direction.Invalid,
        1,
        new int[0])] // Invalid direction
    [Arguments(
        0,
        0,
        Direction.Up,
        1,
        new[]
        {
            0,
            -1
        })]
    [Arguments(
        0,
        0,
        Direction.Right,
        1,
        new[]
        {
            1,
            0
        })]
    [Arguments(
        0,
        0,
        Direction.Down,
        1,
        new[]
        {
            0,
            1
        })]
    [Arguments(
        0,
        0,
        Direction.Left,
        1,
        new[]
        {
            -1,
            0
        })]
    [Arguments(
        0,
        0,
        Direction.All,
        1,
        new[]
        {
            0,
            -1,
            1,
            0,
            0,
            1,
            -1,
            0
        })]
    [Arguments(
        3,
        3,
        Direction.All,
        3,
        new[]
        {
            3,
            2,
            4,
            3,
            3,
            4,
            2,
            3,
            3,
            1,
            5,
            3,
            3,
            5,
            1,
            3,
            3,
            0,
            6,
            3,
            3,
            6,
            0,
            3
        })]
    public void GenerateCardinalPoints_ValuePoint_AllDirectionsAndRadius(
            int x,
            int y,
            Direction direction,
            int radius,
            params IEnumerable<int> expected)

        //formatter:on
    {
        var point = new ValuePoint(x, y);

        var result = point.GenerateCardinalPoints(direction, radius)
                          .ToList();

        result.Should()
              .BeEquivalentTo(
                  expected.Chunk(2)
                          .Select(p => new Point(p[0], p[1])));
    }

    //formatter:off
    [Test]
    [Arguments(
        Direction.Up,
        new[]
        {
            0,
            -1,
            0,
            -2
        })]
    [Arguments(
        Direction.Right,
        new[]
        {
            1,
            0,
            2,
            0
        })]
    [Arguments(
        Direction.Down,
        new[]
        {
            0,
            1,
            0,
            2
        })]
    [Arguments(
        Direction.Left,
        new[]
        {
            -1,
            0,
            -2,
            0
        })]
    public void GenerateCardinalPoints_ValuePoint_ByDirection(Direction direction, params IEnumerable<int> expected)

        //formatter:on
    {
        var v = new ValuePoint(0, 0);

        var result = v.GenerateCardinalPoints(direction, 2)
                      .ToList();

        result.Should()
              .BeEquivalentTo(
                  expected.Chunk(2)
                          .Select(p => new Point(p[0], p[1])));
    }

    [Test]
    public void GenerateCardinalPoints_ValuePoint_ThrowsOnNegativeRadius()
    {
        var act = () =>
        {
            var point = new ValuePoint(0, 0);

            _ = point.GenerateCardinalPoints(Direction.All, -1)
                     .ToList();
        };

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void GenerateIntercardinalPoints_IPoint_Invalid_ReturnsEmpty()
    {
        IPoint p = new Point(0, 0);

        p.GenerateIntercardinalPoints(Direction.Invalid, 3)
         .Should()
         .BeEmpty();
    }

    //formatter:off
    [Test]
    [Arguments(
        Direction.Up,
        new[]
        {
            -1,
            -1,
            1,
            -1
        })]
    [Arguments(
        Direction.Right,
        new[]
        {
            1,
            -1,
            1,
            1
        })]
    [Arguments(
        Direction.Down,
        new[]
        {
            1,
            1,
            -1,
            1
        })]
    [Arguments(
        Direction.Left,
        new[]
        {
            -1,
            -1,
            -1,
            1
        })]
    [Arguments(
        Direction.All,
        new[]
        {
            -1,
            -1,
            1,
            -1,
            1,
            1,
            -1,
            1
        })]
    public void GenerateIntercardinalPoints_Point_Direction_Various(Direction direction, params IEnumerable<int> expected)

        //formatter:on
    {
        var start = new Point(0, 0);

        var result = start.GenerateIntercardinalPoints(direction)
                          .ToList();

        result.Should()
              .BeEquivalentTo(
                  expected.Chunk(2)
                          .Select(p => new Point(p[0], p[1])));
    }

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
    public void GenerateIntercardinalPoints_ShouldThrow_WhenStartIsNull_IPointOverload()
    {
        IPoint start = null!;

        var act = () => start.GenerateIntercardinalPoints(Direction.Up)
                             .ToList();

        act.Should()
           .Throw<NullReferenceException>();
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        Direction.Invalid,
        1,
        new int[0])] // Invalid direction
    [Arguments(
        0,
        0,
        Direction.Up,
        1,
        new[]
        {
            -1,
            -1,
            1,
            -1
        })]
    [Arguments(
        0,
        0,
        Direction.Right,
        1,
        new[]
        {
            1,
            -1,
            1,
            1
        })]
    [Arguments(
        0,
        0,
        Direction.Down,
        1,
        new[]
        {
            1,
            1,
            -1,
            1
        })]
    [Arguments(
        0,
        0,
        Direction.Left,
        1,
        new[]
        {
            -1,
            -1,
            -1,
            1
        })]
    [Arguments(
        0,
        0,
        Direction.All,
        1,
        new[]
        {
            -1,
            -1,
            1,
            -1,
            1,
            1,
            -1,
            1
        })]
    [Arguments(
        2,
        2,
        Direction.Up,
        2,
        new[]
        {
            1,
            1,
            3,
            1,
            0,
            0,
            4,
            0
        })]
    [Arguments(
        2,
        2,
        Direction.Right,
        2,
        new[]
        {
            3,
            1,
            3,
            3,
            4,
            0,
            4,
            4
        })]
    [Arguments(
        2,
        2,
        Direction.Down,
        2,
        new[]
        {
            3,
            3,
            1,
            3,
            4,
            4,
            0,
            4
        })]
    [Arguments(
        2,
        2,
        Direction.Left,
        2,
        new[]
        {
            1,
            1,
            1,
            3,
            0,
            0,
            0,
            4
        })]
    public void GenerateIntercardinalPoints_ValuePoint_AllDirectionsAndRadius(
            int x,
            int y,
            Direction direction,
            int radius,
            params IEnumerable<int> expected)

        //formatter:on
    {
        var point = new ValuePoint(x, y);

        var result = point.GenerateIntercardinalPoints(direction, radius)
                          .ToList();

        result.Should()
              .BeEquivalentTo(
                  expected.Chunk(2)
                          .Select(p => new Point(p[0], p[1])));
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
    public void GetDirectPath_Diagonal_Ascending_ReturnsExpectedSubset()
    {
        var start = new Point(0, 0);
        var end = new Point(2, 2);

        var expectedPath = new[]
        {
            new Point(0, 0),
            new Point(0, 1),
            new Point(1, 0),
            new Point(1, 1),
            new Point(1, 2),
            new Point(2, 1),
            new Point(2, 2)
        };

        var result = start.GetDirectPath(end);

        result.Should()
              .BeSubsetOf(expectedPath);
    }

    [Test]
    public void GetDirectPath_Horizontal_ReturnsExpectedSequence()
    {
        var start = new Point(1, 1);
        var end = new Point(3, 1);

        var expectedPath = new[]
        {
            new Point(1, 1),
            new Point(2, 1),
            new Point(3, 1)
        };

        var result = start.GetDirectPath(end);

        result.Should()
              .BeEquivalentTo(expectedPath);
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        0,
        0,
        new[]
        {
            0,
            0
        })] // Same point
    [Arguments(
        0,
        0,
        3,
        0,
        new[]
        {
            0,
            0,
            1,
            0,
            2,
            0,
            3,
            0
        })] // Horizontal
    public void GetDirectPath_IPoint_ToPoint(
            int startX,
            int startY,
            int endX,
            int endY,
            params IEnumerable<int> expected)

        //formatter:on
    {
        IPoint start = new Point(startX, startY);
        var end = new Point(endX, endY);

        var result = start.GetDirectPath(end)
                          .ToList();

        result.Should()
              .BeEquivalentTo(
                  expected.Chunk(2)
                          .Select(p => new Point(p[0], p[1])),
                  options => options.WithStrictOrdering());
    }

    [Test]
    public void GetDirectPath_SameStartAndEnd_ReturnsSinglePoint()
    {
        var start = new Point(0, 0);
        var end = new Point(0, 0);

        var result = start.GetDirectPath(end);

        result.Should()
              .BeEquivalentTo([new Point(0, 0)]);
    }

    [Test]
    public void GetDirectPath_ShouldThrow_WhenEndIsNull_IPointOverload()
    {
        IPoint start = new Point(0, 0);
        IPoint end = null!;

        var act = () => start.GetDirectPath(end)
                             .ToList();

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void GetDirectPath_ShouldThrow_WhenStartIsNull_IPointOverload()
    {
        IPoint start = null!;
        IPoint end = new Point(1, 1);

        var act = () => start.GetDirectPath(end)
                             .ToList();

        act.Should()
           .Throw<NullReferenceException>();
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        0,
        0,
        new[]
        {
            0,
            0
        })] // Same point
    [Arguments(
        0,
        0,
        3,
        0,
        new[]
        {
            0,
            0,
            1,
            0,
            2,
            0,
            3,
            0
        })] // Horizontal
    [Arguments(
        0,
        0,
        0,
        3,
        new[]
        {
            0,
            0,
            0,
            1,
            0,
            2,
            0,
            3
        })] // Vertical
    public void GetDirectPath_ValuePoint_ToIPoint(
            int startX,
            int startY,
            int endX,
            int endY,
            params IEnumerable<int> expected)

        //formatter:on
    {
        var start = new ValuePoint(startX, startY);
        IPoint end = new Point(endX, endY);

        var result = start.GetDirectPath(end)
                          .ToList();

        result.Should()
              .BeEquivalentTo(
                  expected.Chunk(2)
                          .Select(p => new Point(p[0], p[1])),
                  options => options.WithStrictOrdering());
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        0,
        0,
        new[]
        {
            0,
            0
        })] // Same point
    [Arguments(
        0,
        0,
        3,
        0,
        new[]
        {
            0,
            0,
            1,
            0,
            2,
            0,
            3,
            0
        })] // Horizontal
    [Arguments(
        0,
        0,
        0,
        3,
        new[]
        {
            0,
            0,
            0,
            1,
            0,
            2,
            0,
            3
        })] // Vertical
    [Arguments(
        0,
        0,
        3,
        3,
        new[]
        {
            0,
            0
        },
        new[]
        {
            0,
            0,
            0,
            1,
            1,
            0,
            1,
            1,
            1,
            2,
            2,
            1,
            2,
            2,
            2,
            3,
            3,
            2,
            3,
            3
        })] // Diagonal (subset check)
    [Arguments(
        3,
        3,
        0,
        0,
        new[]
        {
            3,
            3
        },
        new[]
        {
            3,
            3,
            3,
            2,
            2,
            3,
            2,
            2,
            2,
            1,
            1,
            2,
            1,
            1,
            1,
            0,
            0,
            1,
            0,
            0
        })] // Reverse diagonal
    public void GetDirectPath_ValuePoint_ToPoint(
            int startX,
            int startY,
            int endX,
            int endY,
            IEnumerable<int> expected,
            IEnumerable<int>? possibleSet = null)

        //formatter:on
    {
        var start = new ValuePoint(startX, startY);
        var end = new Point(endX, endY);

        var result = start.GetDirectPath(end)
                          .ToList();

        if (possibleSet != null)
        {
            var expectedSet = possibleSet.Chunk(2)
                                         .Select(p => new Point(p[0], p[1]))
                                         .ToList();

            result.Should()
                  .BeSubsetOf(expectedSet);

            result.First()
                  .Should()
                  .Be(new Point(startX, startY));

            result.Last()
                  .Should()
                  .Be(end);
        } else
            result.Should()
                  .BeEquivalentTo(
                      expected.Chunk(2)
                              .Select(p => new Point(p[0], p[1])),
                      options => options.WithStrictOrdering());
    }

    [Test]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Up,
        true)]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Right,
        false)]
    [Arguments(
        0,
        0,
        2,
        1,
        Direction.Up,
        false)]
    public void IsInterCardinalTo_IPoint_IPoint(
        int ax,
        int ay,
        int bx,
        int by,
        Direction dir,
        bool expected)
    {
        IPoint a = new Point(ax, ay);
        IPoint b = new Point(bx, by);

        var result = a.IsInterCardinalTo(b, dir);

        result.Should()
              .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.All,
        true)]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Invalid,
        false)]
    public void IsInterCardinalTo_IPoint_IPoint_All_And_Invalid(
            int ax,
            int ay,
            int bx,
            int by,
            Direction dir,
            bool expected)

        //formatter:on
    {
        IPoint a = new Point(ax, ay);
        IPoint b = new Point(bx, by);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .Be(expected);
    }

    //formatter:off
    [Test]

    // Up true branches: (-1,-1) and (1,-1)
    [Arguments(
        -1,
        -1,
        0,
        0,
        Direction.Up,
        true)]
    [Arguments(
        1,
        -1,
        0,
        0,
        Direction.Up,
        true)]

    // Right true branches: (1,-1) and (1,1)
    [Arguments(
        1,
        -1,
        0,
        0,
        Direction.Right,
        true)]
    [Arguments(
        1,
        1,
        0,
        0,
        Direction.Right,
        true)]

    // Down true branches: (1,1) and (-1,1)
    [Arguments(
        1,
        1,
        0,
        0,
        Direction.Down,
        true)]
    [Arguments(
        -1,
        1,
        0,
        0,
        Direction.Down,
        true)]

    // Left true branches: (-1,1) and (-1,-1)
    [Arguments(
        -1,
        1,
        0,
        0,
        Direction.Left,
        true)]
    [Arguments(
        -1,
        -1,
        0,
        0,
        Direction.Left,
        true)]
    public void IsInterCardinalTo_IPoint_IPoint_DirectionBranches(
            int ax,
            int ay,
            int bx,
            int by,
            Direction dir,
            bool expected)

        //formatter:on
    {
        IPoint a = new Point(ax, ay);
        IPoint b = new Point(bx, by);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(Direction.Up, 1, 1)]
    [Arguments(Direction.Up, -1, 1)]
    [Arguments(Direction.Right, -1, -1)]
    [Arguments(Direction.Right, -1, 1)]
    [Arguments(Direction.Down, 1, -1)]
    [Arguments(Direction.Down, -1, -1)]
    [Arguments(Direction.Left, 1, -1)]
    [Arguments(Direction.Left, 1, 1)]
    public void IsInterCardinalTo_IPoint_IPoint_DirectionBranches_False(Direction dir, int ax, int ay)

        //formatter:on
    {
        IPoint a = new Point(ax, ay);
        IPoint b = new Point(0, 0);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .BeFalse();
    }

    [Test]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Up,
        true)]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Left,
        true)]
    [Arguments(
        0,
        0,
        2,
        1,
        Direction.Up,
        false)]
    public void IsInterCardinalTo_IPoint_Point(
        int ax,
        int ay,
        int bx,
        int by,
        Direction dir,
        bool expected)
    {
        IPoint a = new Point(ax, ay);
        var b = new Point(bx, by);

        var result = a.IsInterCardinalTo(b, dir);

        result.Should()
              .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.All,
        true)]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Invalid,
        false)]
    public void IsInterCardinalTo_IPoint_Point_All_And_Invalid(
            int ax,
            int ay,
            int bx,
            int by,
            Direction dir,
            bool expected)

        //formatter:on
    {
        IPoint a = new Point(ax, ay);
        var b = new Point(bx, by);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .Be(expected);
    }

    //formatter:off
    [Test]

    // Up true branches: (-1,-1) and (1,-1)
    [Arguments(
        -1,
        -1,
        0,
        0,
        Direction.Up,
        true)]
    [Arguments(
        1,
        -1,
        0,
        0,
        Direction.Up,
        true)]

    // Right true branches: (1,-1) and (1,1)
    [Arguments(
        1,
        -1,
        0,
        0,
        Direction.Right,
        true)]
    [Arguments(
        1,
        1,
        0,
        0,
        Direction.Right,
        true)]

    // Down true branches: (1,1) and (-1,1)
    [Arguments(
        1,
        1,
        0,
        0,
        Direction.Down,
        true)]
    [Arguments(
        -1,
        1,
        0,
        0,
        Direction.Down,
        true)]

    // Left true branches: (-1,1) and (-1,-1)
    [Arguments(
        -1,
        1,
        0,
        0,
        Direction.Left,
        true)]
    [Arguments(
        -1,
        -1,
        0,
        0,
        Direction.Left,
        true)]
    public void IsInterCardinalTo_IPoint_Point_DirectionBranches(
            int ax,
            int ay,
            int bx,
            int by,
            Direction dir,
            bool expected)

        //formatter:on
    {
        IPoint a = new Point(ax, ay);
        var b = new Point(bx, by);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(Direction.Up, 1, 1)] // yDiff>0 => not Up
    [Arguments(Direction.Up, -1, 1)]
    [Arguments(Direction.Right, -1, -1)] // xDiff<0 => not Right
    [Arguments(Direction.Right, -1, 1)]
    [Arguments(Direction.Down, 1, -1)] // yDiff<0 => not Down
    [Arguments(Direction.Down, -1, -1)]
    [Arguments(Direction.Left, 1, -1)] // xDiff>0 => not Left
    [Arguments(Direction.Left, 1, 1)]
    public void IsInterCardinalTo_IPoint_Point_DirectionBranches_False(Direction dir, int ax, int ay)

        //formatter:on
    {
        IPoint a = new Point(ax, ay);
        var b = new Point(0, 0);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .BeFalse();
    }

    [Test]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Up,
        true)]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Down,
        false)]
    [Arguments(
        0,
        0,
        2,
        1,
        Direction.Up,
        false)]
    public void IsInterCardinalTo_Point_IPoint(
        int ax,
        int ay,
        int bx,
        int by,
        Direction dir,
        bool expected)
    {
        var a = new Point(ax, ay);
        IPoint b = new Point(bx, by);

        var result = a.IsInterCardinalTo(b, dir);

        result.Should()
              .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.All,
        true)]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Invalid,
        false)]
    public void IsInterCardinalTo_Point_IPoint_All_And_Invalid(
            int ax,
            int ay,
            int bx,
            int by,
            Direction dir,
            bool expected)

        //formatter:on
    {
        var a = new Point(ax, ay);
        IPoint b = new Point(bx, by);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .Be(expected);
    }

    //formatter:off
    [Test]

    // Up true branches: (-1,-1) and (1,-1)
    [Arguments(
        -1,
        -1,
        0,
        0,
        Direction.Up,
        true)]
    [Arguments(
        1,
        -1,
        0,
        0,
        Direction.Up,
        true)]

    // Right true branches: (1,-1) and (1,1)
    [Arguments(
        1,
        -1,
        0,
        0,
        Direction.Right,
        true)]
    [Arguments(
        1,
        1,
        0,
        0,
        Direction.Right,
        true)]

    // Down true branches: (1,1) and (-1,1)
    [Arguments(
        1,
        1,
        0,
        0,
        Direction.Down,
        true)]
    [Arguments(
        -1,
        1,
        0,
        0,
        Direction.Down,
        true)]

    // Left true branches: (-1,1) and (-1,-1)
    [Arguments(
        -1,
        1,
        0,
        0,
        Direction.Left,
        true)]
    [Arguments(
        -1,
        -1,
        0,
        0,
        Direction.Left,
        true)]
    public void IsInterCardinalTo_Point_IPoint_DirectionBranches(
            int ax,
            int ay,
            int bx,
            int by,
            Direction dir,
            bool expected)

        //formatter:on
    {
        var a = new Point(ax, ay);
        IPoint b = new Point(bx, by);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(Direction.Up, 1, 1)]
    [Arguments(Direction.Up, -1, 1)]
    [Arguments(Direction.Right, -1, -1)]
    [Arguments(Direction.Right, -1, 1)]
    [Arguments(Direction.Down, 1, -1)]
    [Arguments(Direction.Down, -1, -1)]
    [Arguments(Direction.Left, 1, -1)]
    [Arguments(Direction.Left, 1, 1)]
    public void IsInterCardinalTo_Point_IPoint_DirectionBranches_False(Direction dir, int ax, int ay)

        //formatter:on
    {
        var a = new Point(ax, ay);
        IPoint b = new Point(0, 0);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .BeFalse();
    }

    //formatter:off
    [Test]
    [Arguments(
        1,
        0,
        0,
        -1,
        Direction.Right,
        true)] // xDiff>0,yDiff>0 => Right
    [Arguments(
        -1,
        0,
        0,
        1,
        Direction.Left,
        true)] // xDiff<0,yDiff<0 => Left
    [Arguments(
        0,
        -1,
        1,
        0,
        Direction.Up,
        true)] // yDiff<0 => Up
    [Arguments(
        0,
        1,
        -1,
        0,
        Direction.Down,
        true)] // yDiff>0 => Down
    public void IsInterCardinalTo_Point_IPoint_MoreDirections(
            int ax,
            int ay,
            int bx,
            int by,
            Direction dir,
            bool expected)

        //formatter:on
    {
        var a = new Point(ax, ay);
        IPoint b = new Point(bx, by);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .Be(expected);
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

    [Test]
    public void IsInterCardinalTo_ShouldThrow_WhenFirstParamIsNull_IPointOverload()
    {
        IPoint start = null!;
        IPoint other = new Point(1, 1);

        var act = () => start.IsInterCardinalTo(other, Direction.Up);

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void IsInterCardinalTo_ShouldThrow_WhenSecondParamIsNull_IPointOverload()
    {
        IPoint start = new Point(0, 0);
        IPoint other = null!;

        var act = () => start.IsInterCardinalTo(other, Direction.Up);

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Up,
        true)]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Left,
        true)]
    [Arguments(
        0,
        0,
        2,
        1,
        Direction.Up,
        false)]
    public void IsInterCardinalTo_ValuePoint_IPoint(
        int ax,
        int ay,
        int bx,
        int by,
        Direction dir,
        bool expected)
    {
        var a = new ValuePoint(ax, ay);
        IPoint b = new Point(bx, by);

        var result = a.IsInterCardinalTo(b, dir);

        result.Should()
              .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.All,
        true)]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Invalid,
        false)]
    public void IsInterCardinalTo_ValuePoint_IPoint_All_And_Invalid(
            int ax,
            int ay,
            int bx,
            int by,
            Direction dir,
            bool expected)

        //formatter:on
    {
        var a = new ValuePoint(ax, ay);
        IPoint b = new Point(bx, by);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .Be(expected);
    }

    //formatter:off
    [Test]

    // Up true branches: (-1,-1) and (1,-1)
    [Arguments(-1, -1, Direction.Up)]
    [Arguments(1, -1, Direction.Up)]

    // Right true branches: (1,-1) and (1,1)
    [Arguments(1, -1, Direction.Right)]
    [Arguments(1, 1, Direction.Right)]

    // Down true branches: (1,1) and (-1,1)
    [Arguments(1, 1, Direction.Down)]
    [Arguments(-1, 1, Direction.Down)]

    // Left true branches: (-1,1) and (-1,-1)
    [Arguments(-1, 1, Direction.Left)]
    [Arguments(-1, -1, Direction.Left)]
    public void IsInterCardinalTo_ValuePoint_IPoint_DirectionBranches(int ax, int ay, Direction dir)

        //formatter:on
    {
        var a = new ValuePoint(ax, ay);
        IPoint b = new Point(0, 0);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .BeTrue();
    }

    //formatter:off
    [Test]
    [Arguments(
        1,
        -1,
        Direction.Right,
        true)]
    [Arguments(
        1,
        1,
        Direction.Right,
        true)]
    [Arguments(
        1,
        1,
        Direction.Down,
        true)]
    [Arguments(
        -1,
        1,
        Direction.Down,
        true)]
    [Arguments(
        -1,
        1,
        Direction.Left,
        true)]
    [Arguments(
        -1,
        -1,
        Direction.Left,
        true)]
    public void IsInterCardinalTo_ValuePoint_IPoint_MoreDirections(
            int ax,
            int ay,
            Direction dir,
            bool expected)

        //formatter:on
    {
        var a = new ValuePoint(ax, ay);
        IPoint b = new Point(0, 0);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .Be(expected);
    }

    [Test]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Up,
        true)]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Right,
        false)]
    [Arguments(
        0,
        0,
        2,
        1,
        Direction.Up,
        false)]
    public void IsInterCardinalTo_ValuePoint_Point(
        int ax,
        int ay,
        int bx,
        int by,
        Direction dir,
        bool expected)
    {
        var a = new ValuePoint(ax, ay);
        var b = new Point(bx, by);

        var result = a.IsInterCardinalTo(b, dir);

        result.Should()
              .Be(expected);
    }

    //formatter:on

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.All,
        true)]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Invalid,
        false)]
    public void IsInterCardinalTo_ValuePoint_Point_All_And_Invalid(
            int ax,
            int ay,
            int bx,
            int by,
            Direction dir,
            bool expected)

        //formatter:on
    {
        var a = new ValuePoint(ax, ay);
        var b = new Point(bx, by);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .Be(expected);
    }

    //formatter:off
    [Test]

    // Up true branches: (-1,-1) and (1,-1)
    [Arguments(-1, -1, Direction.Up)]
    [Arguments(1, -1, Direction.Up)]

    // Right true branches: (1,-1) and (1,1)
    [Arguments(1, -1, Direction.Right)]
    [Arguments(1, 1, Direction.Right)]

    // Down true branches: (1,1) and (-1,1)
    [Arguments(1, 1, Direction.Down)]
    [Arguments(-1, 1, Direction.Down)]

    // Left true branches: (-1,1) and (-1,-1)
    [Arguments(-1, 1, Direction.Left)]
    [Arguments(-1, -1, Direction.Left)]
    public void IsInterCardinalTo_ValuePoint_Point_DirectionBranches(int ax, int ay, Direction dir)

        //formatter:on
    {
        var a = new ValuePoint(ax, ay);
        var b = new Point(0, 0);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .BeTrue();
    }

    //formatter:off
    [Test]
    [Arguments(
        1,
        -1,
        Direction.Right,
        true)]
    [Arguments(
        1,
        1,
        Direction.Right,
        true)]
    [Arguments(
        1,
        1,
        Direction.Down,
        true)]
    [Arguments(
        -1,
        1,
        Direction.Down,
        true)]
    [Arguments(
        -1,
        1,
        Direction.Left,
        true)]
    [Arguments(
        -1,
        -1,
        Direction.Left,
        true)]
    public void IsInterCardinalTo_ValuePoint_Point_MoreDirections(
            int ax,
            int ay,
            Direction dir,
            bool expected)

        //formatter:on
    {
        var a = new ValuePoint(ax, ay);
        var b = new Point(0, 0);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Up,
        true)]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.All,
        true)]
    [Arguments(
        0,
        0,
        2,
        1,
        Direction.Up,
        false)]
    public void IsInterCardinalTo_ValuePoint_ValuePoint(
        int ax,
        int ay,
        int bx,
        int by,
        Direction dir,
        bool expected)
    {
        var a = new ValuePoint(ax, ay);
        var b = new ValuePoint(bx, by);

        var result = a.IsInterCardinalTo(b, dir);

        result.Should()
              .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.All,
        true)]
    [Arguments(
        0,
        0,
        1,
        1,
        Direction.Invalid,
        false)]
    public void IsInterCardinalTo_ValuePoint_ValuePoint_All_And_Invalid(
            int ax,
            int ay,
            int bx,
            int by,
            Direction dir,
            bool expected)

        //formatter:on
    {
        var a = new ValuePoint(ax, ay);
        var b = new ValuePoint(bx, by);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .Be(expected);
    }

    //formatter:off
    [Test]

    // Up true branches: (-1,-1) and (1,-1)
    [Arguments(-1, -1, Direction.Up)]
    [Arguments(1, -1, Direction.Up)]

    // Right true branches: (1,-1) and (1,1)
    [Arguments(1, -1, Direction.Right)]
    [Arguments(1, 1, Direction.Right)]

    // Down true branches: (1,1) and (-1,1)
    [Arguments(1, 1, Direction.Down)]
    [Arguments(-1, 1, Direction.Down)]

    // Left true branches: (-1,1) and (-1,-1)
    [Arguments(-1, 1, Direction.Left)]
    [Arguments(-1, -1, Direction.Left)]
    public void IsInterCardinalTo_ValuePoint_ValuePoint_DirectionBranches(int ax, int ay, Direction dir)

        //formatter:on
    {
        var a = new ValuePoint(ax, ay);
        var b = new ValuePoint(0, 0);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .BeTrue();
    }

    //formatter:off
    [Test]
    [Arguments(
        1,
        -1,
        Direction.Right,
        true)]
    [Arguments(
        1,
        1,
        Direction.Right,
        true)]
    [Arguments(
        1,
        1,
        Direction.Down,
        true)]
    [Arguments(
        -1,
        1,
        Direction.Down,
        true)]
    [Arguments(
        -1,
        1,
        Direction.Left,
        true)]
    [Arguments(
        -1,
        -1,
        Direction.Left,
        true)]
    public void IsInterCardinalTo_ValuePoint_ValuePoint_MoreDirections(
            int ax,
            int ay,
            Direction dir,
            bool expected)

        //formatter:on
    {
        var a = new ValuePoint(ax, ay);
        var b = new ValuePoint(0, 0);

        a.IsInterCardinalTo(b, dir)
         .Should()
         .Be(expected);
    }

    [Test]
    public void ManhattanDistanceFrom_IPoint_ShouldThrow_WhenEitherArgNull()
    {
        IPoint a = null!;
        IPoint b = new Point(0, 0);

        var a1 = a;
        var b1 = b;
        Action act1 = () => a1.ManhattanDistanceFrom(b1);

        act1.Should()
            .Throw<NullReferenceException>();

        a = new Point(0, 0);
        b = null!;
        Action act2 = () => a.ManhattanDistanceFrom(b);

        act2.Should()
            .Throw<NullReferenceException>();
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        0,
        0,
        0)] // Same point
    [Arguments(
        0,
        0,
        1,
        0,
        1)] // Horizontal
    [Arguments(
        0,
        0,
        0,
        1,
        1)] // Vertical
    [Arguments(
        0,
        0,
        1,
        1,
        2)] // Diagonal
    [Arguments(
        3,
        4,
        0,
        0,
        7)] // 3-4-5 triangle
    public void ManhattanDistanceFrom_ValuePoint_AllOverloads(
            int x1,
            int y1,
            int x2,
            int y2,
            int expectedDistance)

        //formatter:on
    {
        var vp1 = new ValuePoint(x1, y1);
        var vp2 = new ValuePoint(x2, y2);
        var p2 = new Point(x2, y2);
        IPoint ip2 = new Point(x2, y2);

        vp1.ManhattanDistanceFrom(vp2)
           .Should()
           .Be(expectedDistance);

        vp1.ManhattanDistanceFrom(p2)
           .Should()
           .Be(expectedDistance);

        vp1.ManhattanDistanceFrom(ip2)
           .Should()
           .Be(expectedDistance);

        var p1 = new Point(x1, y1);

        p1.ManhattanDistanceFrom(vp2)
          .Should()
          .Be(expectedDistance);

        IPoint ip1 = new Point(x1, y1);

        ip1.ManhattanDistanceFrom(vp2)
           .Should()
           .Be(expectedDistance);
    }

    [Test]
    public void OffsetTowards_IPoint_IPoint_Should_Offset_Correctly()
    {
        IPoint start = new Point(0, 0);
        IPoint other = new Point(3, 0);

        var result = start.OffsetTowards(other);

        result.Should()
              .BeEquivalentTo(new Point(1, 0));
    }

    [Test]
    public void OffsetTowards_IPoint_Point_Should_Offset_Correctly()
    {
        IPoint start = new Point(0, 0);
        var other = new Point(-3, 0);

        var result = start.OffsetTowards(other);

        result.Should()
              .BeEquivalentTo(new Point(-1, 0));
    }

    [Test]
    public void OffsetTowards_IPoint_ValuePoint_Should_Offset_Correctly()
    {
        IPoint start = new Point(0, 0);
        var other = new ValuePoint(0, -3);

        var result = start.OffsetTowards(other);

        result.Should()
              .BeEquivalentTo(new Point(0, -1));
    }

    [Test]
    public void OffsetTowards_Point_ValuePoint_Should_Offset_Correctly()
    {
        var start = new Point(0, 0);
        var other = new ValuePoint(2, 0);

        var result = start.OffsetTowards(other);

        result.Should()
              .BeEquivalentTo(new Point(1, 0));
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

    [Test]
    public void OffsetTowards_ShouldThrow_WhenEitherParamIsNull()
    {
        IPoint point = null!;
        IPoint other = new Point(1, 1);

        var point1 = point;
        var other1 = other;
        Action act1 = () => point1.OffsetTowards(other1);

        act1.Should()
            .Throw<NullReferenceException>();

        point = new Point(0, 0);
        other = null!;
        Action act2 = () => point.OffsetTowards(other);

        act2.Should()
            .Throw<NullReferenceException>();
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        0,
        1,
        0,
        1)] // Up
    [Arguments(
        0,
        0,
        1,
        0,
        1,
        0)] // Right
    [Arguments(
        0,
        0,
        0,
        -1,
        0,
        -1)] // Down
    [Arguments(
        0,
        0,
        -1,
        0,
        -1,
        0)] // Left
    public void OffsetTowards_ValuePoint_IPoint_Should_Offset_Correctly(
            int startX,
            int startY,
            int otherX,
            int otherY,
            int expectedOffsetX,
            int expectedOffsetY)

        //formatter:on
    {
        var start = new ValuePoint(startX, startY);
        IPoint other = new Point(otherX, otherY);
        var expected = new Point(expectedOffsetX, expectedOffsetY);

        var result = start.OffsetTowards(other);

        result.Should()
              .BeEquivalentTo(expected);
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        0,
        1,
        0,
        1)] // Up
    [Arguments(
        0,
        0,
        1,
        0,
        1,
        0)] // Right
    [Arguments(
        0,
        0,
        0,
        -1,
        0,
        -1)] // Down
    [Arguments(
        0,
        0,
        -1,
        0,
        -1,
        0)] // Left
    public void OffsetTowards_ValuePoint_Point_Should_Offset_Correctly(
            int startX,
            int startY,
            int otherX,
            int otherY,
            int expectedOffsetX,
            int expectedOffsetY)

        //formatter:on
    {
        var start = new ValuePoint(startX, startY);
        var other = new Point(otherX, otherY);
        var expected = new Point(expectedOffsetX, expectedOffsetY);

        var result = start.OffsetTowards(other);

        result.Should()
              .BeEquivalentTo(expected);
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        0,
        1,
        0,
        1)] // Up
    [Arguments(
        0,
        0,
        1,
        0,
        1,
        0)] // Right
    [Arguments(
        0,
        0,
        0,
        -1,
        0,
        -1)] // Down
    [Arguments(
        0,
        0,
        -1,
        0,
        -1,
        0)] // Left
    public void OffsetTowards_ValuePoint_ValuePoint_Should_Offset_Correctly(
            int startX,
            int startY,
            int otherX,
            int otherY,
            int expectedOffsetX,
            int expectedOffsetY)

        //formatter:on
    {
        var start = new ValuePoint(startX, startY);
        var other = new ValuePoint(otherX, otherY);
        var expected = new Point(expectedOffsetX, expectedOffsetY);

        var result = start.OffsetTowards(other);

        result.Should()
              .BeEquivalentTo(expected);
    }

    [Test]
    public void OrderByAngle_Generic_Should_Order_Points_By_Angle_Relative_To_Origin()
    {
        var origin = new TestPoint(0, 0);

        var points = new List<TestPoint>
        {
            new(1, 0),
            new(0, 1),
            new(0, -1),
            new(-1, 0),
            new(1, 1)
        };

        var result = points.OrderByAngle(origin)
                           .ToList();

        result.Select(p => (p.X, p.Y))
              .Should()
              .ContainInOrder(
                  new List<(int, int)>
                  {
                      (1, 1),
                      (0, 1),
                      (-1, 0),
                      (0, -1),
                      (1, 0)
                  });
    }

    [Test]
    public void OrderByAngle_Should_Order_Points_By_Angle_Relative_To_Origin_Point_Overload()
    {
        var origin = new Point(0, 0);

        var points = new List<Point>
        {
            new(1, 0),
            new(0, 1),
            new(0, -1),
            new(-1, 0),
            new(1, 1)
        };

        var result = points.OrderByAngle(origin)
                           .ToList();

        result.Should()
              .ContainInOrder(
                  new List<Point>
                  {
                      new(1, 1),
                      new(0, 1),
                      new(-1, 0),
                      new(0, -1),
                      new(1, 0)
                  });
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        0,
        0,
        new[]
        {
            0,
            0
        })] // Same point
    [Arguments(
        0,
        0,
        3,
        0,
        new[]
        {
            0,
            0,
            1,
            0,
            2,
            0,
            3,
            0
        })] // Horizontal
    public void RayTraceTo_IPoint_ToPoint(
            int startX,
            int startY,
            int endX,
            int endY,
            params IEnumerable<int> expected)

        //formatter:on
    {
        IPoint start = new Point(startX, startY);
        var end = new Point(endX, endY);

        var result = start.RayTraceTo(end)
                          .ToList();

        result.Should()
              .BeEquivalentTo(
                  expected.Chunk(2)
                          .Select(p => new Point(p[0], p[1])),
                  options => options.WithStrictOrdering());
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

    [Test]
    public void RayTraceTo_ShouldThrow_WhenEndIsNull_IPointOverload()
    {
        IPoint start = new Point(0, 0);
        IPoint end = null!;

        var act = () => start.RayTraceTo(end)
                             .ToList();

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void RayTraceTo_ShouldThrow_WhenStartIsNull_IPointOverload()
    {
        IPoint start = null!;
        IPoint end = new Point(1, 1);

        var act = () => start.RayTraceTo(end)
                             .ToList();

        act.Should()
           .Throw<NullReferenceException>();
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        0,
        0,
        new[]
        {
            0,
            0
        })] // Same point
    [Arguments(
        0,
        0,
        3,
        0,
        new[]
        {
            0,
            0,
            1,
            0,
            2,
            0,
            3,
            0
        })] // Horizontal
    public void RayTraceTo_ValuePoint_ToIPoint(
            int startX,
            int startY,
            int endX,
            int endY,
            params IEnumerable<int> expected)

        //formatter:on
    {
        var start = new ValuePoint(startX, startY);
        IPoint end = new Point(endX, endY);

        var result = start.RayTraceTo(end)
                          .ToList();

        result.Should()
              .BeEquivalentTo(
                  expected.Chunk(2)
                          .Select(p => new Point(p[0], p[1])),
                  options => options.WithStrictOrdering());
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        0,
        0,
        new[]
        {
            0,
            0
        })] // Same point
    [Arguments(
        0,
        0,
        3,
        0,
        new[]
        {
            0,
            0,
            1,
            0,
            2,
            0,
            3,
            0
        })] // Horizontal
    [Arguments(
        0,
        0,
        0,
        3,
        new[]
        {
            0,
            0,
            0,
            1,
            0,
            2,
            0,
            3
        })] // Vertical
    [Arguments(
        0,
        0,
        2,
        2,
        new[]
        {
            0,
            0,
            0,
            1,
            1,
            1,
            1,
            2,
            2,
            2
        })] // Diagonal
    public void RayTraceTo_ValuePoint_ToPoint(
            int startX,
            int startY,
            int endX,
            int endY,
            params IEnumerable<int> expected)

        //formatter:on
    {
        var start = new ValuePoint(startX, startY);
        var end = new Point(endX, endY);

        var result = start.RayTraceTo(end)
                          .ToList();

        result.Should()
              .BeEquivalentTo(
                  expected.Chunk(2)
                          .Select(p => new Point(p[0], p[1])),
                  options => options.WithStrictOrdering());
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
    public void SpiralSearch_ShouldThrow_WhenPointIsNull_IPointOverload()
    {
        IPoint start = null!;

        var act = () => start.SpiralSearch()
                             .ToList();

        act.Should()
           .Throw<NullReferenceException>();
    }

    [Test]
    public void SpiralSearch_ValuePoint_GeneratesCorrectSpiral()
    {
        var start = new ValuePoint(0, 0);

        var result = start.SpiralSearch(1)
                          .Take(5)
                          .ToList();

        result[0]
            .Should()
            .Be(new Point(0, 0)); // Center

        result.Skip(1)
              .Should()
              .HaveCount(4); // 4 points at radius 1
    }

    //formatter:off
    [Test]
    [Arguments(
        Direction.Up,
        new[]
        {
            0,
            0,
            -1,
            1,
            2,
            1,
            1,
            2
        })] // Y asc then X asc
    [Arguments(
        Direction.Right,
        new[]
        {
            2,
            1,
            1,
            2,
            0,
            0,
            -1,
            1
        })] // X desc then Y desc
    [Arguments(
        Direction.Down,
        new[]
        {
            1,
            2,
            2,
            1,
            -1,
            1,
            0,
            0
        })] // Y desc then X desc
    [Arguments(
        Direction.Left,
        new[]
        {
            -1,
            1,
            0,
            0,
            1,
            2,
            2,
            1
        })] // X asc then Y asc
    public void WithConsistentDirectionBias_Generic_AllDirections(Direction direction, params IEnumerable<int> expectedOrder)

        //formatter:on
    {
        var pts = new List<TestPoint>
        {
            new(0, 0),
            new(1, 2),
            new(2, 1),
            new(-1, 1)
        };

        var ordered = pts.WithConsistentDirectionBias(direction)
                         .ToList();

        ordered.Select(p => (p.X, p.Y))
               .Should()
               .ContainInOrder(
                   expectedOrder.Chunk(2)
                                .Select(p => (p[0], p[1])));
    }

    [Test]
    public void WithConsistentDirectionBias_Generic_Should_Order_And_Throw_For_Invalid()
    {
        var pts = new List<TestPoint>
        {
            new(2, 2),
            new(1, 3),
            new(3, 1)
        };

        var up = pts.WithConsistentDirectionBias(Direction.Up)
                    .ToList();

        up.Select(p => (p.X, p.Y))
          .Should()
          .ContainInOrder((3, 1), (2, 2), (1, 3));

        IEnumerable<TestPoint> nullSeq = null!;
        #pragma warning disable CA1806

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        Action nullAct = () => nullSeq.WithConsistentDirectionBias(Direction.Up)
                                      .ToList();
        #pragma warning restore CA1806
        nullAct.Should()
               .Throw<ArgumentNullException>();

        #pragma warning disable CA1806

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        Action bad = () => pts.WithConsistentDirectionBias(Direction.Invalid)
                              .ToList();
        #pragma warning restore CA1806
        bad.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void WithConsistentDirectionBias_Point_NullCheck()
    {
        IEnumerable<Point> nullPoints = null!;

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        #pragma warning disable CA1806
        Action act = () => nullPoints.WithConsistentDirectionBias(Direction.Up)
                                     .ToList();
        #pragma warning restore CA1806
        act.Should()
           .Throw<ArgumentNullException>();
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

    //formatter:off
    [Test]
    [Arguments(Direction.Invalid)]
    [Arguments(Direction.All)]
    public void WithConsistentDirectionBias_ShouldThrow_WhenDirectionInvalidOrAll(Direction direction)

        //formatter:on
    {
        var points = new List<Point>
        {
            new(1, 1),
            new(0, 0)
        };

        var act = () => points.WithConsistentDirectionBias(direction)
                              .ToList();

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    //formatter:off
    [Test]
    [Arguments(
        Direction.Up,
        new[]
        {
            0,
            0,
            3,
            1,
            1,
            3
        })] // Y asc
    [Arguments(
        Direction.Right,
        new[]
        {
            3,
            1,
            1,
            3,
            0,
            0
        })] // X desc
    [Arguments(
        Direction.Down,
        new[]
        {
            1,
            3,
            3,
            1,
            0,
            0
        })] // Y desc
    [Arguments(
        Direction.Left,
        new[]
        {
            0,
            0,
            1,
            3,
            3,
            1
        })] // X asc
    public void WithDirectionBias_Generic_BasicDirections(Direction direction, params IEnumerable<int> expectedOrder)

        //formatter:on
    {
        var pts = new List<TestPoint>
        {
            new(0, 0),
            new(1, 3),
            new(3, 1)
        };

        var ordered = pts.WithDirectionBias(direction)
                         .ToList();

        ordered.Select(p => (p.X, p.Y))
               .Should()
               .ContainInOrder(
                   expectedOrder.Chunk(2)
                                .Select(p => (p[0], p[1])));
    }

    [Test]
    public void WithDirectionBias_Generic_Should_Order_And_Throw_For_Invalid()
    {
        var pts = new List<TestPoint>
        {
            new(2, 2),
            new(1, 3),
            new(3, 1)
        };

        var right = pts.WithDirectionBias(Direction.Right)
                       .ToList();

        right.Select(p => (p.X, p.Y))
             .Should()
             .ContainInOrder((3, 1), (2, 2), (1, 3));

        IEnumerable<TestPoint> nullSeq = null!;

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        #pragma warning disable CA1806
        Action nullAct = () => nullSeq.WithDirectionBias(Direction.Left)
                                      .ToList();
        #pragma warning restore CA1806
        nullAct.Should()
               .Throw<ArgumentNullException>();

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        #pragma warning disable CA1806
        Action bad = () => pts.WithDirectionBias(Direction.All)
                              .ToList();
        #pragma warning restore CA1806
        bad.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void WithDirectionBias_Point_NullCheck()
    {
        IEnumerable<Point> nullPoints = null!;
        #pragma warning disable CA1806

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        Action act = () => nullPoints.WithDirectionBias(Direction.Up)
                                     .ToList();
        #pragma warning restore CA1806
        act.Should()
           .Throw<ArgumentNullException>();
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

    //formatter:off
    [Test]
    [Arguments(Direction.Invalid)]
    [Arguments(Direction.All)]
    public void WithDirectionBias_ShouldThrow_WhenDirectionInvalidOrAll(Direction direction)

        //formatter:on
    {
        var points = new List<Point>
        {
            new(1, 1),
            new(0, 0)
        };

        var act = () => points.WithDirectionBias(direction)
                              .ToList();

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    private sealed class TestPoint : IPoint
    {
        public int X { get; }
        public int Y { get; }

        public TestPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}