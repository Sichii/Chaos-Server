namespace Chaos.Geometry.Abstractions.Definitions;

/// <summary>
///     Defines the possible directions an object can face, and their networking byte values
/// </summary>
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

/// <summary>
///     Defines the possible distance types for distance checking
/// </summary>
public enum DistanceType : byte
{
    /// <summary>
    ///     Manhattan distance
    /// </summary>
    Manhattan = 0,

    /// <summary>
    ///     Euclidean distance
    /// </summary>
    Euclidean = 1
}