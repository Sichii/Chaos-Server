using Chaos.Geometry;
using Chaos.Geometry.Abstractions;

namespace Chaos.Extensions.Geometry;

/// <summary>
///     Provides extension methods for <see cref="Chaos.Geometry.Abstractions.IRectangle" />
/// </summary>
public static class RectangleExtensions
{
    /// <summary>
    ///     Determines whether the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" /> contains another
    ///     <see cref="Chaos.Geometry.Abstractions.IRectangle" />
    /// </summary>
    /// <param name="rect">The possibly outer rectangle</param>
    /// <param name="other">The possible inner rectangle</param>
    /// <returns><c>true</c> if <paramref name="rect" /> fully encompasses <paramref name="other" />, otherwise <c>false</c></returns>
    public static bool Contains(this IRectangle rect, IRectangle other)
    {
        ArgumentNullException.ThrowIfNull(rect);

        ArgumentNullException.ThrowIfNull(other);

        return (rect.Bottom >= other.Bottom) && (rect.Left <= other.Left) && (rect.Right >= other.Right) && (rect.Top <= other.Top);
    }

    /// <summary>
    ///     Determines whether the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" /> contains an
    ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
    /// </summary>
    /// <param name="rect">The rectangle to check</param>
    /// <param name="point">The point to check</param>
    /// <returns><c>true</c> if <paramref name="point" /> is inside of the <paramref name="rect" />, otherwise <c>false</c></returns>
    public static bool Contains(this IRectangle rect, IPoint point)
    {
        ArgumentNullException.ThrowIfNull(rect);

        ArgumentNullException.ThrowIfNull(point);

        return (rect.Left <= point.X) && (rect.Right >= point.X) && (rect.Top <= point.Y) && (rect.Bottom >= point.Y);
    }

    /// <summary>
    ///     Lazily generates points along the outline of the rectangle. The points will be in the order the vertices are listed.
    /// </summary>
    public static IEnumerable<Point> GetOutline(this IRectangle rect)
    {
        var vertices = rect.Vertices;

        for (var i = 0; i < vertices.Count - 1; i++)
        {
            var current = vertices[i];
            var next = vertices[i + 1];

            //skip the last point so the vertices are not included twice
            foreach (var point in current.GetDirectPath(next).SkipLast(1))
                yield return point;
        }
    }

    /// <summary>
    ///     Lazily generates all points inside of the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" />
    /// </summary>
    /// <param name="rect">The rectangle togenerate points for</param>
    public static IEnumerable<Point> GetPoints(this IRectangle rect)
    {
        ArgumentNullException.ThrowIfNull(rect);

        for (var x = rect.Left; x <= rect.Right; x++)
            for (var y = rect.Top; y <= rect.Bottom; y++)
                yield return new Point(x, y);
    }

    /// <summary>
    ///     Generates a random point inside the <see cref="Chaos.Geometry.Abstractions.IRectangle" />
    /// </summary>
    /// <param name="rect">The rect to use as bounds</param>
    public static Point GetRandomPoint(this IRectangle rect) => new(
        rect.Left + Random.Shared.Next(rect.Width),
        rect.Top + Random.Shared.Next(rect.Height));

    /// <summary>
    ///     Determines whether the specified <see cref="Chaos.Geometry.Abstractions.IRectangle" /> intersects another
    ///     <see cref="Chaos.Geometry.Abstractions.IRectangle" />
    /// </summary>
    /// <param name="rect">A rectangle</param>
    /// <param name="other">Another rectangle</param>
    /// <returns><c>true</c> if the rectangles intersect at any point or if either rect fully contains the other, otherwise <c>false</c></returns>
    public static bool Intersects(this IRectangle rect, IRectangle other)
    {
        ArgumentNullException.ThrowIfNull(rect);

        ArgumentNullException.ThrowIfNull(other);

        return !((rect.Bottom >= other.Top) || (rect.Left >= other.Right) || (rect.Right <= other.Left) || (rect.Top <= other.Bottom));
    }
}