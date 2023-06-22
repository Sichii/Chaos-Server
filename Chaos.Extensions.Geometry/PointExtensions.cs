using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Geometry.EqualityComparers;

namespace Chaos.Extensions.Geometry;

/// <summary>
///     Provides extension methods for <see cref="Chaos.Geometry.Abstractions.IPoint" />.
/// </summary>
public static class PointExtensions
{
    /// <summary>
    ///     Lazily generates a sequence of points in a cone shape.
    /// </summary>
    /// <param name="point">The starting point of the cone</param>
    /// <param name="direction">The direction the cone is facing</param>
    /// <param name="maxDistance">The maximum distance the cone extends from the starting point</param>
    /// <returns>An enumeration of points in the shape of a cone</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <remarks>
    ///     The <paramref name="maxDistance" /> is the maximum distance the cone extends from the starting point, however there
    ///     will be points
    ///     that are part of the cone that are farther than <paramref name="maxDistance" /> distance from the starting point.
    ///     This is because the
    ///     forward edges and the center of the cone both extend the same number of spaces in the given direction.
    /// </remarks>
    public static IEnumerable<Point> ConalSearch(this IPoint point, Direction direction, int maxDistance)
    {
        if (direction == Direction.Invalid)
            throw new ArgumentOutOfRangeException(nameof(direction), "Direction cannot be invalid");

        foreach (var edgePair in point.GenerateIntercardinalPoints(direction, maxDistance).Chunk(2))
        {
            var edge1 = edgePair[0];
            var edge2 = edgePair[1];

            foreach (var pt in edge1.GetDirectPath(edge2))
                yield return pt;
        }
    }

    /// <summary>
    ///     Offsets an <see cref="Chaos.Geometry.Abstractions.IPoint" /> in the specified
    ///     <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> by the specified <paramref name="distance" />
    /// </summary>
    /// <param name="point">The point to offset</param>
    /// <param name="direction">The direction to offset to</param>
    /// <param name="distance">The distance to offset by</param>
    /// <returns>
    ///     A new <see cref="Chaos.Geometry.Point" /> offset <paramref name="distance" /> number of tiles in
    ///     <paramref name="direction" />
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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

    /// <summary>
    ///     Determines the directional relationship between this <see cref="Chaos.Geometry.Abstractions.IPoint" /> and another
    ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
    /// </summary>
    /// <param name="point">The <see cref="Chaos.Geometry.Abstractions.IPoint" /> whose relation to another to find</param>
    /// <param name="other">The <see cref="Chaos.Geometry.Abstractions.IPoint" /> to find the relation to</param>
    /// <returns>
    ///     The <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> <paramref name="other" /> would need to face
    ///     to be facing
    ///     <paramref name="point" />
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

