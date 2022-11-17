using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Extensions;

public static class AoeShapeExtensions
{
    public static IEnumerable<Point> ResolvePoints(
        this AoeShape aoeShape,
        IPoint source,
        int range = 1,
        Direction? direction = null,
        IRectangle? bounds = null,
        bool includeSource = false
    )
    {
        var sourcePoint = Point.From(source);
        IEnumerable<Point> points;

        switch (aoeShape)
        {
            case AoeShape.None:
                points = Enumerable.Empty<Point>();

                break;
            case AoeShape.Front:
            {
                ArgumentNullException.ThrowIfNull(direction, nameof(direction));

                var endPoint = sourcePoint.DirectionalOffset(direction.Value, range);

                points = sourcePoint.GetDirectPath(endPoint);

                break;
            }
            case AoeShape.AllAround:
                points = sourcePoint.SpiralSearch(range);

                break;
            case AoeShape.FrontalCone:
                ArgumentNullException.ThrowIfNull(direction, nameof(direction));

                points = sourcePoint.ConalSearch(direction.Value, range);

                break;
            case AoeShape.FrontalDiamond:
                ArgumentNullException.ThrowIfNull(direction, nameof(direction));

                points = sourcePoint.ConalSearch(direction.Value, range)
                                    .Where(p => p.WithinRange(sourcePoint, range));

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(aoeShape), aoeShape, null);
        }

        if (bounds != null)
            points = points.Where(p => bounds.Contains(p));

        if (includeSource)
            points = points.Prepend(sourcePoint);

        return points;
    }
}