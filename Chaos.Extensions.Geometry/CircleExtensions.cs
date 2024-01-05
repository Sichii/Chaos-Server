using Chaos.Geometry;
using Chaos.Geometry.Abstractions;

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
    public static Point? CalculateIntersectionEntryPoint<TCircle, TPoint>(this TCircle circle, TPoint lineStart, TPoint lineEnd)
        where TCircle: ICircle
        where TPoint: IPoint
    {
        ArgumentNullException.ThrowIfNull(circle);
        ArgumentNullException.ThrowIfNull(lineStart);
        ArgumentNullException.ThrowIfNull(lineEnd);

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

    /// <summary>
    ///     Determines whether this circle fully encompasses another circle.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="other">
    ///     Another circle.
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
    public static bool Contains<TCircle>(this TCircle circle, TCircle other) where TCircle: ICircle
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(other);

        return circle.Radius >= (circle.Center.DistanceFrom(other.Center) + other.Radius);
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
    public static bool Contains<TCircle, TPoint>(this TCircle circle, TPoint point) where TCircle: ICircle
                                                                                    where TPoint: IPoint
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(point);

        return point.DistanceFrom(circle.Center) <= circle.Radius;
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
    public static float EdgeDistanceFrom<TCircle, TPoint>(this TCircle circle, TPoint other) where TCircle: ICircle
        where TPoint: IPoint
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(other);

        return Math.Max(0.0f, circle.Center.EuclideanDistanceFrom(other) - circle.Radius);
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
    public static float EdgeToEdgeDistanceFrom<TCircle>(this TCircle circle, TCircle other) where TCircle: ICircle
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(other);

        return Convert.ToInt32(Math.Max(0.0f, circle.Center.EuclideanDistanceFrom(other.Center) - circle.Radius - other.Radius));
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
    public static IEnumerable<Point> GetOutline<TCircle>(this TCircle circle) where TCircle: ICircle
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
    public static IEnumerable<Point> GetPoints<TCircle>(this TCircle circle) where TCircle: ICircle
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
    public static Point GetRandomPoint<TCircle>(this TCircle circle) where TCircle: ICircle
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
    public static bool Intersects<TCircle>(this TCircle circle, TCircle other) where TCircle: ICircle
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(other);

        return circle.Center.EuclideanDistanceFrom(other.Center) <= (circle.Radius + other.Radius);
    }
}