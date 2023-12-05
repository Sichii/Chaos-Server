namespace Chaos.Geometry.Abstractions;

/// <summary>
///     Represents a pair of coordinates
/// </summary>
public interface IPoint
{
    /// <summary>
    ///     The X coordinate
    /// </summary>
    int X { get; }

    /// <summary>
    ///     The Y coordinate
    /// </summary>
    int Y { get; }

    /// <inheritdoc cref="Object.ToString" />
    static string ToString(IPoint point) => $"({point.X}, {point.Y})";
}