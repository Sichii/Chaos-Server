using System.Text.Json.Serialization;
using Chaos.Collections.Common;
using Chaos.Geometry;

namespace Chaos.Schemas.Content;

/// <summary>
///     Represents the serializable schema of a reactor tile
/// </summary>
public sealed record ReactorTileSchema
{
    /// <summary>
    ///     If this reactor does damage, it is required to have an owner, otherwise this property can be ignored. The owning
    ///     monster can be a basic monster with no stats or scripts
    /// </summary>
    public string? OwnerMonsterTemplateKey { get; set; }

    /// <summary>
    ///     A collection of names of scripts to attach to this object by default
    /// </summary>
    public ICollection<string> ScriptKeys { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     A collection of key-value pairs of key-value pairs
    ///     <br />
    ///     Each script that has variables needs a scriptName-Value pair, and the value of that entry is a dictionary of
    ///     propertyName-Value pairs
    /// </summary>
    public IDictionary<string, DynamicVars> ScriptVars { get; set; }
        = new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Whether or not this reactor should block monster pathfinding. If this is set to false, monsters and merchants will
    ///     be able to step on this reactor
    /// </summary>
    public bool ShouldBlockPathfinding { get; set; }

    /// <summary>
    ///     The coordinates this reactor is located at
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Point Source { get; set; }
}