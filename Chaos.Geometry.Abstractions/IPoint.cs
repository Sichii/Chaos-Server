namespace Chaos.Geometry.Abstractions;

/// <summary>
///     Represents a pair of coordinates
/// </summary>
public interface IPoint
{
    int X { get; }
    int Y { get; }

    static string ToString(IPoint point) => $@"({point.X}, {point.Y})";
}