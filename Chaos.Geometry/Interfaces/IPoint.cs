namespace Chaos.Geometry.Interfaces;

public interface IPoint
{
    int X { get; }
    int Y { get; }

    static string ToString(IPoint point) => $@"({point.X}, {point.Y})";
}