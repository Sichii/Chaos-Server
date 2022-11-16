namespace Chaos.Geometry.Abstractions.Definitions;

public enum Direction : byte
{
    /// <summary>
    ///     Up arrow key
    /// </summary>
    Up = 0,
    /// <summary>
    ///     Right arrow key
    /// </summary>
    Right = 1,
    /// <summary>
    ///     Down arrow key
    /// </summary>
    Down = 2,
    /// <summary>
    ///     Left arrow key
    /// </summary>
    Left = 3,
    /// <summary>
    ///     All directions
    /// </summary>
    All = 4,
    /// <summary>
    ///     No direction
    /// </summary>
    Invalid = 255
}