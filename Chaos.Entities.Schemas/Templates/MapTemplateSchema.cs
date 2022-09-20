using Chaos.Geometry;

namespace Chaos.Entities.Schemas.Templates;

public record MapTemplateSchema
{
    /// <summary>
    ///     The height of the map
    /// </summary>
    public required byte Height { get; init; }
    /// <summary>
    ///     A collection of names of map scripts to attach to this map by default
    /// </summary>
    public required ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    /// <summary>
    ///     A unique id specific to this map template<br />Best practice is to match the name of the file, and use the numeric id the map this
    ///     template is for
    /// </summary>
    public required string TemplateKey { get; init; }
    /// <summary>
    ///     The coordinates of each warp tile on the map
    /// </summary>
    public required Point[] WarpPoints { get; init; } = Array.Empty<Point>();
    /// <summary>
    ///     The width of the map
    /// </summary>
    public required byte Width { get; init; }
}