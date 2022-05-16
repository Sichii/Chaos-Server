namespace Chaos.Core.Extensions;

public static class PointExtensions
{
    /// <summary>
    ///     Retreives a list of points in a line from the user, with an option for distance and direction. Direction.All is
    ///     optional. Direction.Invalid direction returns empty list.
    /// </summary>
    public static IEnumerable<Point> GetCardinalPoints(this Point start, int radius = 1, Direction direction = Direction.All)
    {
        if (direction == Direction.Invalid)
            yield break;

        for (var i = 1; i <= radius; i++)
            switch (direction)
            {
                case Direction.All:
                    yield return start.Offset(Direction.North, i);
                    yield return start.Offset(Direction.East, i);
                    yield return start.Offset(Direction.South, i);
                    yield return start.Offset(Direction.West, i);

                    break;
                default:
                    yield return start.Offset(direction, i);

                    break;
            }
    }

    /// <summary>
    ///     Creates an enumerable list of points representing a path between two given points, and returns it.
    /// </summary>
    /// <param name="start">Starting point for the creation of the path.</param>
    /// <param name="end">Ending point for the creation of the path.</param>
    public static IEnumerable<Point> GetDirectPath(this Point start, Point end)
    {
        yield return start;

        var current = start;

        while (current != end)
        {
            current = start.Offset(end.Relation(current));

            yield return current;
        }

        yield return end;
    }

    /// <summary>
    ///     Retreives a list of diagonal points in relevance to the user, with an optional distance and direction.
    ///     Direction.All is optional. Direction.Invalid direction returns empty list.
    /// </summary>
    public static IEnumerable<Point> GetInterCardinalPoints(this Point start, int radius = 1, Direction direction = Direction.All)
    {
        if (direction == Direction.Invalid)
            yield break;

        for (var i = 1; i <= radius; i++)
            switch (direction)
            {
                case Direction.North:
                    yield return (start.X - i, start.Y - i);
                    yield return (start.X + i, start.Y - i);

                    break;
                case Direction.East:
                    yield return (start.X + i, start.Y - i);
                    yield return (start.X + i, start.Y + i);

                    break;
                case Direction.South:
                    yield return (start.X + i, start.Y + i);
                    yield return (start.X - i, start.Y + i);

                    break;
                case Direction.West:
                    yield return (start.X - i, start.Y - i);
                    yield return (start.X - i, start.Y + i);

                    break;
                case Direction.All:
                    yield return (start.X - i, start.Y - i);
                    yield return (start.X + i, start.Y - i);
                    yield return (start.X + i, start.Y + i);
                    yield return (start.X - i, start.Y + i);

                    break;
                default:
                    yield break;
            }
    }

    public static IEnumerable<Point> SpiralSearch(this Point point, int maxRadius = byte.MaxValue)
    {
        var currentPoint = point;
        var radius = 1;

        for (; radius < maxRadius; radius++)
        {
            currentPoint = currentPoint.Offset(Direction.North);

            //travel from north to east
            while (point.X != currentPoint.X)
            {
                currentPoint = (currentPoint.X + 1, currentPoint.Y + 1);

                yield return currentPoint;
            }

            //travel from east to south
            while (point.Y != currentPoint.Y)
            {
                currentPoint = (currentPoint.X - 1, currentPoint.Y + 1);

                yield return currentPoint;
            }

            //travel from south to west
            while (point.X != currentPoint.X)
            {
                currentPoint = (currentPoint.X - 1, currentPoint.Y - 1);

                yield return currentPoint;
            }

            //travel from west to north
            while (point.Y != currentPoint.Y)
            {
                currentPoint = (currentPoint.X + 1, currentPoint.Y - 1);

                yield return currentPoint;
            }
        }
    }
}