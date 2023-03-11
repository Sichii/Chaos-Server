namespace Chaos.Geometry.Abstractions;

/// <summary>
///     Defines a combination of a map id and a coordinate pair
/// </summary>
public interface ILocation : IPoint
{
    /// <summary>
    ///     A map unique id or name
    /// </summary>
    string Map { get; }

    /// <inheritdoc cref="Object.ToString" />
    static string ToString(ILocation location) => $"{location.Map}:{IPoint.ToString(location)}";
}