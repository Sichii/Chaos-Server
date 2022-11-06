using Chaos.Geometry.Abstractions;

namespace Chaos.Extensions.Geometry;

public static class PolygonExtensions
{
    public static bool Contains(this IPolygon polygon, IPoint point)
    {
        var inside = false;
        var count = polygon.Vertices.Count;

        for (int i = 0,
                 j = count - 1;
             i < count;
             j = i++)
        {
            var iVertex = polygon.Vertices[i];
            var jVertex = polygon.Vertices[j];

            //long form version of pnpoly, allowing for fast fails
            if ((((iVertex.Y < point.Y) && (jVertex.Y >= point.Y)) || ((jVertex.Y < point.Y) && (iVertex.Y >= point.Y)))
                && ((iVertex.X <= point.X) || (jVertex.X <= point.X)))
                inside ^= iVertex.X + (point.Y - iVertex.Y) / (jVertex.Y - iVertex.Y) * (jVertex.X - iVertex.X) < point.X;
        }

        return inside;
    }
}