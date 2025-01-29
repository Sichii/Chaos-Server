#region
using System.Runtime.CompilerServices;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Geometry.EqualityComparers;
#endregion

namespace Chaos.Extensions.Geometry;

/// <summary>
///     Provides extension methods for <see cref="Chaos.Geometry.Abstractions.IPoint" />.
/// </summary>
public static class PointExtensions
{
    #region Point GenerateCardinalPoints
    /// <inheritdoc cref="GenerateCardinalPoints(IPoint, Direction, int)" />
    public static IEnumerable<Point> GenerateCardinalPoints(this ValuePoint start, Direction direction = Direction.All, int radius = 1)
        => GenerateCardinalPoints((Point)start, direction, radius);

    /// <inheritdoc cref="GenerateCardinalPoints(IPoint, Direction, int)" />
    public static IEnumerable<Point> GenerateCardinalPoints(this Point start, Direction direction = Direction.All, int radius = 1)
    {
        if (direction == Direction.Invalid)
            yield break;

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(radius);

        for (var i = 1; i <= radius; i++)
            if (direction == Direction.All)
            {
                yield return start.DirectionalOffset(Direction.Up, i);
                yield return start.DirectionalOffset(Direction.Right, i);
                yield return start.DirectionalOffset(Direction.Down, i);
                yield return start.DirectionalOffset(Direction.Left, i);
            } else
                yield return start.DirectionalOffset(direction, i);
    }

    /// <summary>
    ///     Lazily generates an enumeration of points in a line from the user, with an option for distance and direction.
    ///     Direction.All is optional. Direction.Invalid direction returns empty list.
    /// </summary>
    /// <param name="start">
    /// </param>
    /// <param name="direction">
    /// </param>
    /// <param name="radius">
    ///     The max distance to generate points
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="radius" /> must be positive
    /// </exception>
    /// <remarks>
    ///     Assumes <see cref="Chaos.Geometry.Abstractions.Definitions.Direction.Up" /> is equivalent to the cardinal direction
    ///     "North", this method will generate points in all 4 cardinal directions. Points will be generated 1 radius at a
    ///     time, clock-wise.
    /// </remarks>
    /// <example>
    ///     <code>
    /// //generates points in a counter clockwise spiral around the start
    /// //will generate the 4 points immediately around the start
    /// var points = new Point(0, 0).GenerateCardinalPoints();
    /// </code>
    /// </example>
    public static IEnumerable<Point> GenerateCardinalPoints(this IPoint start, Direction direction = Direction.All, int radius = 1)
    {
        ArgumentNullException.ThrowIfNull(start);

        if (direction == Direction.Invalid)
            yield break;

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(radius);

        for (var i = 1; i <= radius; i++)
            if (direction == Direction.All)
            {
                yield return start.DirectionalOffset(Direction.Up, i);
                yield return start.DirectionalOffset(Direction.Right, i);
                yield return start.DirectionalOffset(Direction.Down, i);
                yield return start.DirectionalOffset(Direction.Left, i);
            } else
                yield return start.DirectionalOffset(direction, i);
    }
    #endregion

    #region Point GenerateIntercardinalPoints
    /// <inheritdoc cref="GenerateIntercardinalPoints(IPoint, Direction, int)" />
    public static IEnumerable<Point> GenerateIntercardinalPoints(this ValuePoint start, Direction direction = Direction.All, int radius = 1)
        => GenerateIntercardinalPoints((Point)start, direction, radius);

    /// <inheritdoc cref="GenerateIntercardinalPoints(IPoint, Direction, int)" />
    public static IEnumerable<Point> GenerateIntercardinalPoints(this Point start, Direction direction = Direction.All, int radius = 1)
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

