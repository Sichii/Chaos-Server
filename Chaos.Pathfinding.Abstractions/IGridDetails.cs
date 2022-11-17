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
    ///     A collection of points in the grid that are walls
    /// </summary>
    ICollection<IPoint> Walls { get; }
    
    /// <summary>
    ///     The width of the grid
    /// </summary>
    int Width { get; }
}