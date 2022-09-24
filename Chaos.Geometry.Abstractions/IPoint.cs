namespace Chaos.Geometry.Abstractions;

public interface IPoint
{
    int X { get; }
    int Y { get; }

    static string ToString(IPoint point) => $@"({point.X}, {point.Y})";
}