    /// <summary>
    ///     Lazily generates an enumeration of diagonal points in relevance to the user, with an optional distance and
    ///     direction. Direction.All is optional. Direction.Invalid direction returns an empty enumeration
    /// </summary>
    /// <param name="start">
    /// </param>
    /// <param name="direction">
    ///     The general direction to generate points for. See remarks.
    /// </param>
    /// <param name="radius">
    ///     The range in which to generate points
    /// </param>
    /// <remarks>
    ///     Assuming <see cref="Chaos.Geometry.Abstractions.Definitions.Direction.Up" /> is equivalent to the cardinal
    ///     direction "North", this method will generate points in the inter-cardinal directions "North-East", "South-East",
    ///     "South-West", and "North-West". Points will be generated 1 radius at a time, clock-wise. Optionally, you can choose
    ///     a cardinal <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> to generate points for the 2
    ///     inter-cardinal directions that share the given cardinal direction.
    /// </remarks>
    /// <example>
    ///     <code>
    ///     //this call will generate points in the inter-cardinal directions "North-West" and "North-East"
    ///     //4 points will be generated, 2 in each inter-cardinal direction
    ///     //the points will be diagonal to the original point 
    ///     var points = new Point(0, 0).GenerateInterCardinalPoints(Direction.Up, 2);
    /// </code>
    ///     <code>
    ///     //this call will generate points in the inter-cardinal directions "South-West" and "South-East"
    ///     //10 points will be generated, 5 in each inter-cardinal direction
    ///     //the points will be diagonal to the original point 
    ///     var points = new Point(0, 0).GenerateInterCardinalPoints(Direction.Down, 5);
    /// </code>
    ///     <code>
    ///     //this call will generate points in all inter-cardinal directions
    ///     //12 points will be generated, 3 in each inter-cardinal direction
    ///     //the points will be diagonal to the original point 
    ///     var points = new Point(0, 0).GenerateInterCardinalPoints(Direction.All, 3);
    /// </code>
    /// </example>
    public static IEnumerable<Point> GenerateIntercardinalPoints(this IPoint start, Direction direction = Direction.All, int radius = 1)
    {
        ArgumentNullException.ThrowIfNull(start);

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
    #endregion

    #region Point GetDirectPath
    /// <inheritdoc cref="GetDirectPath(IPoint, IPoint)" />
    public static IEnumerable<Point> GetDirectPath(this ValuePoint start, Point end) => GetDirectPath((Point)start, end);

    /// <inheritdoc cref="GetDirectPath(IPoint, IPoint)" />
    public static IEnumerable<Point> GetDirectPath(this ValuePoint start, IPoint end) => GetDirectPath((Point)start, end);

    /// <inheritdoc cref="GetDirectPath(IPoint, IPoint)" />
    public static IEnumerable<Point> GetDirectPath(this Point start, Point end)
    {
        var current = start;

        yield return current;

        while (!current.Equals(end))
        {
            current = current.OffsetTowards(end);

            yield return Point.From(current);
        }
    }

    /// <inheritdoc cref="GetDirectPath(IPoint, IPoint)" />
    public static IEnumerable<Point> GetDirectPath(this Point start, IPoint end)
    {
        var current = start;

        yield return current;

        while (!current.Equals(end))
        {
            current = current.OffsetTowards(end);

            yield return Point.From(current);
        }
    }

    /// <inheritdoc cref="GetDirectPath(IPoint, IPoint)" />
    public static IEnumerable<Point> GetDirectPath(this IPoint start, Point end)
    {
        var current = Point.From(start);

        yield return current;

        while (!current.Equals(end))
        {
            current = current.OffsetTowards(end);

            yield return Point.From(current);
        }
    }

    /// <summary>
    ///     Creates an enumerable list of points representing a path between two given points, and returns it.
    /// </summary>
    /// <param name="start">
    ///     Starting point for the creation of the path
    /// </param>
    /// <param name="end">
    ///     Ending point for the creation of the path
    /// </param>
    /// <remarks>
    ///     Does not return the start point, only the points between the start and end, as well as the end point itself
    /// </remarks>
    public static IEnumerable<Point> GetDirectPath(this IPoint start, IPoint end)
    {
        ArgumentNullException.ThrowIfNull(start);

        ArgumentNullException.ThrowIfNull(end);

        var current = Point.From(start);

        yield return current;

        while (!current.Equals(end))
        {
            current = current.OffsetTowards(end);

            yield return Point.From(current);
        }
    }
    #endregion

    #region Point IsInterCardinalTo
    /// <inheritdoc cref="IsInterCardinalTo(Point, Point, Direction)" />
    public static bool IsInterCardinalTo(this ValuePoint point, ValuePoint other, Direction direction)
    {
        var xDiff = point.X - other.X;
        var yDiff = point.Y - other.Y;

        if (Math.Abs(xDiff) != Math.Abs(yDiff))
            return false;

        return direction switch
        {
            Direction.Up    => ((xDiff < 0) && (yDiff < 0)) || ((xDiff > 0) && (yDiff < 0)),
            Direction.Right => ((xDiff > 0) && (yDiff < 0)) || ((xDiff > 0) && (yDiff > 0)),
            Direction.Down  => ((xDiff > 0) && (yDiff > 0)) || ((xDiff < 0) && (yDiff > 0)),
            Direction.Left  => ((xDiff < 0) && (yDiff > 0)) || ((xDiff < 0) && (yDiff < 0)),
            Direction.All   => true,
            _               => false
        };
    }

    /// <inheritdoc cref="IsInterCardinalTo(Point, Point, Direction)" />
    public static bool IsInterCardinalTo(this ValuePoint point, Point other, Direction direction)
    {
        var xDiff = point.X - other.X;
        var yDiff = point.Y - other.Y;

        if (Math.Abs(xDiff) != Math.Abs(yDiff))
            return false;

        return direction switch
        {
            Direction.Up    => ((xDiff < 0) && (yDiff < 0)) || ((xDiff > 0) && (yDiff < 0)),
            Direction.Right => ((xDiff > 0) && (yDiff < 0)) || ((xDiff > 0) && (yDiff > 0)),
            Direction.Down  => ((xDiff > 0) && (yDiff > 0)) || ((xDiff < 0) && (yDiff > 0)),
            Direction.Left  => ((xDiff < 0) && (yDiff > 0)) || ((xDiff < 0) && (yDiff < 0)),
            Direction.All   => true,
            _               => false
        };
    }

    /// <inheritdoc cref="IsInterCardinalTo(Point, Point, Direction)" />
    public static bool IsInterCardinalTo(this ValuePoint point, IPoint other, Direction direction)
    {
        var xDiff = point.X - other.X;
        var yDiff = point.Y - other.Y;

        if (Math.Abs(xDiff) != Math.Abs(yDiff))
            return false;

        return direction switch
        {
            Direction.Up    => ((xDiff < 0) && (yDiff < 0)) || ((xDiff > 0) && (yDiff < 0)),
            Direction.Right => ((xDiff > 0) && (yDiff < 0)) || ((xDiff > 0) && (yDiff > 0)),
            Direction.Down  => ((xDiff > 0) && (yDiff > 0)) || ((xDiff < 0) && (yDiff > 0)),
            Direction.Left  => ((xDiff < 0) && (yDiff > 0)) || ((xDiff < 0) && (yDiff < 0)),
            Direction.All   => true,
            _               => false
        };
    }

    /// <inheritdoc cref="IsInterCardinalTo(IPoint, IPoint, Direction)" />
    public static bool IsInterCardinalTo(this Point point, Point other, Direction direction)
    {
        var xDiff = point.X - other.X;
        var yDiff = point.Y - other.Y;

        if (Math.Abs(xDiff) != Math.Abs(yDiff))
            return false;

        return direction switch
        {
            Direction.Up    => ((xDiff < 0) && (yDiff < 0)) || ((xDiff > 0) && (yDiff < 0)),
            Direction.Right => ((xDiff > 0) && (yDiff < 0)) || ((xDiff > 0) && (yDiff > 0)),
            Direction.Down  => ((xDiff > 0) && (yDiff > 0)) || ((xDiff < 0) && (yDiff > 0)),
            Direction.Left  => ((xDiff < 0) && (yDiff > 0)) || ((xDiff < 0) && (yDiff < 0)),
            Direction.All   => true,
            _               => false
        };
    }

    /// <inheritdoc cref="IsInterCardinalTo(Point, Point, Direction)" />
    public static bool IsInterCardinalTo(this Point point, IPoint other, Direction direction)
    {
        var xDiff = point.X - other.X;
        var yDiff = point.Y - other.Y;

        if (Math.Abs(xDiff) != Math.Abs(yDiff))
            return false;

        return direction switch
        {
            Direction.Up    => ((xDiff < 0) && (yDiff < 0)) || ((xDiff > 0) && (yDiff < 0)),
            Direction.Right => ((xDiff > 0) && (yDiff < 0)) || ((xDiff > 0) && (yDiff > 0)),
            Direction.Down  => ((xDiff > 0) && (yDiff > 0)) || ((xDiff < 0) && (yDiff > 0)),
            Direction.Left  => ((xDiff < 0) && (yDiff > 0)) || ((xDiff < 0) && (yDiff < 0)),
            Direction.All   => true,
            _               => false
        };
    }

    /// <inheritdoc cref="IsInterCardinalTo(Point, Point, Direction)" />
    public static bool IsInterCardinalTo(this IPoint point, Point other, Direction direction)
    {
        var xDiff = point.X - other.X;
        var yDiff = point.Y - other.Y;

        if (Math.Abs(xDiff) != Math.Abs(yDiff))
            return false;

        return direction switch
        {
            Direction.Up    => ((xDiff < 0) && (yDiff < 0)) || ((xDiff > 0) && (yDiff < 0)),
            Direction.Right => ((xDiff > 0) && (yDiff < 0)) || ((xDiff > 0) && (yDiff > 0)),
            Direction.Down  => ((xDiff > 0) && (yDiff > 0)) || ((xDiff < 0) && (yDiff > 0)),
            Direction.Left  => ((xDiff < 0) && (yDiff > 0)) || ((xDiff < 0) && (yDiff < 0)),
            Direction.All   => true,
            _               => false
        };
    }

    /// <summary>
    ///     Determines if this point is on either intercardinal diagonal in relation to another point, in the given direction
    /// </summary>
    /// <param name="point">
    ///     The point to test
    /// </param>
    /// <param name="other">
    ///     The point in which directions are based on
    /// </param>
    /// <param name="direction">
    ///     The direction between the 2 intercardinals to check
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if this point is on an intercardinal diagonal in relation to the other point in the given direction, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public static bool IsInterCardinalTo(this IPoint point, IPoint other, Direction direction)
    {
        ArgumentNullException.ThrowIfNull(point);

        ArgumentNullException.ThrowIfNull(other);

        var xDiff = point.X - other.X;
        var yDiff = point.Y - other.Y;

        if (Math.Abs(xDiff) != Math.Abs(yDiff))
            return false;

        return direction switch
        {
            Direction.Up    => ((xDiff < 0) && (yDiff < 0)) || ((xDiff > 0) && (yDiff < 0)),
            Direction.Right => ((xDiff > 0) && (yDiff < 0)) || ((xDiff > 0) && (yDiff > 0)),
            Direction.Down  => ((xDiff > 0) && (yDiff > 0)) || ((xDiff < 0) && (yDiff > 0)),
            Direction.Left  => ((xDiff < 0) && (yDiff > 0)) || ((xDiff < 0) && (yDiff < 0)),
            Direction.All   => true,
            _               => false
        };
    }
    #endregion

    #region Point ManhattanDistanceFrom
    /// <inheritdoc cref="ManhattanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ManhattanDistanceFrom(this ValuePoint point, ValuePoint other)
        => Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);

