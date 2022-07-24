using Chaos.Geometry.Interfaces;

namespace Chaos.Geometry.Extensions;

public static class RectangleExtensions
{
    public static bool Contains(this IRectangle rect, IRectangle other)
    {
        if (rect == null)
            throw new ArgumentNullException(nameof(rect));

        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return (rect.Bottom >= other.Bottom) && (rect.Left >= other.Left) && (rect.Right <= other.Right) && (rect.Top <= other.Top);
    }
    
    public static bool Contains(this IRectangle rect, IPoint point)
    {
        if (rect == null)
            throw new ArgumentNullException(nameof(rect));

        if (point == null)
            throw new ArgumentNullException(nameof(point));

        return (rect.Left <= point.X) && (rect.Right >= point.X) && (rect.Top <= point.Y) && (rect.Bottom >= point.Y);
    }
    
    public static bool Intersects(this IRectangle rect, IRectangle other)
    {
        if (rect == null)
            throw new ArgumentNullException(nameof(rect));

        if (other == null)
            throw new ArgumentNullException(nameof(other));

        return !((rect.Bottom >= other.Top) || (rect.Left >= other.Right) || (rect.Right <= other.Left) || (rect.Top <= other.Bottom));
    }
    
    public static IEnumerable<Point> Points(this IRectangle rect)
    {
        if (rect == null)
            throw new ArgumentNullException(nameof(rect));
        
        for (var x = rect.Left; x <= rect.Right; x++)
            for (var y = rect.Top; y <= rect.Bottom; y++)
                yield return new Point(x, y);
    }
}