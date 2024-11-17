#region
using System.Runtime.CompilerServices;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
#endregion

namespace Chaos.Extensions.Geometry;

/// <summary>
///     Provides extension methods for <see cref="Chaos.Geometry.Abstractions.ICircle" />
/// </summary>
public static class CircleExtensions
{
    /// <summary>
    ///     Calculates the first point at which a line intersects a circle.
    /// </summary>
    /// <param name="circle">
    ///     The circle.
    /// </param>
    /// <param name="lineStart">
    ///     The start point of the line.
    /// </param>
    /// <param name="lineEnd">
    ///     The end point of the line.
    /// </param>
    /// <returns>
    ///     The first point of intersection between the line and the circle, or null if they do not intersect.
    /// </returns>
    [OverloadResolutionPriority(1)] //prefer to not box structs
    public static Point? CalculateIntersectionEntryPoint(this ICircle circle, Point lineStart, Point lineEnd)
    {
        ArgumentNullException.ThrowIfNull(circle);

        var xDiff = Math.Abs(lineEnd.X - lineStart.X);
        var yDiff = Math.Abs(lineEnd.Y - lineStart.Y);

        var directionalX = lineStart.X < lineEnd.X ? 1 : -1;
        var directionalY = lineStart.Y < lineEnd.Y ? 1 : -1;

        var err = xDiff - yDiff;

        var retX = lineStart.X;
        var retY = lineStart.Y;

        while (true)
        {
            var distanceSquared = Math.Pow(retX - circle.Center.X, 2) + Math.Pow(retY - circle.Center.Y, 2);

            // If the current point is inside the circle, return it as the intersection point.
            if (distanceSquared <= Math.Pow(circle.Radius, 2))
                return new Point(retX, retY);

            // If the line has ended, return null.
            if ((retX == lineEnd.X) && (retY == lineEnd.Y))
                return null;

            var e2 = 2 * err;

            if (e2 > -yDiff)
            {
                err -= yDiff;
                retX += directionalX;
            }

            if (e2 < xDiff)
            {
                err += xDiff;
                retY += directionalY;
            }
        }
    }

    /// <inheritdoc cref="CalculateIntersectionEntryPoint(ICircle, Point, Point)" />
    public static Point? CalculateIntersectionEntryPoint(this ICircle circle, IPoint lineStart, IPoint lineEnd)
    {
        ArgumentNullException.ThrowIfNull(lineStart);
        ArgumentNullException.ThrowIfNull(lineEnd);

        return CalculateIntersectionEntryPoint(circle, Point.From(lineStart), Point.From(lineEnd));
    }

