using Chaos.Geometry.Definitions;

namespace Chaos.Geometry.Extensions;

public static class DirectionExtensions
{
    /// <summary>
    ///     Returns the Direction Enum equivalent of the reverse of a given cardinal direction.
    /// </summary>
    public static Direction Reverse(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Direction.Down;
            case Direction.Right:
                return Direction.Left;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            default:
                return Direction.Invalid;
        }
    }
    
    public static IEnumerable<Direction> AsEnumerable(this Direction direction)
    {
        if (direction == Direction.All)
            direction = Direction.Up;

        var dir = (int)direction;

        for (var i = 0; i < 4; i++)
        {
            yield return (Direction)dir;

            dir++;

            if (dir >= 4)
                dir -= 4;
        }
    }
}