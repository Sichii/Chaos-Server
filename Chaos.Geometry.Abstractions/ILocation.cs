namespace Chaos.Geometry.Abstractions;

/// <summary>
///     Represents the combination of a map and coordinate-pair
/// </summary>
public interface ILocation : IPoint
{
    /// <summary>
    ///     A map unique id or name
    /// </summary>
    string Map { get; }
    
    static string ToString(ILocation location) => $"{location.Map}:{IPoint.ToString(location)}";
}