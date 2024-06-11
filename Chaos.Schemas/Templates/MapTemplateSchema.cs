using System.Text.Json.Serialization;

namespace Chaos.Schemas.Templates;

/// <summary>
///     Represents the serializable schema for a map template
/// </summary>
public sealed record MapTemplateSchema
{
    /// <summary>
    ///     The height of the map
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public byte Height { get; set; }

    /// <summary>
    ///     Default null. If specified, this is the type of light used for day/night cycle on this map
    /// </summary>
    public string? LightType { get; set; }

    /// <summary>
    ///     A collection of names of map scripts to attach to this map by default
    /// </summary>
    public ICollection<string> ScriptKeys { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     A unique id specific to this map template
    ///     <br />
    ///     This must match the name of the folder containing this file
    /// </summary>
    [JsonRequired]
    public string TemplateKey { get; set; } = null!;

    /// <summary>
    ///     The width of the map
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public byte Width { get; set; }
}