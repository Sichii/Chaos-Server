#region
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
#endregion

namespace Chaos.Extensions.Geometry;

/// <summary>
///     Provides extension methods for <see cref="Chaos.Geometry.Abstractions.IRectangle" />
/// </summary>
public static class RectangleExtensions
{
    /// <param name="rect">
    ///     The possibly outer rectangle
    /// </param>
    extension<T>(T rect) where T: IRectangle, allows ref struct
    {
        /// <summary>
        ///     Determines whether the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" /> contains an
        ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
        /// </summary>
        /// <param name="point">
        ///     The point to check
        /// </param>
        /// <returns>
        ///     <c>
        ///         true
        ///     </c>
        ///     if <paramref name="point" /> is inside of the rect, otherwise
        ///     <c>
        ///         false
        ///     </c>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [OverloadResolutionPriority(1)]
        public bool ContainsPoint<TPoint>(TPoint point) where TPoint: IPoint, allows ref struct
            => (rect.Left <= point.X) && (rect.Right >= point.X) && (rect.Top <= point.Y) && (rect.Bottom >= point.Y);

        /// <summary>
        ///     Determines whether the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" /> contains another
        ///     <see cref="Chaos.Geometry.Abstractions.IRectangle" />
        /// </summary>
        /// <param name="other">
        ///     The possible inner rectangle
        /// </param>
        /// <returns>
        ///     <c>
        ///         true
        ///     </c>
        ///     if the rect fully encompasses <paramref name="other" />, otherwise
        ///     <c>
        ///         false
        ///     </c>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsRectangle<T2>(T2 other) where T2: IRectangle, allows ref struct
            => (rect.Bottom >= other.Bottom) && (rect.Left <= other.Left) && (rect.Right >= other.Right) && (rect.Top <= other.Top);

        /// <summary>
        ///     Lazily generates points along the outline of the rectangle. The points will be in the order the vertices are
        ///     listed.
        /// </summary>
        public IEnumerable<Point> GetOutline()
        {
            var vertices = rect.Vertices;

            return InnerGetOutline(vertices);

            static IEnumerable<Point> InnerGetOutline(IReadOnlyList<IPoint> localVertices)
            {
                for (var i = 0; i < (localVertices.Count - 1); i++)
                {
                    var current = localVertices[i];
                    var next = localVertices[i + 1];

                    //skip the last point so the vertices are not included twice
                    foreach (var point in current.GetDirectPath(next)
                                                 .SkipLast(1))
                        yield return point;
                }

                foreach (var point in localVertices[^1]
                                      .GetDirectPath(localVertices[0])
                                      .SkipLast(1))
                    yield return point;
            }
        }

        /// <summary>
        ///     Lazily generates all points inside of the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Point> GetPoints()
            => new RectanglePointIterator(
                rect.Left,
                rect.Top,
                rect.Right,
                rect.Bottom);

        /// <summary>
        ///     Generates a random point inside the <see cref="Chaos.Geometry.Abstractions.IRectangle" />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point GetRandomPoint() => new(rect.Left + Random.Shared.Next(rect.Width), rect.Top + Random.Shared.Next(rect.Height));

        /// <summary>
        ///     Determines whether the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" /> intersects another
        ///     <see cref="Chaos.Geometry.Abstractions.IRectangle" />
        /// </summary>
        /// <param name="other">
        ///     Another rectangle
        /// </param>
        /// <returns>
        ///     <c>
        ///         true
        ///     </c>
        ///     if the rectangles intersect at any point or if either rect fully contains the other, otherwise
        ///     <c>
        ///         false
        ///     </c>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects<T2>(T2 other) where T2: IRectangle, allows ref struct
            => !((rect.Bottom < other.Top) || (rect.Left > other.Right) || (rect.Right < other.Left) || (rect.Top > other.Bottom));

        /// <summary>
        ///     Determines whether the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" /> intersects a
        /// </summary>
        /// <param name="circle">
        ///     A circle
        /// </param>
        /// <param name="distanceType">
        ///     The type of distance check to use when measuring distance
        /// </param>
        /// <returns>
        ///     <c>
        ///         true
        ///     </c>
        ///     if the rectangle intersects the circle at any point, or if either fully contains the other, otherwise
        ///     <c>
        ///         false
        ///     </c>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public bool Intersects<TCircle>(TCircle circle, DistanceType distanceType = DistanceType.Euclidean)
            where TCircle: ICircle, allows ref struct
        {
            var closestX = Math.Clamp(circle.Center.X, rect.Left, rect.Right);
            var closestY = Math.Clamp(circle.Center.Y, rect.Top, rect.Bottom);

            // For zero-radius circles (points), check if the point is inside/on the rectangle
            // For non-zero radius, use <= to include tangent (touching) cases
            return distanceType switch
            {
                DistanceType.Manhattan when circle.Radius == 0 => new Point(closestX, closestY).ManhattanDistanceFrom(
                                                                      Point.From(circle.Center))
                                                                  == 0,
                DistanceType.Manhattan => new Point(closestX, closestY).ManhattanDistanceFrom(Point.From(circle.Center)) <= circle.Radius,
                DistanceType.Euclidean when circle.Radius == 0 => new Point(closestX, closestY).EuclideanDistanceFrom(
                                                                      Point.From(circle.Center))
                                                                  == 0,
                DistanceType.Euclidean => new Point(closestX, closestY).EuclideanDistanceFrom(Point.From(circle.Center)) <= circle.Radius,
                _                      => throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null)
            };
        }

