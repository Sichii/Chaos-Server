namespace Chaos.Geometry.Abstractions;

/// <summary>
///     Represents a rectangle, a 4 sided polygon
/// </summary>
public interface IRectangle : IPolygon
{
    /// <summary>
    ///     The area of the rectangle
    /// </summary>
    int Area { get; }

    /// <summary>
    ///     The highest Y valye of the rectangle
    /// </summary>
    int Bottom { get; }

    /// <summary>
    ///     The difference between the highest and lowest Y value of the rectangle
    /// </summary>
    int Height { get; }

    /// <summary>
    ///     The lowest X value of the rectangle
    /// </summary>
    int Left { get; }

    /// <summary>
    ///     The highest X value of the rectangle
    /// </summary>
    int Right { get; }

    /// <summary>
    ///     The lowest Y value of the rectangle
    /// </summary>
    int Top { get; }

    /// <summary>
    ///     The difference between the highest and lowest X value of the rectangle
    /// </summary>
    int Width { get; }
}