    /// <inheritdoc cref="ManhattanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ManhattanDistanceFrom(this ValuePoint point, Point other)
        => Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);

    /// <inheritdoc cref="ManhattanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ManhattanDistanceFrom(this ValuePoint point, IPoint other)
        => Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);

    /// <inheritdoc cref="ManhattanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ManhattanDistanceFrom(this Point point, ValuePoint other)
        => Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);

    /// <inheritdoc cref="ManhattanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ManhattanDistanceFrom(this Point point, Point other) => Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);

    /// <inheritdoc cref="ManhattanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ManhattanDistanceFrom(this Point point, IPoint other) => Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);

    /// <inheritdoc cref="ManhattanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ManhattanDistanceFrom(this IPoint point, ValuePoint other)
        => Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);

    /// <inheritdoc cref="ManhattanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ManhattanDistanceFrom(this IPoint point, Point other) => Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);

    /// <summary>
    ///     Determines the distances between this <see cref="Chaos.Geometry.Abstractions.IPoint" /> and another
    ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
    /// </summary>
    /// <param name="point">
    /// </param>
    /// <param name="other">
    ///     The <see cref="Chaos.Geometry.Abstractions.IPoint" /> to check distance against
    /// </param>
    /// <returns>
    ///     The manhattan distance between the two given points
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ManhattanDistanceFrom(this IPoint point, IPoint other)
    {
        ArgumentNullException.ThrowIfNull(point);

        ArgumentNullException.ThrowIfNull(other);

        return Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);
    }
    #endregion

    #region Point OffsetTowards
    /// <inheritdoc cref="OffsetTowards(IPoint, IPoint)" />
    public static Point OffsetTowards(this ValuePoint point, ValuePoint other)
    {
        var direction = other.DirectionalRelationTo(point);

        return point.DirectionalOffset(direction);
    }

    /// <inheritdoc cref="OffsetTowards(IPoint, IPoint)" />
    public static Point OffsetTowards(this ValuePoint point, Point other)
    {
        var direction = other.DirectionalRelationTo(point);

        return point.DirectionalOffset(direction);
    }

    /// <inheritdoc cref="OffsetTowards(IPoint, IPoint)" />
    public static Point OffsetTowards(this ValuePoint point, IPoint other)
    {
        var direction = other.DirectionalRelationTo(point);

        return point.DirectionalOffset(direction);
    }

    /// <inheritdoc cref="OffsetTowards(IPoint, IPoint)" />
    public static Point OffsetTowards(this Point point, ValuePoint other)
    {
        var direction = other.DirectionalRelationTo(point);

        return point.DirectionalOffset(direction);
    }

    /// <inheritdoc cref="OffsetTowards(IPoint, IPoint)" />
    public static Point OffsetTowards(this Point point, Point other)
    {
        var direction = other.DirectionalRelationTo(point);

        return point.DirectionalOffset(direction);
    }

    /// <inheritdoc cref="OffsetTowards(IPoint, IPoint)" />
    public static Point OffsetTowards(this Point point, IPoint other)
    {
        var direction = other.DirectionalRelationTo(point);

        return point.DirectionalOffset(direction);
    }

    /// <inheritdoc cref="OffsetTowards(IPoint, IPoint)" />
    public static Point OffsetTowards(this IPoint point, ValuePoint other)
    {
        var direction = other.DirectionalRelationTo(point);

        return point.DirectionalOffset(direction);
    }

    /// <inheritdoc cref="OffsetTowards(IPoint, IPoint)" />
    public static Point OffsetTowards(this IPoint point, Point other)
    {
        var direction = other.DirectionalRelationTo(point);

        return point.DirectionalOffset(direction);
    }

    /// <summary>
    ///     Offsets one <see cref="Chaos.Geometry.Abstractions.IPoint" /> towards another
    ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
    /// </summary>
    /// <param name="point">
    /// </param>
    /// <param name="other">
    ///     The point to offset towards
    /// </param>
    /// <returns>
    ///     A new <see cref="Chaos.Geometry.Point" /> that has been offset in the direction of <paramref name="other" />
    /// </returns>
    public static Point OffsetTowards(this IPoint point, IPoint other)
    {
        ArgumentNullException.ThrowIfNull(point);

        ArgumentNullException.ThrowIfNull(other);

        var direction = other.DirectionalRelationTo(point);

        return point.DirectionalOffset(direction);
    }
    #endregion

    #region Points OrderByAngle
    /// <summary>
    ///     Orders a sequence of points by their angle in relation to a given point
    /// </summary>
    /// <param name="points">
    ///     A sequence of points
    /// </param>
    /// <param name="origin">
    ///     The point for which to get the angle for
    /// </param>
    public static IEnumerable<T> OrderByAngle<T>(this IEnumerable<T> points, T origin) where T: IPoint
        => points.OrderBy(pt => Math.Atan2(origin.Y - pt.Y, origin.X - pt.X));

    /// <summary>
    ///     Orders a sequence of points by their angle in relation to a given point
    /// </summary>
    /// <param name="points">
    ///     A sequence of points
    /// </param>
    /// <param name="origin">
    ///     The point for which to get the angle for
    /// </param>
    public static IEnumerable<Point> OrderByAngle(this IEnumerable<Point> points, Point origin)
        => points.OrderBy(pt => Math.Atan2(origin.Y - pt.Y, origin.X - pt.X));
    #endregion

    #region Point RayTraceTo
    /// <inheritdoc cref="RayTraceTo(IPoint, IPoint)" />
    public static IEnumerable<Point> RayTraceTo(this ValuePoint start, Point end) => RayTraceTo((Point)start, end);

    /// <inheritdoc cref="RayTraceTo(IPoint, IPoint)" />
    public static IEnumerable<Point> RayTraceTo(this ValuePoint start, IPoint end) => RayTraceTo((Point)start, end);

    /// <inheritdoc cref="RayTraceTo(IPoint, IPoint)" />
    public static IEnumerable<Point> RayTraceTo(this Point start, Point end)
    {
        var x0 = start.X;
        var y0 = start.Y;
        var x1 = end.X;
        var y1 = end.Y;
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

    /// <inheritdoc cref="RayTraceTo(IPoint, IPoint)" />
    public static IEnumerable<Point> RayTraceTo(this Point start, IPoint end)
    {
        var x0 = start.X;
        var y0 = start.Y;
        var x1 = end.X;
        var y1 = end.Y;
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

    /// <inheritdoc cref="RayTraceTo(IPoint, IPoint)" />
    public static IEnumerable<Point> RayTraceTo(this IPoint start, Point end)
    {
        var x0 = start.X;
        var y0 = start.Y;
        var x1 = end.X;
        var y1 = end.Y;
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

    /// <summary>
    ///     Lazily generates points between two points.
    ///     <br />
    ///     https://playtechs.blogspot.com/2007/03/raytracing-on-grid.html
    /// </summary>
    /// <param name="start">
    ///     The starting point
    /// </param>
    /// <param name="end">
    ///     The ending point
    /// </param>
    /// <remarks>
    ///     This will enumerate all points between <paramref name="start" /> and <paramref name="end" /> as if a line had been
    ///     drawn perfectly between the two points. Any point the line crosses over will be returned.
    ///     <br />
    /// </remarks>
    public static IEnumerable<Point> RayTraceTo(this IPoint start, IPoint end)
    {
        ArgumentNullException.ThrowIfNull(start);

        ArgumentNullException.ThrowIfNull(end);

        var x0 = start.X;
        var y0 = start.Y;
        var x1 = end.X;
        var y1 = end.Y;
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
    #endregion

    #region Point SpiralSearch
    /// <inheritdoc cref="SpiralSearch(Point, int)" />
    public static IEnumerable<Point> SpiralSearch(this ValuePoint point, int maxRadius = byte.MaxValue)
        => SpiralSearch((Point)point, maxRadius);

    /// <inheritdoc cref="SpiralSearch(IPoint, int)" />
    public static IEnumerable<Point> SpiralSearch(this Point point, int maxRadius = byte.MaxValue)
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
    ///     Lazily generates points around a given point. The search expands outwards from the given point until it reaches the
    ///     specified max distance
    /// </summary>
    /// <param name="point">
    ///     The point to search around
    /// </param>
    /// <param name="maxRadius">
    ///     The maximum distance from the <paramref name="point" /> to search
    /// </param>
    /// <remarks>
    ///     The search starts from <see cref="Chaos.Geometry.Abstractions.Definitions.Direction.Up" /> and searches clock-wise
    /// </remarks>
    public static IEnumerable<Point> SpiralSearch(this IPoint point, int maxRadius = byte.MaxValue)
    {
        ArgumentNullException.ThrowIfNull(point);

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
    #endregion

    #region Points WithConsistentDirectionBias
    /// <inheritdoc cref="WithConsistentDirectionBias{T}(IEnumerable{T}, Direction)" />
    public static IEnumerable<Point> WithConsistentDirectionBias(this IEnumerable<Point> points, Direction direction)
    {
        ArgumentNullException.ThrowIfNull(points);

        if (direction is Direction.Invalid or Direction.All)
            throw new ArgumentOutOfRangeException(nameof(direction));

        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return direction switch
        {
            Direction.Up => points.OrderBy(p => p.Y)
                                  .ThenBy(p => p.X),
            Direction.Right => points.OrderByDescending(p => p.X)
                                     .ThenByDescending(p => p.Y),
            Direction.Down => points.OrderByDescending(p => p.Y)
                                    .ThenByDescending(p => p.X),
            Direction.Left => points.OrderBy(p => p.X)
                                    .ThenBy(p => p.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    /// <summary>
    ///     Orders points by their X or Y values, based on the direction given. The output of this method will always order
    ///     points in the same order.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    public static IEnumerable<TPoint> WithConsistentDirectionBias<TPoint>(this IEnumerable<TPoint> points, Direction direction)
        where TPoint: IPoint
    {
        ArgumentNullException.ThrowIfNull(points);

        if (direction is Direction.Invalid or Direction.All)
            throw new ArgumentOutOfRangeException(nameof(direction));

        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return direction switch
        {
            Direction.Up => points.OrderBy(p => p.Y)
                                  .ThenBy(p => p.X),
            Direction.Right => points.OrderByDescending(p => p.X)
                                     .ThenByDescending(p => p.Y),
            Direction.Down => points.OrderByDescending(p => p.Y)
                                    .ThenByDescending(p => p.X),
            Direction.Left => points.OrderBy(p => p.X)
                                    .ThenBy(p => p.Y),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
    #endregion

    #region Points WithDirectionBias
    /// <inheritdoc cref="WithDirectionBias{T}(IEnumerable{T}, Direction)" />
    public static IEnumerable<Point> WithDirectionBias(this IEnumerable<Point> points, Direction direction)
    {
        ArgumentNullException.ThrowIfNull(points);

        if (direction is Direction.Invalid or Direction.All)
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
    ///     Orders points by their X or Y values, based on the direction given.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    public static IEnumerable<TPoint> WithDirectionBias<TPoint>(this IEnumerable<TPoint> points, Direction direction) where TPoint: IPoint
    {
        ArgumentNullException.ThrowIfNull(points);

        if (direction is Direction.Invalid or Direction.All)
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
    #endregion

    #region Point FloodFill
    /// <inheritdoc cref="FloodFill{T}(IEnumerable{T}, T)" />
    public static IEnumerable<Point> FloodFill(this IEnumerable<Point> points, Point start)
    {
        var allPoints = points.ToHashSet();

        var shape = new HashSet<Point>
        {
            start
        };

        var discoveryQueue = new Stack<Point>();
        discoveryQueue.Push(start);

        yield return start;

        while (discoveryQueue.TryPop(out var popped))
            foreach (var neighbor in GetNeighbors(popped, allPoints))
                if (shape.Add(neighbor))
                {
                    yield return neighbor;

                    discoveryQueue.Push(neighbor);
                }

        yield break;

        static IEnumerable<Point> GetNeighbors(Point point, HashSet<Point> localAllPoints)
        {
            foreach (var cardinalPoint in point.GenerateCardinalPoints())
                if (localAllPoints.TryGetValue(cardinalPoint, out var existingPoint))
                    yield return existingPoint;
        }
    }

    /// <summary>
    ///     Flood fills in a given point set starting at a given point
    /// </summary>
    /// <param name="points">
    ///     All possible points
    /// </param>
    /// <param name="start">
    ///     The starting point
    /// </param>
    /// <typeparam name="T">
    ///     An inheritor of IPoint
    /// </typeparam>
    /// <returns>
    ///     A sequence of all touching points contained within the given sequence starting with the given start point
    /// </returns>
    public static IEnumerable<T> FloodFill<T>(this IEnumerable<T> points, T start) where T: IPoint
    {
        var allPoints = points.Cast<IPoint>()
                              .ToHashSet(PointEqualityComparer.Instance);

        var shape = new HashSet<IPoint>(PointEqualityComparer.Instance)
        {
            start
        };

        var discoveryQueue = new Stack<IPoint>();
        discoveryQueue.Push(start);

        yield return start;

        while (discoveryQueue.TryPop(out var popped))
            foreach (var neighbor in GetNeighbors(popped, allPoints))
                if (shape.Add(neighbor))
                {
                    yield return (T)neighbor;

                    discoveryQueue.Push(neighbor);
                }

        yield break;

        static IEnumerable<IPoint> GetNeighbors(IPoint point, HashSet<IPoint> localAllPoints)
        {
            foreach (var cardinalPoint in point.GenerateCardinalPoints())
                if (localAllPoints.TryGetValue(cardinalPoint, out var existingPoint))
                    yield return existingPoint;
        }
    }
    #endregion

    #region Point ConalSearch
    /// <inheritdoc cref="ConalSearch(IPoint, Direction, int)" />
    public static IEnumerable<Point> ConalSearch(this ValuePoint point, Direction direction, int maxDistance)
        => ConalSearch((Point)point, direction, maxDistance);

    /// <inheritdoc cref="ConalSearch(IPoint, Direction, int)" />
    public static IEnumerable<Point> ConalSearch(this Point point, Direction direction, int maxDistance)
    {
        if (direction == Direction.Invalid)
            throw new ArgumentOutOfRangeException(nameof(direction), "Direction cannot be invalid");

        foreach (var edgePair in point.GenerateIntercardinalPoints(direction, maxDistance)
                                      .Chunk(2))
        {
            var edge1 = edgePair[0];
            var edge2 = edgePair[1];

            foreach (var pt in edge1.GetDirectPath(edge2))
                yield return pt;
        }
    }

    /// <summary>
    ///     Lazily generates a sequence of points in a cone shape.
    /// </summary>
    /// <param name="point">
    ///     The starting point of the cone
    /// </param>
    /// <param name="direction">
    ///     The direction the cone is facing
    /// </param>
    /// <param name="maxDistance">
    ///     The maximum distance the cone extends from the starting point
    /// </param>
    /// <returns>
    ///     An enumeration of points in the shape of a cone
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    /// <remarks>
    ///     The <paramref name="maxDistance" /> is the maximum distance the cone extends from the starting point, however there
    ///     will be points that are part of the cone that are farther than <paramref name="maxDistance" /> distance from the
    ///     starting point. This is because the forward edges and the center of the cone both extend the same number of spaces
    ///     in the given direction.
    /// </remarks>
    public static IEnumerable<Point> ConalSearch(this IPoint point, Direction direction, int maxDistance)
    {
        ArgumentNullException.ThrowIfNull(point);

        if (direction == Direction.Invalid)
            throw new ArgumentOutOfRangeException(nameof(direction), "Direction cannot be invalid");

        foreach (var edgePair in point.GenerateIntercardinalPoints(direction, maxDistance)
                                      .Chunk(2))
        {
            var edge1 = edgePair[0];
            var edge2 = edgePair[1];

            foreach (var pt in edge1.GetDirectPath(edge2))
                yield return pt;
        }
    }
    #endregion

    #region Point DirectionalOffset
    /// <inheritdoc cref="DirectionalOffset(IPoint, Direction, int)" />
    public static Point DirectionalOffset(this ValuePoint point, Direction direction, int distance = 1)
    {
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

    /// <inheritdoc cref="DirectionalOffset(IPoint, Direction, int)" />
    public static Point DirectionalOffset(this Point point, Direction direction, int distance = 1)
    {
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

    /// <summary>
    ///     Offsets an <see cref="Chaos.Geometry.Abstractions.IPoint" /> in the specified
    ///     <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> by the specified <paramref name="distance" />
    /// </summary>
    /// <param name="point">
    ///     The point to offset
    /// </param>
    /// <param name="direction">
    ///     The direction to offset to
    /// </param>
    /// <param name="distance">
    ///     The distance to offset by
    /// </param>
    /// <returns>
    ///     A new <see cref="Chaos.Geometry.Point" /> offset <paramref name="distance" /> number of tiles in
    ///     <paramref name="direction" />
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    public static Point DirectionalOffset(this IPoint point, Direction direction, int distance = 1)
    {
        ArgumentNullException.ThrowIfNull(point);

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
    #endregion

    #region Point DirectionalRelationTo
    /// <inheritdoc cref="DirectionalRelationTo(IPoint, IPoint)" />
    public static Direction DirectionalRelationTo(this ValuePoint point, ValuePoint other)
    {
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

    /// <inheritdoc cref="DirectionalRelationTo(IPoint, IPoint)" />
    public static Direction DirectionalRelationTo(this ValuePoint point, Point other)
    {
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

    /// <inheritdoc cref="DirectionalRelationTo(IPoint, IPoint)" />
    public static Direction DirectionalRelationTo(this ValuePoint point, IPoint other)
    {
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

    /// <inheritdoc cref="DirectionalRelationTo(IPoint, IPoint)" />
    public static Direction DirectionalRelationTo(this Point point, ValuePoint other)
    {
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

    /// <inheritdoc cref="DirectionalRelationTo(IPoint, IPoint)" />
    public static Direction DirectionalRelationTo(this Point point, Point other)
    {
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

    /// <inheritdoc cref="DirectionalRelationTo(IPoint, IPoint)" />
    public static Direction DirectionalRelationTo(this Point point, IPoint other)
    {
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

    /// <inheritdoc cref="DirectionalRelationTo(IPoint, IPoint)" />
    public static Direction DirectionalRelationTo(this IPoint point, Point other)
    {
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

    /// ///
    /// <summary>
    ///     Determines the directional relationship between this <see cref="Chaos.Geometry.Abstractions.IPoint" /> and another
    ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
    /// </summary>
    /// <param name="point">
    ///     The <see cref="Chaos.Geometry.Abstractions.IPoint" /> whose relation to another to find
    /// </param>
    /// <param name="other">
    ///     The <see cref="Chaos.Geometry.Abstractions.IPoint" /> to find the relation to
    /// </param>
    /// <returns>
    ///     The <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" />  <paramref name="other" /> would need to face
    ///     to be facing <paramref name="point" />
    /// </returns>
    public static Direction DirectionalRelationTo(this IPoint point, IPoint other)
    {
        ArgumentNullException.ThrowIfNull(point);

        ArgumentNullException.ThrowIfNull(other);

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
    #endregion

    #region Point EuclideanDistanceFrom
    /// <inheritdoc cref="EuclideanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EuclideanDistanceFrom(this ValuePoint point, ValuePoint other)
    {
        var xDiff = other.X - point.X;
        var yDiff = other.Y - point.Y;

        return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
    }

    /// <inheritdoc cref="EuclideanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EuclideanDistanceFrom(this ValuePoint point, Point other)
    {
        var xDiff = other.X - point.X;
        var yDiff = other.Y - point.Y;

        return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
    }

    /// <inheritdoc cref="EuclideanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EuclideanDistanceFrom(this ValuePoint point, IPoint other)
    {
        var xDiff = other.X - point.X;
        var yDiff = other.Y - point.Y;

        return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
    }

    /// <inheritdoc cref="EuclideanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EuclideanDistanceFrom(this Point point, ValuePoint other)
    {
        var xDiff = other.X - point.X;
        var yDiff = other.Y - point.Y;

        return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
    }

    /// <inheritdoc cref="EuclideanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EuclideanDistanceFrom(this Point point, Point other)
    {
        var xDiff = other.X - point.X;
        var yDiff = other.Y - point.Y;

        return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
    }

    /// <inheritdoc cref="EuclideanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EuclideanDistanceFrom(this Point point, IPoint other)
    {
        var xDiff = other.X - point.X;
        var yDiff = other.Y - point.Y;

        return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
    }

    /// <inheritdoc cref="EuclideanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EuclideanDistanceFrom(this IPoint point, ValuePoint other)
    {
        var xDiff = other.X - point.X;
        var yDiff = other.Y - point.Y;

        return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
    }

    /// <inheritdoc cref="EuclideanDistanceFrom(IPoint, IPoint)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EuclideanDistanceFrom(this IPoint point, Point other)
    {
        var xDiff = other.X - point.X;
        var yDiff = other.Y - point.Y;

        return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
    }

    /// <summary>
    ///     Determines the distances between this <see cref="Chaos.Geometry.Abstractions.IPoint" /> and another
    ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
    /// </summary>
    /// <param name="point">
    /// </param>
    /// <param name="other">
    ///     The <see cref="Chaos.Geometry.Abstractions.IPoint" /> to check distance against
    /// </param>
    /// <returns>
    ///     The manhattan distance between the two given points
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EuclideanDistanceFrom(this IPoint point, IPoint other)
    {
        ArgumentNullException.ThrowIfNull(point);

        ArgumentNullException.ThrowIfNull(other);

        var xDiff = other.X - point.X;
        var yDiff = other.Y - point.Y;

        return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
    }
    #endregion
}