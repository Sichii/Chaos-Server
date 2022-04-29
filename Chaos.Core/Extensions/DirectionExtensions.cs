using Chaos.Core.Definitions;

namespace Chaos.Core.Extensions;

public static class DirectionExtensions
{
    public static IEnumerable<Direction> AsEnumerable(this Direction direction)
    {
        if (direction == Direction.All)
            direction = Direction.North;

        var dir = (int)direction;

        for (var i = 0; i < 4; i++)
        {
            yield return (Direction)dir;

            dir++;

            if (dir >= 4)
                dir -= 4;
        }
    }
    
    public static IEnumerable<T> Flatten<T>(this T[,] map)
    {
        for (var x = 0; x < map.GetLength(0); x++)
            for (var y = 0; y < map.GetLength(1); y++)
                yield return map[x, y];
    }

    public static IEnumerable<T> Flatten<T>(this T[][] map)
    {
        for (var x = 0; x < map.Length; x++)
        {
            var arr = map[x];

            for (var y = 0; y < arr.Length; y++)
                yield return arr[y];
        }
    }

    /// <summary>
    ///     Returns the Direction Enum equivalent of the reverse of a given cardinal direction.
    /// </summary>
    public static Direction Reverse(this Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return Direction.South;
            case Direction.East:
                return Direction.West;
            case Direction.South:
                return Direction.North;
            case Direction.West:
                return Direction.East;
            default:
                return Direction.Invalid;
        }
    }
}