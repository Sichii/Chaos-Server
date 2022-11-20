using Chaos.Geometry.Abstractions;

namespace Chaos.Pathfinding.Abstractions;

/// <summary>
///     Defines the details needed to create a pathfinding grid
/// </summary>
public interface IGridDetails
{
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
    ///     A collection of points in the grid that should never be pathed over 
    /// </summary>
    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    ICollection<IPoint> Blacklist { get; }

    /// <summary>
    ///     The width of the grid
    /// </summary>
    int Width { get; }
}