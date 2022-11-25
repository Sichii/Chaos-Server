using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Extensions.Geometry;

/// <summary>
///     Provides extensions methods for <see cref="Chaos.Geometry.Abstractions.Definitions.Direction"/>
/// </summary>
public static class DirectionExtensions
{
    /// <summary>
    ///     Starting with the direction provided, enumerates all directions in clockwise order
    /// </summary>
    /// <param name="direction">The direction to start with</param>
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

    /// <summary>
    ///     Returns the <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" />s that would be to the sides of a given cardinal direction.
    /// </summary>
    public static (Direction side1, Direction side2) GetSideDirections(this Direction direction) => direction switch
    {
        Direction.Up    => (Direction.Left, Direction.Right),
        Direction.Right => (Direction.Up, Direction.Down),
        Direction.Down  => (Direction.Right, Direction.Left),
        Direction.Left  => (Direction.Down, Direction.Up),
        _               => (Direction.Invalid, Direction.Invalid)
    };

    /// <summary>
    ///     Returns the <see cref="Chaos.Geometry.Abstractions.Definitions.Direction" /> equivalent of the reverse of a given cardinal direction.
    /// </summary>
    public static Direction Reverse(this Direction direction) =>
        direction switch
        {
            Direction.Up    => Direction.Down,
            Direction.Right => Direction.Left,
            Direction.Down  => Direction.Up,
            Direction.Left  => Direction.Right,
            _               => Direction.Invalid
        };
}