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
    /// <param name="circle">The circle.</param>
    /// <param name="lineStart">The start point of the line.</param>
    /// <param name="lineEnd">The end point of the line.</param>
    /// <returns>The first point of intersection between the line and the circle, or null if they do not intersect.</returns>
    public static Point? CalculateIntersectionEntryPoint(this ICircle circle, IPoint lineStart, IPoint lineEnd)
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(lineStart);

        ArgumentNullException.ThrowIfNull(lineEnd);

        var center = circle.Center;
        var radius = circle.Radius;

        // Calculate the line vector
        var lineX = lineEnd.X - lineStart.X;
        var lineY = lineEnd.Y - lineStart.Y;

        // Calculate the vector from the center of the circle to the start of the line
        var circleToLineX = lineStart.X - center.X;
        var circleToLineY = lineStart.Y - center.Y;

        // Calculate the dot product of the line and the vector from the center of the circle to the start of the line
        var dotProduct = lineX * circleToLineX + lineY * circleToLineY;

        // Calculate the squared length of the line
        var lineLengthSquared = lineX * lineX + lineY * lineY;

        // Calculate the squared distance from the center of the circle to the line
        var distanceSquared = circleToLineX * circleToLineX + circleToLineY * circleToLineY - dotProduct * dotProduct / lineLengthSquared;

        // If the distance is greater than the radius, the line does not intersect the circle
        if (distanceSquared > radius * radius)
            return null;

        // Calculate the distance along the line from the start to the point of intersection
        var distanceAlongLine = (int)Math.Sqrt(radius * radius - distanceSquared);

        // Calculate the coordinates of the first point of intersection
        var intersectionX =
            lineStart.X + lineX * dotProduct / lineLengthSquared - distanceAlongLine * lineY / (int)Math.Sqrt(lineLengthSquared);

        var intersectionY =
            lineStart.Y + lineY * dotProduct / lineLengthSquared + distanceAlongLine * lineX / (int)Math.Sqrt(lineLengthSquared);

        return new Point(intersectionX, intersectionY);
    }

    /// <summary>
    ///     Determines whether this circle fully encompasses another circle.
    /// </summary>
    /// <param name="circle">This circle.</param>
    /// <param name="other">Another circle.</param>
    /// <returns><c>true</c> if this circle fully encompasses the other (or edges touch); otherwise, <c>false</c>.</returns>
    /// <exception cref="System.ArgumentNullException">circle</exception>
    /// <exception cref="System.ArgumentNullException">other</exception>
    public static bool Contains(this ICircle circle, ICircle other)
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(other);

        return circle.Radius >= circle.EdgeToEdgeDistanceFrom(other) + other.Radius;
    }

    /// <summary>
    ///     Determines whether this circle contains the given point.
    /// </summary>
    /// <param name="circle">This circle.</param>
    /// <param name="point">A point.</param>
    /// <returns><c>true</c> if this circle contains the point, otherwise <c>false</c>.</returns>
    /// <exception cref="System.ArgumentNullException">circle</exception>
    /// <exception cref="System.ArgumentNullException">point</exception>
    public static bool Contains(this ICircle circle, IPoint point)
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(point);

        return point.DistanceFrom(circle.Center) <= circle.Radius;
    }

    /// <summary>
    ///     Calculates the edge-to-center euclidean distance to some center-point.
    /// </summary>
    /// <param name="circle">This circle.</param>
    /// <param name="other">A center-point of some entity.</param>
    /// <returns>
    ///     <see cref="float" />
    ///     <br />
    ///     The euclidean distance between the center-point of this circle and the some other point, minus this circle's
    ///     radius.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">circle</exception>
    /// <exception cref="System.ArgumentNullException">other</exception>
    public static float EdgeDistanceFrom(this ICircle circle, IPoint other)
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(other);

        return Math.Max(0, circle.Center.DistanceFrom(other) - circle.Radius);
    }

    /// <summary>
    ///     Calculates the edge-to-edge euclidean distance to another circle.
    /// </summary>
    /// <param name="circle">This circle.</param>
    /// <param name="other">Another circle.</param>
    /// <returns>
    ///     <see cref="float" />
    ///     <br />
    ///     The euclidean distance between the centerpoints of two circles, minus the sum of their radi.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">circle</exception>
    /// <exception cref="System.ArgumentNullException">other</exception>
    public static float EdgeToEdgeDistanceFrom(this ICircle circle, ICircle other)
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(other);

        return Math.Max(0, circle.Center.DistanceFrom(other.Center) - circle.Radius - other.Radius);
    }

    /// <summary>
    ///     Generates a sequence of point along the circumference of this circle
    /// </summary>
    /// <param name="circle">This circle.</param>
    /// <returns>A sequence of point along the circumfnerence of the circle</returns>
    /// <exception cref="System.ArgumentNullException">circle</exception>
    public static IEnumerable<Point> GetOutline(this ICircle circle)
    {
        ArgumentNullException.ThrowIfNull(circle);

        var x = circle.Center.X;
        var y = circle.Center.Y;
        var xOffset = circle.Radius;
        var yOffset = 0;
        var decisionOver2 = 1 - xOffset;

        while (yOffset <= xOffset)
        {
            yield return new Point(x + xOffset, y + yOffset);
            yield return new Point(x + yOffset, y + xOffset);
            yield return new Point(x - yOffset, y + xOffset);
            yield return new Point(x - xOffset, y + yOffset);
            yield return new Point(x - xOffset, y - yOffset);
            yield return new Point(x - yOffset, y - xOffset);
            yield return new Point(x + yOffset, y - xOffset);
            yield return new Point(x + xOffset, y - yOffset);

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
    /// <param name="circle"></param>
    /// <returns><see cref="IEnumerable{T}" /> of <see cref="Point" /></returns>
    /// <exception cref="System.ArgumentNullException">circle</exception>
    public static IEnumerable<Point> GetPoints(this ICircle circle)
    {
        ArgumentNullException.ThrowIfNull(circle);

        var centerX = circle.Center.X;
        var centerY = circle.Center.Y;
        var radiusSqrd = circle.Radius * circle.Radius;

        for (var x = centerX - circle.Radius; x <= centerX; x++)
            for (var y = centerY - circle.Radius; y <= centerY; y++)
            {
                var xdc = x - centerX;
                var ydc = y - centerY;

                if (xdc * xdc + ydc * ydc <= radiusSqrd)
                {
                    var xS = centerX - xdc;
                    var yS = centerY - ydc;

                    yield return new Point(x, y);
                    yield return new Point(x, yS);
                    yield return new Point(xS, y);
                    yield return new Point(xS, yS);
                }
            }
    }

    /// <summary>
    ///     Gets a random point within this circle.
    /// </summary>
    /// <param name="circle">The circle</param>
    /// <returns></returns>
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
    /// <param name="circle">This circle.</param>
    /// <param name="other">Another circle.</param>
    /// <returns><c>true</c> if this circle intersects the <paramref name="other" />, <c>false</c> otherwise.</returns>
    /// <exception cref="System.ArgumentNullException">circle</exception>
    /// <exception cref="System.ArgumentNullException">other</exception>
    public static bool Intersects(this ICircle circle, ICircle other)
    {
        ArgumentNullException.ThrowIfNull(circle);

        ArgumentNullException.ThrowIfNull(other);

        return circle.Center.DistanceFrom(other.Center) <= circle.Radius + other.Radius;
    }
}