        /// <summary>
        ///     Generates a random point inside the <see cref="Chaos.Geometry.Abstractions.IRectangle" />
        /// </summary>
        /// <param name="predicate">
        ///     A predicate the point must match
        /// </param>
        /// <param name="point">
        ///     A random point that matches the given predicate
        /// </param>
        /// <returns>
        ///     <c>
        ///         true
        ///     </c>
        ///     if a random point was found that matches the predicate, otherwise
        ///     <c>
        ///         false
        ///     </c>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetRandomPoint(Func<Point, bool> predicate, [NotNullWhen(true)] out Point? point)
        {
            var totalPoints = rect.Area;
            var maxAttempts = Math.Max(1, totalPoints / 10);

            // try random points for up to 10% of possibilities
            for (var i = 0; i < maxAttempts; i++)
            {
                var randomPoint = rect.GetRandomPoint();

                if (predicate(randomPoint))
                {
                    point = randomPoint;

                    return true;
                }
            }

            // fall back to reservoir sampling (this is better than materializing a list or set)
            Point? selected = null;
            var count = 0;

            foreach (var pt in rect.GetPoints())
                if (predicate(pt))
                {
                    count++;

                    // with probability 1/count, select this element
                    if (Random.Shared.Next(count) == 0)
                        selected = pt;
                }

            point = selected;

            return selected.HasValue;
        }
    }

    /// <param name="rect">
    ///     The rect that represents the bounds of the maze
    /// </param>
    extension(IRectangle rect)
    {
        /// <summary>
        ///     Given a start and end point, generates a maze inside the <see cref="Chaos.Geometry.Abstractions.IRectangle" />
        /// </summary>
        /// <param name="start">
        ///     The start point of the maze
        /// </param>
        /// <param name="end">
        ///     The end point of the maze
        /// </param>
        /// <returns>
        /// </returns>
        public IEnumerable<Point> GenerateMaze<T, TEnd>(T start, TEnd end) where T: IPoint
                                                                           where TEnd: IPoint
        {
            //neighbor pattern
            List<(int, int)> pattern =
            [
                (0, 1),
                (1, 0),
                (0, -1),
                (-1, 0)
            ];

            var height = rect.Height;
            var width = rect.Width;
            var startNode = UnOffsetCoordinates(rect, start);
            var endNode = UnOffsetCoordinates(rect, end);
            var maze = new bool[width, height];
            var discoveryQueue = new Stack<Point>();

            var mazeRect = new Rectangle(
                0,
                0,
                width,
                height);

            //initialize maze full of walls
            for (var x = 0; x < maze.GetLength(0); x++)
                for (var y = 0; y < maze.GetLength(1); y++)
                    maze[x, y] = true;

            //start wtih startNode
            //startNode is not a wall
            discoveryQueue.Push(startNode);
            maze[startNode.X, startNode.Y] = false;

            //carve out the maze
            while (discoveryQueue.Count > 0)
            {
                //get current node
                var current = discoveryQueue.Peek();
                var carved = false;

                //shuffle neighbor pattern so we get a random direction
                IRectangle.ShuffleInPlace(pattern);

                //for each direction in the pattern
                foreach ((var dx, var dy) in pattern)
                {
                    //get a point 2 spaces in the direction
                    var target = new Point(current.X + dx * 2, current.Y + dy * 2);

                    //if the target is out of bounds or already carved, skip
                    if (!mazeRect.ContainsPoint(target) || !maze[target.X, target.Y])
                        continue;

                    //carve out the wall between the current node and the target
                    //carve out the target node
                    maze[current.X + dx, current.Y + dy] = false;
                    maze[target.X, target.Y] = false;

                    //push the target node onto the stack
                    discoveryQueue.Push(target);
                    carved = true;

                    //don't look at any more of these neighbors
                    break;
                }

                //since we carved, pop the node we peeked
                if (!carved)
                    discoveryQueue.Pop();
            }

            //end node is not a wall
            maze[endNode.X, endNode.Y] = false;

            //yield all walls in the maze
            for (var x = 0; x < maze.GetLength(0); x++)
                for (var y = 0; y < maze.GetLength(1); y++)
                    if (maze[x, y])
                        yield return OffsetCoordinates(rect, new Point(x, y));

            yield break;

            static Point OffsetCoordinates(IRectangle rect, IPoint point) => new(point.X + rect.Left, point.Y + rect.Top);

            static Point UnOffsetCoordinates(IRectangle rect, IPoint point) => new(point.X - rect.Left, point.Y - rect.Top);
        }

        private static void ShuffleInPlace<T>(IList<T> arr)
        {
            for (var i = arr.Count - 1; i > 0; i--)
            {
                var j = Random.Shared.Next(i + 1);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }
    }

    /// <summary>
    ///     An allocation-free iterator over points in a rectangle
    /// </summary>
    public struct RectanglePointIterator : IEnumerator<Point>, IEnumerable<Point>
    {
        private readonly int Left;
        private readonly int Top;
        private readonly int Right;
        private readonly int Bottom;
        private int X;
        private int Y;

        /// <summary>
        ///     Creates a new <see cref="RectanglePointIterator" />
        /// </summary>
        public RectanglePointIterator(
            int left,
            int top,
            int right,
            int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
            X = left;
            Y = top - 1;
        }

        /// <inheritdoc />
        public readonly void Dispose() { }

        /// <inheritdoc />
        public bool MoveNext()
        {
            Y++;

            if (Y > Bottom)
            {
                Y = Top;
                X++;
            }

            return X <= Right;
        }

        /// <inheritdoc />
        public void Reset()
        {
            X = Left;
            Y = Top - 1;
        }

        /// <inheritdoc />
        public readonly Point Current => new(X, Y);

        /// <inheritdoc />
        readonly object IEnumerator.Current => Current;

        /// <inheritdoc />
        public IEnumerator<Point> GetEnumerator() => this;

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}