using System.Text.Json.Serialization;
using Chaos.Geometry;

namespace Chaos.Schemas.Templates;

public sealed record MapTemplateSchema
{
    /// <summary>
    ///     The height of the map
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public byte Height { get; init; }
    /// <summary>
    ///     A collection of names of map scripts to attach to this map by default
    /// </summary>
    public ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    /// <summary>
    ///     A unique id specific to this map template<br />Best practice is to match the name of the file, and use the numeric id the map this
    ///     template is for
    /// </summary>
    [JsonRequired]
    public string TemplateKey { get; init; } = null!;
    /// <summary>
    ///     The coordinates of each warp tile on the map
    /// </summary>
    public Point[] WarpPoints { get; init; } = Array.Empty<Point>();
    /// <summary>
    ///     The width of the map
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public byte Width { get; init; }
}