    /// <summary>
    ///     Determines whether this circle fully encompasses another circle.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="other">
    ///     Another circle.
    /// </param>
    /// <param name="distanceType">
    ///     The distance type to use for calculations.
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if this circle fully encompasses the other (or edges touch); otherwise,
    ///     <c>
    ///         false
    ///     </c>
    ///     .
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     circle
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    ///     other
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this ICircle circle, ICircle other, DistanceType distanceType = DistanceType.Euclidean)
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(other);

        return distanceType switch
        {
            DistanceType.Manhattan => circle.Radius >= (circle.Center.ManhattanDistanceFrom(other.Center) + other.Radius),
            DistanceType.Euclidean => circle.Radius >= (circle.Center.EuclideanDistanceFrom(other.Center) + other.Radius),
            _                      => throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null)
        };
    }

    /// <summary>
    ///     Determines whether this circle contains the given point.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="point">
    ///     A point.
    /// </param>
    /// <param name="distanceType">
    ///     The distance type to use for calculations.
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if this circle contains the point, otherwise
    ///     <c>
    ///         false
    ///     </c>
    ///     .
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     circle
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    ///     point
    /// </exception>
    [OverloadResolutionPriority(1), MethodImpl(MethodImplOptions.AggressiveInlining)] //prefer to not box structs
    public static bool Contains(this ICircle circle, Point point, DistanceType distanceType = DistanceType.Euclidean)
    {
        ArgumentNullException.ThrowIfNull(circle);

        return distanceType switch
        {
            DistanceType.Manhattan => point.ManhattanDistanceFrom(circle.Center) <= circle.Radius,
            DistanceType.Euclidean => point.EuclideanDistanceFrom(circle.Center) <= circle.Radius,
            _                      => throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null)
        };
    }

    /// <inheritdoc cref="Contains(ICircle, Point, DistanceType)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(this ICircle circle, IPoint point, DistanceType distanceType = DistanceType.Euclidean)
    {
        ArgumentNullException.ThrowIfNull(point);

        return Contains(circle, Point.From(point), distanceType);
    }

    /// <summary>
    ///     Calculates the edge-to-center euclidean distance to some center-point.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="other">
    ///     A center-point of some entity.
    /// </param>
    /// <returns>
    ///     The euclidean distance between the center-point of this circle and the some other point, minus this circle's
    ///     radius. Value can not be negative.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     circle
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    ///     other
    /// </exception>
    [OverloadResolutionPriority(1), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EuclideanEdgeDistanceFrom(this ICircle circle, Point other)
    {
        ArgumentNullException.ThrowIfNull(circle);

        return Math.Max(0.0f, circle.Center.EuclideanDistanceFrom(other) - circle.Radius);
    }

    /// <inheritdoc cref="EuclideanEdgeDistanceFrom(ICircle, Point)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EuclideanEdgeDistanceFrom(this ICircle circle, IPoint other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return EuclideanEdgeDistanceFrom(circle, Point.From(other));
    }

    /// <summary>
    ///     Calculates the edge-to-edge euclidean distance to another circle.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="other">
    ///     Another circle.
    /// </param>
    /// <returns>
    ///     The euclidean distance between the centerpoints of two circles, minus the sum of their radi. Value can not be
    ///     negative.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     circle
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    ///     other
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EuclideanEdgeToEdgeDistanceFrom(this ICircle circle, ICircle other)
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(other);

        return Math.Max(0.0f, circle.Center.EuclideanDistanceFrom(other.Center) - circle.Radius - other.Radius);
    }

    /// <summary>
    ///     Generates a sequence of point along the circumference of this circle
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <returns>
    ///     A sequence of point along the circumfnerence of the circle
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     circle
    /// </exception>
    public static IEnumerable<Point> GetOutline(this ICircle circle)
    {
        ArgumentNullException.ThrowIfNull(circle);

        var set = new HashSet<Point>();
        var x = circle.Center.X;
        var y = circle.Center.Y;
        var xOffset = circle.Radius;
        var yOffset = 0;
        var decisionOver2 = 1 - xOffset;

        while (yOffset <= xOffset)
        {
            var pt1 = new Point(x + xOffset, y + yOffset);
            var pt2 = new Point(x + yOffset, y + xOffset);
            var pt3 = new Point(x - yOffset, y + xOffset);
            var pt4 = new Point(x - xOffset, y + yOffset);
            var pt5 = new Point(x - xOffset, y - yOffset);
            var pt6 = new Point(x - yOffset, y - xOffset);
            var pt7 = new Point(x + yOffset, y - xOffset);
            var pt8 = new Point(x + xOffset, y - yOffset);

            if (set.Add(pt1))
                yield return pt1;

            if (set.Add(pt2))
                yield return pt2;

            if (set.Add(pt3))
                yield return pt3;

            if (set.Add(pt4))
                yield return pt4;

            if (set.Add(pt5))
                yield return pt5;

            if (set.Add(pt6))
                yield return pt6;

            if (set.Add(pt7))
                yield return pt7;

            if (set.Add(pt8))
                yield return pt8;

            yOffset++;

            if (decisionOver2 <= 0)
                decisionOver2 += 2 * yOffset + 1;
            else
            {
                xOffset--;
                decisionOver2 += 2 * (yOffset - xOffset) + 1;
            }
        }
    }

    /// <summary>
    ///     Lazily generates all points within this circle.
    /// </summary>
    /// <param name="circle">
    /// </param>
    /// <returns>
    ///     <see cref="IEnumerable{T}" /> of <see cref="Point" />
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     circle
    /// </exception>
    public static IEnumerable<Point> GetPoints(this ICircle circle)
    {
        ArgumentNullException.ThrowIfNull(circle);

        var set = new HashSet<Point>();
        var centerX = circle.Center.X;
        var centerY = circle.Center.Y;
        var radiusSqrd = circle.Radius * circle.Radius;

        for (var x = centerX - circle.Radius; x <= centerX; x++)
            for (var y = centerY - circle.Radius; y <= centerY; y++)
            {
                var xdc = x - centerX;
                var ydc = y - centerY;

                if ((xdc * xdc + ydc * ydc) <= radiusSqrd)
                {
                    var xS = centerX - xdc;
                    var yS = centerY - ydc;

                    var pt1 = new Point(x, y);
                    var pt2 = new Point(x, yS);
                    var pt3 = new Point(xS, y);
                    var pt4 = new Point(xS, yS);

                    if (set.Add(pt1))
                        yield return pt1;

                    if (set.Add(pt2))
                        yield return pt2;

                    if (set.Add(pt3))
                        yield return pt3;

                    if (set.Add(pt4))
                        yield return pt4;
                }
            }
    }

    /// <summary>
    ///     Gets a random point within this circle.
    /// </summary>
    /// <param name="circle">
    ///     The circle
    /// </param>
    /// <returns>
    /// </returns>
    public static Point GetRandomPoint(this ICircle circle)
    {
        var rngA = Random.Shared.NextDouble();
        var rngR = Random.Shared.NextDouble();
        var rngAngle = rngA * 2 * Math.PI;
        var rngRadius = Math.Sqrt(rngR) * circle.Radius;
        var x = (int)(rngRadius * Math.Cos(rngAngle) + circle.Center.X);
        var y = (int)(rngRadius * Math.Sin(rngAngle) + circle.Center.Y);

        return new Point(x, y);
    }

    /// <summary>
    ///     Determines whether this circle intersects with another circle.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="other">
    ///     Another circle.
    /// </param>
    /// <param name="distanceType">
    ///     The distance type to use for calculations.
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if this circle intersects the <paramref name="other" />,
    ///     <c>
    ///         false
    ///     </c>
    ///     otherwise.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     circle
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    ///     other
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(this ICircle circle, ICircle other, DistanceType distanceType = DistanceType.Euclidean)
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(other);

        return distanceType switch
        {
            DistanceType.Manhattan => circle.Center.ManhattanDistanceFrom(other.Center) <= (circle.Radius + other.Radius),
            DistanceType.Euclidean => circle.Center.EuclideanDistanceFrom(other.Center) <= (circle.Radius + other.Radius),
            _                      => throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null)
        };
    }

    /// <summary>
    ///     Determines whether the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" /> intersects a
    /// </summary>
    /// <param name="rect">
    ///     A rectangle
    /// </param>
    /// <param name="circle">
    ///     A circle
    /// </param>
    /// <param name="distanceType">
    ///     The distance type to use for calculations.
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
    public static bool Intersects(this ICircle circle, IRectangle rect, DistanceType distanceType = DistanceType.Euclidean)
    {
        ArgumentNullException.ThrowIfNull(rect);
        ArgumentNullException.ThrowIfNull(circle);

        var closestX = Math.Clamp(circle.Center.X, rect.Left, rect.Right);
        var closestY = Math.Clamp(circle.Center.Y, rect.Top, rect.Bottom);

        return distanceType switch
        {
            DistanceType.Manhattan => new Point(closestX, closestY).ManhattanDistanceFrom(circle.Center) <= circle.Radius,
            DistanceType.Euclidean => new Point(closestX, closestY).EuclideanDistanceFrom(circle.Center) <= circle.Radius,
            _                      => throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null)
        };
    }

    /// <summary>
    ///     Calculates the edge-to-center euclidean distance to some center-point.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="other">
    ///     A center-point of some entity.
    /// </param>
    /// <returns>
    ///     The manhattan distance between the center-point of this circle and the some other point, minus this circle's
    ///     radius. Value can not be negative.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     circle
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    ///     other
    /// </exception>
    [OverloadResolutionPriority(1), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ManhattanEdgeDistanceFrom(this ICircle circle, Point other)
    {
        ArgumentNullException.ThrowIfNull(circle);

        return Math.Max(0, circle.Center.ManhattanDistanceFrom(other) - circle.Radius);
    }

    /// <inheritdoc cref="ManhattanEdgeDistanceFrom(ICircle, Point)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ManhattanEdgeDistanceFrom(this ICircle circle, IPoint other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return ManhattanEdgeDistanceFrom(circle, Point.From(other));
    }

    /// <summary>
    ///     Calculates the edge-to-edge euclidean distance to another circle.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="other">
    ///     Another circle.
    /// </param>
    /// <returns>
    ///     The manhattan distance between the centerpoints of two circles, minus the sum of their radi. Value can not be
    ///     negative.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     circle
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    ///     other
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ManhattanEdgeToEdgeDistanceFrom(this ICircle circle, ICircle other)
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(other);

        return Math.Max(0, circle.Center.ManhattanDistanceFrom(other.Center) - circle.Radius - other.Radius);
    }
}