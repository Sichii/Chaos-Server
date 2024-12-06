#region
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
#endregion

namespace Chaos.Extensions;

public class AoeShapeOptions
{
    public IRectangle? Bounds { get; init; }
    public Direction? Direction { get; init; }
    public int? ExclusionRange { get; init; }
    public int Range { get; init; }
    public required IPoint Source { get; init; }
}

public sealed class CascadingAoeShapeOptions : AoeShapeOptions
{
    public required ICollection<Point> AllPossiblePoints { get; init; }
}

public static class AoeShapeExtensions
{
    /// <summary>
    ///     Resolves the points for the specified <see cref="AoeShape" /> given the specified options.
    /// </summary>
    /// <param name="aoeShape">
    ///     The shape of the aoe
    /// </param>
    /// <param name="options">
    ///     Additional options
    /// </param>
    /// <returns>
    ///     Distinct points
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    public static IEnumerable<Point> ResolvePoints(this AoeShape aoeShape, AoeShapeOptions options)
    {
        var sourcePoint = Point.From(options.Source);
        IEnumerable<Point> points;

        switch (aoeShape)
        {
            case AoeShape.None:
                points = [];

                break;
            case AoeShape.Front:
            {
                if (!options.Direction.HasValue)
                    throw new ArgumentNullException(nameof(options.Direction));

                var endPoint = sourcePoint.DirectionalOffset(options.Direction.Value, options.Range);

                points = sourcePoint.GetDirectPath(endPoint);

                break;
            }
            case AoeShape.AllAround:
            {
                points = sourcePoint.SpiralSearch(options.Range);

                break;
            }
            case AoeShape.FrontalCone:
            {
                if (!options.Direction.HasValue)
                    throw new ArgumentNullException(nameof(options.Direction));

                points = sourcePoint.ConalSearch(options.Direction.Value, options.Range);

                break;
            }
            case AoeShape.FrontalDiamond:
            {
                if (!options.Direction.HasValue)
                    throw new ArgumentNullException(nameof(options.Direction));

                points = sourcePoint.ConalSearch(options.Direction.Value, options.Range)
                                    .Where(p => p.WithinRange(sourcePoint, options.Range));

                break;
            }
            case AoeShape.Circle:
            {
                var circle = new Circle(sourcePoint, options.Range);

                points = circle.GetPoints();

                break;
            }
            case AoeShape.Square:
            {
                var rectangle = new Rectangle(sourcePoint, options.Range * 2 + 1, options.Range * 2 + 1);

                points = rectangle.GetPoints();

                break;
            }
            case AoeShape.CircleOutline:
            {
                var circle = new Circle(sourcePoint, options.Range);

                points = circle.GetOutline();

                break;
            }
            case AoeShape.SquareOutline:
            {
                var rectangle = new Rectangle(sourcePoint, options.Range * 2 + 1, options.Range * 2 + 1);

                points = rectangle.GetOutline();

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(aoeShape), aoeShape, null);
        }

        if (options.Bounds != null)
            points = points.Where(options.Bounds.Contains);

        if (!options.ExclusionRange.HasValue)
            points = points.Prepend(sourcePoint);
        else
            switch (aoeShape)
            {
                case AoeShape.Square:
                case AoeShape.SquareOutline:
                {
                    var rectangle = new Rectangle(sourcePoint, options.ExclusionRange.Value * 2 + 1, options.ExclusionRange.Value * 2 + 1);

                    points = points.Where(pt => !rectangle.Contains(pt));

                    break;
                }
                case AoeShape.Circle:
                case AoeShape.CircleOutline:
                {
                    var circle = new Circle(sourcePoint, options.ExclusionRange.Value);

                    points = points.Where(pt => !circle.Contains(pt));

                    break;
                }
                default:
                {
                    points = points.Where(pt => pt.ManhattanDistanceFrom(sourcePoint) > options.ExclusionRange.Value);

                    break;
                }
            }

        return points.Distinct();
    }

    /// <summary>
    ///     Resolves the points for the specified <see cref="AoeShape" /> given the specified options. This is used by
    ///     Cascading AOE shapes to get the points for a specific stage of the cascade.
    /// </summary>
    /// <param name="aoeShape">
    ///     The shape of the aoe
    /// </param>
    /// <param name="options">
    ///     Additional options
    /// </param>
    /// <returns>
    ///     Distinct points
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    public static IEnumerable<Point> ResolvePointsForRange(this AoeShape aoeShape, CascadingAoeShapeOptions options)
    {
        var sourcePoint = Point.From(options.Source);

        switch (aoeShape)
        {
            case AoeShape.None:
                return [];
            case AoeShape.AllAround:
            case AoeShape.Front:
            case AoeShape.FrontalDiamond:
            {
                return options.AllPossiblePoints.Where(pt => pt.ManhattanDistanceFrom(sourcePoint) == options.Range);
            }
            case AoeShape.FrontalCone:
            {
                var travelsOnXAxis = options.Direction is Direction.Left or Direction.Right;
                var nextOffset = sourcePoint.DirectionalOffset(options.Direction!.Value, options.Range);

                return options.AllPossiblePoints.Where(pt => travelsOnXAxis ? pt.X == nextOffset.X : pt.Y == nextOffset.Y);
            }
            case AoeShape.Circle:
            {
                var circle = new Circle(sourcePoint, options.Range);

                var outline = circle.GetOutline()
                                    .ToList();

                return options.AllPossiblePoints.Where(pt => outline.Contains(pt));
            }
            case AoeShape.Square:
            {
                var rectangle = new Rectangle(sourcePoint, options.Range * 2 + 1, options.Range * 2 + 1);

                var outline = rectangle.GetOutline()
                                       .ToList();

                return options.AllPossiblePoints.Where(pt => outline.Contains(pt));
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}