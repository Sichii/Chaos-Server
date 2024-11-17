#region
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
#endregion

namespace Chaos.Extensions;

public static class AoeShapeExtensions
{
    public static IEnumerable<Point> ResolvePoints(
        this AoeShape aoeShape,
        IPoint source,
        int range = 1,
        Direction? direction = null,
        IRectangle? bounds = null,
        bool excludeSource = false)
    {
        var sourcePoint = Point.From(source);
        IEnumerable<Point> points;

        switch (aoeShape)
        {
            case AoeShape.None:
                points = [];

                break;
            case AoeShape.Front:
            {
                ArgumentNullException.ThrowIfNull(direction, nameof(direction));

                var endPoint = sourcePoint.DirectionalOffset(direction.Value, range);

                points = sourcePoint.GetDirectPath(endPoint)
                                    .Skip(1);

                break;
            }
            case AoeShape.AllAround:
                points = sourcePoint.SpiralSearch(range)
                                    .Skip(1);

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
            points = points.Where(bounds.Contains);

        if (!excludeSource)
            points = points.Prepend(sourcePoint);

        return points.Distinct();
    }

    public static IEnumerable<IPoint> ResolvePointsForRange(
        this AoeShape shape,
        IPoint source,
        Direction aoeDirection,
        int range,
        IEnumerable<IPoint> allPossiblePoints)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(allPossiblePoints);

        switch (shape)
        {
            case AoeShape.None:
                return [];
            case AoeShape.AllAround:
            case AoeShape.Front:
            case AoeShape.FrontalDiamond:
                return allPossiblePoints.Where(pt => pt.ManhattanDistanceFrom(source) == range);

            case AoeShape.FrontalCone:
                var travelsOnXAxis = aoeDirection is Direction.Left or Direction.Right;
                var nextOffset = source.DirectionalOffset(aoeDirection, range);

                return allPossiblePoints.Where(pt => travelsOnXAxis ? pt.X == nextOffset.X : pt.Y == nextOffset.Y);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}