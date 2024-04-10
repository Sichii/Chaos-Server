using Chaos.Geometry.Abstractions;

namespace Chaos.Pathfinding.Abstractions;

/// <summary>
///     Defines the details needed to create a pathfinding grid
/// </summary>
public interface IGridDetails
{
    /// <summary>
    ///     A collection of points in the grid that represent blocking reactors
    /// </summary>
    ICollection<IPoint> BlockingReactors { get; }

    /// <summary>
    ///     The height of the grid
    /// </summary>
    int Height { get; }

    /// <summary>
    ///     A collection of points in the grid that represent walls
    /// </summary>

    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    ICollection<IPoint> Walls { get; }

    /// <summary>
    ///     The width of the grid
    /// </summary>
    int Width { get; }
}