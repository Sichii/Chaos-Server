using System.Runtime.CompilerServices;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Extensions.Geometry;

public static class PointExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<Point> ConalSearch(this IPoint point, Direction direction, int maxDistance)
    {
        if (direction == Direction.Invalid)
            throw new ArgumentOutOfRangeException(nameof(direction), "Direction cannot be invalid");

        foreach (var edgePair in point.GetInterCardinalPoints(direction, maxDistance).Chunk(2))
        {
            var edge1 = edgePair[0];
            var edge2 = edgePair[1];

            foreach (var pt in edge1.GetDirectPath(edge2))
                yield return pt;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static Point DirectionalOffset(this IPoint point, Direction direction, int distance = 1)
    {
        if (point == null)
            throw new ArgumentNullException(nameof(point));

        if (direction == Direction.Invalid)
            throw new ArgumentOutOfRangeException(nameof(direction));

        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return direction switch
        {
            Direction.Up    => new Point(point.X, point.Y - distance),
            Direction.Right => new Point(point.X + distance, point.Y),
            Direction.Down  => new Point(point.X, point.Y + distance),
            Direction.Left  => new Point(point.X - distance, point.Y),
            _               => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static Direction DirectionalRelationTo(this IPoint point, IPoint other)
    {
        if (point == null)
            throw new ArgumentNullException(nameof(point));

        if (other == null)
            throw new ArgumentNullException(nameof(other));

        var direction = Direction.Invalid;
        var degree = 0;

        if (point.Y < other.Y)
        {
            degree = other.Y - point.Y;
            direction = Direction.Up;
        } else if (point.Y > other.Y)
        {
            degree = point.Y - other.Y;
            direction = Direction.Down;
        }

        if (point.X > other.X)
        {
            var xDegree = point.X - other.X;

            //if xdegree is higher, the point is more to the right of the other point
            //if xdegree is equal, there's a 50% chance to get the vertical or horizonal direction
            if ((xDegree > degree) || ((xDegree == degree) && (Random.Shared.Next(0, 101) < 50)))
                direction = Direction.Right;
        } else if (point.X < other.X)
        {
            var xDegree = other.X - point.X;

            //if xdegree is higher, the point is more to the right of the other point
            //if xdegree is equal, there's a 50% chance to get the vertical or horizonal direction
            if ((xDegree > degree) || ((xDegree == degree) && (Random.Shared.Next(0, 101) < 50)))
                direction = Direction.Left;
        }

        return direction;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static int DistanceFrom(this IPoint point, IPoint other)
    {
        if (point == null)
            throw new ArgumentNullException(nameof(point));

        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);
    }

    /// <summary>
    ///     Retreives a list of points in a line from the user, with an option for distance and direction. Direction.All is
    ///     optional. Direction.Invalid direction returns empty list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<Point> GetCardinalPoints(this IPoint start, int radius = 1, Direction direction = Direction.All)
    {
        if (direction == Direction.Invalid)
            yield break;

        for (var i = 1; i <= radius; i++)
            switch (direction)
            {
                case Direction.All:
                    yield return start.DirectionalOffset(Direction.Up, i);
                    yield return start.DirectionalOffset(Direction.Right, i);
                    yield return start.DirectionalOffset(Direction.Down, i);
                    yield return start.DirectionalOffset(Direction.Left, i);

                    break;
                default:
                    yield return start.DirectionalOffset(direction, i);

                    break;
            }
    }

    /// <summary>
    ///     Creates an enumerable list of points representing a path between two given points, and returns it.
    /// </summary>
    /// <param name="start">Starting point for the creation of the path.</param>
    /// <param name="end">Ending point for the creation of the path.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<Point> GetDirectPath(this IPoint start, IPoint end)
    {
        var current = Point.From(start);

        while (current != end)
        {
            yield return current;

            current = current.OffsetTowards(end);
        }

        yield return Point.From(end);
    }

    /// <summary>
    ///     Retreives a list of diagonal points in relevance to the user, with an optional distance and direction.
    ///     Direction.All is optional. Direction.Invalid direction returns empty list.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<Point> GetInterCardinalPoints(this IPoint start, Direction direction = Direction.All, int radius = 1)
    {
        if (direction == Direction.Invalid)
            yield break;

        for (var i = 1; i <= radius; i++)
            switch (direction)
            {
                case Direction.Up:
                    yield return new Point(start.X - i, start.Y - i);
                    yield return new Point(start.X + i, start.Y - i);

                    break;
                case Direction.Right:
                    yield return new Point(start.X + i, start.Y - i);
                    yield return new Point(start.X + i, start.Y + i);

                    break;
                case Direction.Down:
                    yield return new Point(start.X + i, start.Y + i);
                    yield return new Point(start.X - i, start.Y + i);

                    break;
                case Direction.Left:
                    yield return new Point(start.X - i, start.Y - i);
                    yield return new Point(start.X - i, start.Y + i);

                    break;
                case Direction.All:
                    yield return new Point(start.X - i, start.Y - i);
                    yield return new Point(start.X + i, start.Y - i);
                    yield return new Point(start.X + i, start.Y + i);
                    yield return new Point(start.X - i, start.Y + i);

                    break;
                default:
                    yield break;
            }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static Point OffsetTowards(this IPoint point, IPoint other)
    {
        if (point == null)
            throw new ArgumentNullException(nameof(point));

        if (other == null)
            throw new ArgumentNullException(nameof(other));

        var direction = other.DirectionalRelationTo(point);

        return point.DirectionalOffset(direction);
    }

    /// <summary>
    ///     Lazily generates points between two points. <br />
    ///     https://playtechs.blogspot.com/2007/03/raytracing-on-grid.html
    /// </summary>
    /// <param name="point">A point.</param>
    /// <param name="other">Another point.</param>
    /// <returns>
    ///     <see cref="IEnumerable{T}" /> of <see cref="Point" /><br />
    ///     An enumeration of points that a line drawn from <paramref name="point" /> to the <paramref name="other" /> would
    ///     cross over.
    /// </returns>
    /// <exception cref="ArgumentNullException">point</exception>
    /// <exception cref="ArgumentNullException">other</exception>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static IEnumerable<Point> RayTraceTo(this IPoint point, IPoint other)
    {
        var x0 = point.X;
        var y0 = point.Y;
        var x1 = other.X;
        var y1 = other.Y;
        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y1 - y0);
        var x = x0;
        var y = y0;
        var n = 1 + dx + dy;
        var xOffset = x1 > x0 ? 1 : -1;
        var yOffset = y1 > y0 ? 1 : -1;
        var error = dx - dy;
        dx *= 2;
        dy *= 2;

        for (; n > 0; --n)
        {
            yield return new Point(x, y);

            if (error > 0)
            {
                x += xOffset;
                error -= dy;
            } else
            {
                y += yOffset;
                error += dx;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<Point> SpiralSearch(this IPoint point, int maxRadius = byte.MaxValue)
    {
        var currentPoint = Point.From(point);
        var radius = 1;

        yield return currentPoint;

        for (; radius <= maxRadius; radius++)
        {
            currentPoint = currentPoint.DirectionalOffset(Direction.Up);

            //travel from north to east
            while (point.Y != currentPoint.Y)
            {
                currentPoint = new Point(currentPoint.X + 1, currentPoint.Y + 1);

                yield return currentPoint;
            }

            //travel from east to south
            while (point.X != currentPoint.X)
            {
                currentPoint = new Point(currentPoint.X - 1, currentPoint.Y + 1);

                yield return currentPoint;
            }

            //travel from south to west
            while (point.Y != currentPoint.Y)
            {
                currentPoint = new Point(currentPoint.X - 1, currentPoint.Y - 1);

                yield return currentPoint;
            }

            //travel from west to north
            while (point.X != currentPoint.X)
            {
                currentPoint = new Point(currentPoint.X + 1, currentPoint.Y - 1);

                yield return currentPoint;
            }
        }
    }

    /// <summary>
    ///     Orders points by their X or Y values, based on the direction given.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<Point> WithDirectionBias(this IEnumerable<Point> points, Direction direction)
    {
        if (points == null)
            throw new ArgumentNullException(nameof(points));

        if (direction == Direction.Invalid)
            throw new ArgumentOutOfRangeException(nameof(direction));

        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return direction switch
        {
            Direction.Up    => points.OrderBy(p => p.Y),
            Direction.Right => points.OrderByDescending(p => p.X),
            Direction.Down  => points.OrderByDescending(p => p.Y),
            Direction.Left  => points.OrderBy(p => p.X),
            _               => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}