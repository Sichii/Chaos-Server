namespace Chaos.Geometry.Abstractions;

/// <summary>
///     Defines a combination of a map id and a coordinate pair
/// </summary>
/// <implements><see cref="IPoint"/></implements>
public interface ILocation : IPoint
{
    /// <summary>
    ///     A map unique id or name
    /// </summary>
    string Map { get; }

    static string ToString(ILocation location) => $"{location.Map}:{IPoint.ToString(location)}";
}