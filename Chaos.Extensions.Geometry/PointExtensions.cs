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
    /// <param name="point">
    ///     The starting point of the cone
    /// </param>
    extension<T>(T point) where T: IPoint, allows ref struct
    {
        /// <summary>
        ///     Lazily generates a sequence of points in a cone shape.
        /// </summary>
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
        public IEnumerable<Point> ConalSearch(Direction direction, int maxDistance)
        {
            if (direction == Direction.Invalid)
                throw new ArgumentOutOfRangeException(nameof(direction), "Direction cannot be invalid");

            var x = point.X;
            var y = point.Y;

            return InnerConalSearch(
                x,
                y,
                direction,
                maxDistance);

            static IEnumerable<Point> InnerConalSearch(
                int localX,
                int localY,
                Direction localDirection,
                int localMaxDistance)
            {
                var localPoint = new Point(localX, localY);

                foreach (var edgePair in localPoint.GenerateIntercardinalPoints(localDirection, localMaxDistance)
                                                   .Chunk(2))
                {
                    var edge1 = edgePair[0];
                    var edge2 = edgePair[1];

                    foreach (var pt in edge1.GetDirectPath(edge2))
                        yield return pt;
                }
            }
        }

        /// <summary>
        ///     Offsets an <see cref="Chaos.Geometry.Abstractions.IPoint" /> in the specified
        ///     <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> by the specified <paramref name="distance" />
        /// </summary>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point DirectionalOffset(Direction direction, int distance = 1)
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
        ///     Determines the directional relationship between this <see cref="Chaos.Geometry.Abstractions.IPoint" /> and another
        ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
        /// </summary>
        /// <param name="other">
        ///     The <see cref="Chaos.Geometry.Abstractions.IPoint" /> to find the relation to
        /// </param>
        /// <returns>
        ///     The <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" />  <paramref name="other" /> would need to face
        ///     to be facing a point
        /// </returns>
        public Direction DirectionalRelationTo<T2>(T2 other) where T2: IPoint, allows ref struct
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

        /// <summary>
        ///     Determines the distances between this <see cref="Chaos.Geometry.Abstractions.IPoint" /> and another
        ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
        /// </summary>
        /// <param name="other">
        ///     The <see cref="Chaos.Geometry.Abstractions.IPoint" /> to check distance against
        /// </param>
        /// <returns>
        ///     The manhattan distance between the two given points
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double EuclideanDistanceFrom<T2>(T2 other) where T2: IPoint, allows ref struct
        {
            var xDiff = other.X - point.X;
            var yDiff = other.Y - point.Y;

            return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

        /// <summary>
        ///     Lazily generates an enumeration of points in a line from the user, with an option for distance and direction.
        ///     Direction.All is optional. Direction.Invalid direction returns empty list.
        /// </summary>
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
        public IEnumerable<Point> GenerateCardinalPoints(Direction direction = Direction.All, int radius = 1)
        {
            var startPoint = new Point(point.X, point.Y);

            return InnerGenerateCardinalPoints(startPoint, direction, radius);

            static IEnumerable<Point> InnerGenerateCardinalPoints(Point localStart, Direction localDirection, int localRadius)
            {
                if (localDirection == Direction.Invalid)
                    yield break;

                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(localRadius);

                for (var i = 1; i <= localRadius; i++)
                    if (localDirection == Direction.All)
                    {
                        yield return localStart.DirectionalOffset(Direction.Up, i);
                        yield return localStart.DirectionalOffset(Direction.Right, i);
                        yield return localStart.DirectionalOffset(Direction.Down, i);
                        yield return localStart.DirectionalOffset(Direction.Left, i);
                    } else
                        yield return localStart.DirectionalOffset(localDirection, i);
            }
        }

        /// <summary>
        ///     Lazily generates an enumeration of diagonal points in relevance to the user, with an optional distance and
        ///     direction. Direction.All is optional. Direction.Invalid direction returns an empty enumeration
        /// </summary>
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
        public IEnumerable<Point> GenerateIntercardinalPoints(Direction direction = Direction.All, int radius = 1)
        {
            var x = point.X;
            var y = point.Y;

            return InnerGenerateIntercardinalPoints(
                x,
                y,
                direction,
                radius);

            static IEnumerable<Point> InnerGenerateIntercardinalPoints(
                int localX,
                int localY,
                Direction localDirection,
                int localRadius)
            {
                if (localDirection == Direction.Invalid)
                    yield break;

                for (var i = 1; i <= localRadius; i++)
                    switch (localDirection)
                    {
                        case Direction.Up:
                            yield return new Point(localX - i, localY - i);
                            yield return new Point(localX + i, localY - i);

                            break;
                        case Direction.Right:
                            yield return new Point(localX + i, localY - i);
                            yield return new Point(localX + i, localY + i);

                            break;
                        case Direction.Down:
                            yield return new Point(localX + i, localY + i);
                            yield return new Point(localX - i, localY + i);

                            break;
                        case Direction.Left:
                            yield return new Point(localX - i, localY - i);
                            yield return new Point(localX - i, localY + i);

                            break;
                        case Direction.All:
                            yield return new Point(localX - i, localY - i);
                            yield return new Point(localX + i, localY - i);
                            yield return new Point(localX + i, localY + i);
                            yield return new Point(localX - i, localY + i);

                            break;
                        default:
                            yield break;
                    }
            }
        }

        /// <summary>
        ///     Creates an enumerable list of points representing a path between two given points, and returns it.
        /// </summary>
        /// <param name="end">
        ///     Ending point for the creation of the path
        /// </param>
        /// <remarks>
        ///     Does not return the start point, only the points between the start and end, as well as the end point itself
        /// </remarks>
        public IEnumerable<Point> GetDirectPath<T2>(T2 end) where T2: IPoint, allows ref struct
        {
            var startPoint = Point.From(point);
            var endPoint = Point.From(end);

            return InnerGetDirectPath(startPoint, endPoint);

            static IEnumerable<Point> InnerGetDirectPath(Point localStart, Point localEnd)
            {
                var current = localStart;

                yield return current;

                while (!current.Equals(localEnd))
                {
                    current = current.OffsetTowards(localEnd);

                    yield return current;
                }
            }
        }

        /// <summary>
        ///     Determines if this point is on either intercardinal diagonal in relation to another point, in the given direction
        /// </summary>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInterCardinalTo<T2>(T2 other, Direction direction) where T2: IPoint, allows ref struct
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
        ///     Determines the distances between this <see cref="Chaos.Geometry.Abstractions.IPoint" /> and another
        ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
        /// </summary>
        /// <param name="other">
        ///     The <see cref="Chaos.Geometry.Abstractions.IPoint" /> to check distance against
        /// </param>
        /// <returns>
        ///     The manhattan distance between the two given points
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ManhattanDistanceFrom<T2>(T2 other) where T2: IPoint, allows ref struct
            => Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);

        /// <summary>
        ///     Offsets one <see cref="Chaos.Geometry.Abstractions.IPoint" /> towards another
        ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
        /// </summary>
        /// <param name="other">
        ///     The point to offset towards
        /// </param>
        /// <returns>
        ///     A new <see cref="Chaos.Geometry.Point" /> that has been offset in the direction of <paramref name="other" />
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point OffsetTowards<T2>(T2 other) where T2: IPoint, allows ref struct
        {
            var direction = other.DirectionalRelationTo(point);

            return point.DirectionalOffset(direction);
        }

        /// <summary>
        ///     Lazily generates points between two points.
        ///     <br />
        ///     https://playtechs.blogspot.com/2007/03/raytracing-on-grid.html
        /// </summary>
        /// <param name="end">
        ///     The ending point
        /// </param>
        /// <remarks>
        ///     This will enumerate all points between a point and <paramref name="end" /> as if a line had been drawn perfectly
        ///     between the two points. Any point the line crosses over will be returned.
        ///     <br />
        /// </remarks>
        public IEnumerable<Point> RayTraceTo<T2>(T2 end) where T2: IPoint, allows ref struct
        {
            var x0 = point.X;
            var y0 = point.Y;
            var x1 = end.X;
            var y1 = end.Y;

            return InnerRayTraceTo(
                x0,
                y0,
                x1,
                y1);

            static IEnumerable<Point> InnerRayTraceTo(
                int localX0,
                int localY0,
                int localX1,
                int localY1)
            {
                var dx = Math.Abs(localX1 - localX0);
                var dy = Math.Abs(localY1 - localY0);
                var x = localX0;
                var y = localY0;
                var n = 1 + dx + dy;
                var xOffset = localX1 > localX0 ? 1 : -1;
                var yOffset = localY1 > localY0 ? 1 : -1;
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
        }

        /// <summary>
        ///     Lazily generates points around a given point. The search expands outwards from the given point until it reaches the
        ///     specified max distance
        /// </summary>
        /// <param name="maxRadius">
        ///     The maximum distance from the point to search
        /// </param>
        /// <remarks>
        ///     The search starts from <see cref="Chaos.Geometry.Abstractions.Definitions.Direction.Up" /> and searches clock-wise
        /// </remarks>
        public IEnumerable<Point> SpiralSearch(int maxRadius = byte.MaxValue)
        {
            var x = point.X;
            var y = point.Y;

            return InnerSpiralSearch(x, y, maxRadius);

            static IEnumerable<Point> InnerSpiralSearch(int localX, int localY, int localMaxRadius)
            {
                var currentPoint = new Point(localX, localY);
                var radius = 1;

                yield return currentPoint;

                for (; radius <= localMaxRadius; radius++)
                {
                    currentPoint = currentPoint.DirectionalOffset(Direction.Up);

                    //travel from north to east
                    while (localY != currentPoint.Y)
                    {
                        currentPoint = new Point(currentPoint.X + 1, currentPoint.Y + 1);

                        yield return currentPoint;
                    }

                    //travel from east to south
                    while (localX != currentPoint.X)
                    {
                        currentPoint = new Point(currentPoint.X - 1, currentPoint.Y + 1);

                        yield return currentPoint;
                    }

                    //travel from south to west
                    while (localY != currentPoint.Y)
                    {
                        currentPoint = new Point(currentPoint.X - 1, currentPoint.Y - 1);

                        yield return currentPoint;
                    }

                    //travel from west to north
                    while (localX != currentPoint.X)
                    {
                        currentPoint = new Point(currentPoint.X + 1, currentPoint.Y - 1);

                        yield return currentPoint;
                    }
                }
            }
        }
    }

    extension<T>(ICollection<T> points) where T: IPoint
    {
        /// <summary>
        ///     Finds the input point closest to the centroid of all points.
        /// </summary>
        public T FindCenterMost()
        {
            if ((points == null) || (points.Count == 0))
                throw new ArgumentException("Points list cannot be null or empty.");

            double sumX = 0,
                   sumY = 0;

            foreach (var p in points)
            {
                sumX += p.X;
                sumY += p.Y;
            }

            var centroidX = sumX / points.Count;
            var centroidY = sumY / points.Count;

            return points.MinBy(point => DistanceSquaredTo(point, centroidX, centroidY))!;

            static double DistanceSquaredTo(T p, double x, double y)
            {
                var dx = p.X - x;
                var dy = p.Y - y;

                return dx * dx + dy * dy;
            }
        }
    }

    /// <param name="points">
    ///     All possible points
    /// </param>
    /// <typeparam name="T">
    ///     An inheritor of IPoint
    /// </typeparam>
    extension<T>(IEnumerable<T> points) where T: IPoint
    {
        /// <summary>
        ///     Flood fills in a given point set starting at a given point
        /// </summary>
        /// <param name="start">
        ///     The starting point
        /// </param>
        /// <returns>
        ///     A sequence of all touching points contained within the given sequence starting with the given start point
        /// </returns>
        public IEnumerable<T> FloodFill(T start)
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

        /// <summary>
        ///     Orders a sequence of points by their angle in relation to a given point
        /// </summary>
        /// <param name="origin">
        ///     The point for which to get the angle for
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> OrderByAngle(T origin) => points.OrderBy(pt => Math.Atan2(origin.Y - pt.Y, origin.X - pt.X));

        /// <summary>
        ///     Orders points by their X or Y values, based on the direction given. The output of this method will always order
        ///     points in the same order.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> WithConsistentDirectionBias(Direction direction)
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
        ///     Orders points by their X or Y values, based on the direction given.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> WithDirectionBias(Direction direction)
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
    }
}