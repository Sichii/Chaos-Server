using System.Text.Json.Serialization;
using Chaos.Collections.Common;

namespace Chaos.Schemas.Templates;

/// <summary>
///     The schema of a template for a reactor tile that can be dynamically placed in the world
/// </summary>
public sealed class ReactorTileTemplateSchema
{
    /// <summary>
    ///     A collection of names of scripts to attach to this object by default
    /// </summary>
    public ICollection<string> ScriptKeys { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     A collection of key-value pairs of key-value pairs<br />Each script that has variables needs a scriptName-Value pair, and the value of
    ///     that entry is a dictionary of propertyName-Value pairs
    /// </summary>
    public IDictionary<string, DynamicVars> ScriptVars { get; set; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Whether or not this reactor should block monster pathfinding. If this is set to false, monsters and merchants will be able to step on
    ///     this reactor.
    /// </summary>
    public bool ShouldBlockPathfinding { get; set; }

    /// <summary>
    ///     A unique id specific to this template. Best practice is to match the file name
    /// </summary>
    [JsonRequired]
    public string TemplateKey { get; set; } = null!;
}