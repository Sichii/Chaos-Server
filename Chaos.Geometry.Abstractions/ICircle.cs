namespace Chaos.Geometry.Abstractions;

/// <summary>
///     Represents a circle in two-dimensional space.
/// </summary>
public interface ICircle
{
    /// <summary>
    ///     The center point of the circle.
    /// </summary>
    IPoint Center { get; }

    /// <summary>
    ///     The radius of the circle.
    /// </summary>
    int Radius { get; }
}