    /// <summary>
    ///     Determines the distances between this <see cref="Chaos.Geometry.Abstractions.IPoint" /> and another
    ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
    /// </summary>
    /// <param name="point"></param>
    /// <param name="other">The <see cref="Chaos.Geometry.Abstractions.IPoint" /> to check distance against</param>
    /// <returns>The distance between the two given points without moving diagonally</returns>
    public static int DistanceFrom(this IPoint point, IPoint other)
    {
        ArgumentNullException.ThrowIfNull(point);

        ArgumentNullException.ThrowIfNull(other);

        return Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);
    }

    /// <summary>
    ///     Determines the distances between this <see cref="Chaos.Geometry.Abstractions.IPoint" /> and another
    ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
    /// </summary>
    /// <param name="point"></param>
    /// <param name="other">The <see cref="Chaos.Geometry.Abstractions.IPoint" /> to check distance against</param>
    /// <returns>The distance between the two given points allowing diagonal movement</returns>
    public static float EuclideanDistanceFrom(this IPoint point, IPoint other)
    {
        ArgumentNullException.ThrowIfNull(point);

        ArgumentNullException.ThrowIfNull(other);

        var xDiff = other.X - point.X;
        var yDiff = other.Y - point.Y;

        return (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
    }

    /// <summary>
    ///     Flood fills in a given point set starting at a given point
    /// </summary>
    /// <param name="points">All possible points</param>
    /// <param name="start">The starting point</param>
    /// <typeparam name="T">An inheritor of IPoint</typeparam>
    /// <returns>A sequence of all touching points contained within the given sequence starting with the given start point</returns>
    public static IEnumerable<T> FloodFill<T>(this IEnumerable<T> points, T start) where T: IPoint
    {
        var allPoints = points.Cast<IPoint>().ToHashSet(PointEqualityComparer.Instance);

        static IEnumerable<IPoint> GetNeighbors(IPoint point, HashSet<IPoint> localAllPoints)
        {
            foreach (var cardinalPoint in point.GenerateCardinalPoints())
                if (localAllPoints.TryGetValue(cardinalPoint, out var adjacentDoor))
                    yield return adjacentDoor;
        }

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
    }

    /// <summary>
    ///     Lazily generates an enumeration of points in a line from the user, with an option for distance and direction.
    ///     Direction.All is
    ///     optional. Direction.Invalid direction returns empty list.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="direction"></param>
    /// <param name="radius">The max distance to generate points</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="radius" /> must be positive</exception>
    /// <remarks>
    ///     Assumes <see cref="Chaos.Geometry.Abstractions.Definitions.Direction.Up" /> is equivalent to the cardinal direction
    ///     "North", this
    ///     method will generate points in all 4
    ///     cardinal directions. Points will be generated 1 radius at a time, clock-wise.
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
        if (direction == Direction.Invalid)
            yield break;

        if (radius <= 0)
            throw new ArgumentOutOfRangeException($"{nameof(radius)} must be positive");

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
    ///     Lazily generates an enumeration of diagonal points in relevance to the user, with an optional distance and
    ///     direction.
    ///     Direction.All is optional. Direction.Invalid direction returns an empty enumeration
    /// </summary>
    /// <param name="start"></param>
    /// <param name="direction">The general direction to generate points for. See remarks.</param>
    /// <param name="radius">The range in which to generate points</param>
    /// <remarks>
    ///     Assuming <see cref="Chaos.Geometry.Abstractions.Definitions.Direction.Up" /> is equivalent to the cardinal
    ///     direction "North", this
    ///     method will generate points in the
    ///     inter-cardinal directions "North-East", "South-East", "South-West", and "North-West". Points will be generated 1
    ///     radius at a time,
    ///     clock-wise. Optionally, you can choose a cardinal <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" />
    ///     to generate points
    ///     for the 2 inter-cardinal directions that
    ///     share the given cardinal direction.
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
    ///     Creates an enumerable list of points representing a path between two given points, and returns it.
    /// </summary>
    /// <param name="start">Starting point for the creation of the path</param>
    /// <param name="end">Ending point for the creation of the path</param>
    /// <remarks>Does not return the start point, only the points between the start and end, as well as the end point itself</remarks>
    public static IEnumerable<Point> GetDirectPath(this IPoint start, IPoint end)
    {
        var current = Point.From(start);

        yield return current;

        while (current != end)
        {
            current = current.OffsetTowards(end);

            yield return current;
        }
    }

    /// <summary>
    ///     Determines if this point is on either intercardinal diagonal in relation to another point, in the given direction
    /// </summary>
    /// <param name="point">The point to test</param>
    /// <param name="other">The point in which directions are based on</param>
    /// <param name="direction">The direction between the 2 intercardinals to check</param>
    /// <returns>
    ///     <c>true</c> if this point is on an intercardinal diagonal in relation to the other point in the given
    ///     direction, otherwise <c>false</c>
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

    /// <summary>
    ///     Offsets one <see cref="Chaos.Geometry.Abstractions.IPoint" /> towards another
    ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
    /// </summary>
    /// <param name="point"></param>
    /// <param name="other">The point to offset towards</param>
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

    /// <summary>
    ///     Lazily generates points between two points. <br />
    ///     https://playtechs.blogspot.com/2007/03/raytracing-on-grid.html
    /// </summary>
    /// <param name="start">The starting point</param>
    /// <param name="end">The ending point</param>
    /// <remarks>
    ///     This will enumerate all points between <paramref name="start" /> and <paramref name="end" /> as if a line had been
    ///     drawn perfectly
    ///     between the two points. Any point the line crosses over will be returned. <br />
    /// </remarks>
    public static IEnumerable<Point> RayTraceTo(this IPoint start, IPoint end)
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
    ///     Lazily generates points around a given point. The search expands outwards from the given point until it reaches the
    ///     specified max
    ///     distance
    /// </summary>
    /// <param name="point">The point to search around</param>
    /// <param name="maxRadius">The maximum distance from the <paramref name="point" /> to search</param>
    /// <remarks>
    ///     The search starts from <see cref="Chaos.Geometry.Abstractions.Definitions.Direction.Up" /> and searches
    ///     clock-wise
    /// </remarks>
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
    public static IEnumerable<Point> WithDirectionBias(this IEnumerable<Point> points, Direction direction)
    {
        ArgumentNullException.ThrowIfNull(points);

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