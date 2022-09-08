using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Definitions;

namespace Chaos.Geometry.Extensions;

public static class PointExtensions
{
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
            Direction.Right => new Point(point.X + 1, point.Y),
            Direction.Down  => new Point(point.X, point.Y + 1),
            Direction.Left  => new Point(point.X - 1, point.Y),
            _               => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

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
    /// Orders points by their X or Y values, based on the direction given.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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

    /// <summary>
    ///     Creates an enumerable list of points representing a path between two given points, and returns it.
    /// </summary>
    /// <param name="start">Starting point for the creation of the path.</param>
    /// <param name="end">Ending point for the creation of the path.</param>
    public static IEnumerable<Point> GetDirectPath(this IPoint start, IPoint end)
    {
        var current = Point.From(start);

        yield return current;

        while (current != end)
        {
            current = start.OffsetTowards(end);

            yield return current;
        }

        yield return Point.From(end);
    }

    /// <summary>
    ///     Retreives a list of diagonal points in relevance to the user, with an optional distance and direction.
    ///     Direction.All is optional. Direction.Invalid direction returns empty list.
    /// </summary>
    public static IEnumerable<Point> GetInterCardinalIoints(this IPoint start, int radius = 1, Direction direction = Direction.All)
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

    public static Point OffsetTowards(this IPoint point, IPoint other)
    {
        if (point == null)
            throw new ArgumentNullException(nameof(point));

        if (other == null)
            throw new ArgumentNullException(nameof(other));

        var direction = other.DirectionalRelationTo(point);

        return point.DirectionalOffset(direction);
    }

    public static IEnumerable<Point> SpiralSearch(this IPoint point, int maxRadius = byte.MaxValue)
    {
        var currentPoint = Point.From(point);
        var radius = 1;

        for (; radius <= maxRadius; radius++)
        {
            currentPoint = currentPoint.DirectionalOffset(Direction.Up);

            //travel from north to east
            while (point.X != currentPoint.X)
            {
                currentPoint = new Point(currentPoint.X + 1, currentPoint.Y + 1);

                yield return currentPoint;
            }

            //travel from east to south
            while (point.Y != currentPoint.Y)
            {
                currentPoint = new Point(currentPoint.X - 1, currentPoint.Y + 1);

                yield return currentPoint;
            }

            //travel from south to west
            while (point.X != currentPoint.X)
            {
                currentPoint = new Point(currentPoint.X - 1, currentPoint.Y - 1);

                yield return currentPoint;
            }

            //travel from west to north
            while (point.Y != currentPoint.Y)
            {
                currentPoint = new Point(currentPoint.X + 1, currentPoint.Y - 1);

                yield return currentPoint;
            }
        }
    }
}