#region
using System.Runtime.CompilerServices;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
#endregion

namespace Chaos.Extensions.Geometry;

/// <summary>
///     Provides extension methods for <see cref="Chaos.Geometry.Abstractions.IPolygon" />
/// </summary>
public static class PolygonExtensions
{
    #region Polygon Contains Point (PNPOLY)
    /// <inheritdoc cref="Contains(IPolygon, IPoint)" />
    public static bool Contains(this ValuePolygon polygon, ValuePoint point)
    {
        var inside = false;
        var vertices = polygon.Vertices;
        var count = vertices.Count;

        for (int i = 0,
                 j = count - 1;
             i < count;
             j = i++)
        {
            var iVertex = vertices[i];
            var jVertex = vertices[j];

            //long form version of pnpoly, allowing for fast fails
            if ((((iVertex.Y < point.Y) && (jVertex.Y >= point.Y)) || ((jVertex.Y < point.Y) && (iVertex.Y >= point.Y)))
                && ((iVertex.X <= point.X) || (jVertex.X <= point.X)))
                inside ^= (iVertex.X + (point.Y - iVertex.Y) / (jVertex.Y - iVertex.Y) * (jVertex.X - iVertex.X)) < point.X;
        }

        return inside;
    }

    /// <inheritdoc cref="Contains(IPolygon, IPoint)" />
    public static bool Contains(this ValuePolygon polygon, Point point)
    {
        var inside = false;
        var vertices = polygon.Vertices;
        var count = vertices.Count;

        for (int i = 0,
                 j = count - 1;
             i < count;
             j = i++)
        {
            var iVertex = vertices[i];
            var jVertex = vertices[j];

            //long form version of pnpoly, allowing for fast fails
            if ((((iVertex.Y < point.Y) && (jVertex.Y >= point.Y)) || ((jVertex.Y < point.Y) && (iVertex.Y >= point.Y)))
                && ((iVertex.X <= point.X) || (jVertex.X <= point.X)))
                inside ^= (iVertex.X + (point.Y - iVertex.Y) / (jVertex.Y - iVertex.Y) * (jVertex.X - iVertex.X)) < point.X;
        }

        return inside;
    }

    /// <inheritdoc cref="Contains(IPolygon, IPoint)" />
    public static bool Contains(this IPolygon polygon, Point point)
    {
        var inside = false;
        var vertices = polygon.Vertices;
        var count = vertices.Count;

        for (int i = 0,
                 j = count - 1;
             i < count;
             j = i++)
        {
            var iVertex = vertices[i];
            var jVertex = vertices[j];

            //long form version of pnpoly, allowing for fast fails
            if ((((iVertex.Y < point.Y) && (jVertex.Y >= point.Y)) || ((jVertex.Y < point.Y) && (iVertex.Y >= point.Y)))
                && ((iVertex.X <= point.X) || (jVertex.X <= point.X)))
                inside ^= (iVertex.X + (point.Y - iVertex.Y) / (jVertex.Y - iVertex.Y) * (jVertex.X - iVertex.X)) < point.X;
        }

        return inside;
    }

    /// <summary>
    ///     Determines whether the given <see cref="Chaos.Geometry.Abstractions.IPolygon" /> contains the given
    ///     <see cref="Chaos.Geometry.Abstractions.IPoint" />
    /// </summary>
    /// <param name="polygon">
    ///     The polygon to check
    /// </param>
    /// <param name="point">
    ///     The point to check
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the polygon contains the point, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    /// <remarks>
    ///     This is a long-form implementation of pnpoly that allows for fast-path failures
    ///     <br />
    ///     https://wrfranklin.org/Research/Short_Notes/pnpoly.html
    ///     <br />
    ///     Vertices may be in either clock-wise or counter-clockwise order, but the vertices must be in one of those orders
    ///     <br />
    ///     The shape may be either convex or concave
    ///     <br />
    ///     The shape may contains holes, but please read the above link for further details about that
    ///     <br />
    ///     <br />
    ///     Copyright (c) 1970-2003, Wm. Randolph Franklin
    ///     <br />
    ///     <br />
    ///     Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
    ///     documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
    ///     rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
    ///     permit persons to whom the Software is furnished to do so, subject to the following conditions:
    ///     <br />
    ///     <br />
    ///     Redistributions of source code must retain the above copyright notice, this list of conditions and the following
    ///     disclaimers.
    ///     <br />
    ///     Redistributions in binary form must reproduce the above copyright notice in the documentation and/or other
    ///     materials provided with the distribution.
    ///     <br />
    ///     The name of W. Randolph Franklin may not be used to endorse or promote products derived from this Software without
    ///     specific prior written permission.
    ///     <br />
    ///     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
    ///     THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ///     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
    ///     TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ///     SOFTWARE.
    /// </remarks>
    public static bool Contains(this IPolygon polygon, IPoint point)
    {
        ArgumentNullException.ThrowIfNull(point);

        var inside = false;
        var vertices = polygon.Vertices;
        var count = vertices.Count;

        for (int i = 0,
                 j = count - 1;
             i < count;
             j = i++)
        {
            var iVertex = vertices[i];
            var jVertex = vertices[j];

            //long form version of pnpoly, allowing for fast fails
            if ((((iVertex.Y < point.Y) && (jVertex.Y >= point.Y)) || ((jVertex.Y < point.Y) && (iVertex.Y >= point.Y)))
                && ((iVertex.X <= point.X) || (jVertex.X <= point.X)))
                inside ^= (iVertex.X + (point.Y - iVertex.Y) / (jVertex.Y - iVertex.Y) * (jVertex.X - iVertex.X)) < point.X;
        }

        return inside;
    }
    #endregion

    #region Polygon GetOutline
    /// <inheritdoc cref="GetOutline(IPolygon)" />
    public static IEnumerable<Point> GetOutline(this ValuePolygon polygon)
    {
        var vertices = polygon.Vertices;

        return InnerGetOutline(vertices);

        static IEnumerable<Point> InnerGetOutline(IReadOnlyList<IPoint> localVertices)
        {
            for (var i = 0; i < (localVertices.Count - 1); i++)
            {
                var current = localVertices[i];
                var next = localVertices[i + 1];

                //skip the last point so the vertices are not included twice
                foreach (var point in current.RayTraceTo(next)
                                             .SkipLast(1))
                    yield return point;
            }

            //include the last point
            foreach (var point in localVertices[^1]
                                  .RayTraceTo(localVertices[0])
                                  .SkipLast(1))
                yield return point;
        }
    }

    /// <summary>
    ///     Lazily generates points along the outline of any kind of polygon. The vertices may be in clockwise or
    ///     counter-clockwise order, but they must be in one of those orders
    /// </summary>
    public static IEnumerable<Point> GetOutline(this IPolygon polygon)
    {
        var vertices = polygon.Vertices;

        for (var i = 0; i < (vertices.Count - 1); i++)
        {
            var current = vertices[i];
            var next = vertices[i + 1];

            //skip the last point so the vertices are not included twice
            foreach (var point in current.RayTraceTo(next)
                                         .SkipLast(1))
                yield return point;
        }

        //include the last point
        foreach (var point in vertices[^1]
                              .RayTraceTo(vertices[0])
                              .SkipLast(1))
            yield return point;
    }
    #endregion
}