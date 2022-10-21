using System.Text.Json.Serialization;
using Chaos.Common.Collections;

namespace Chaos.Schemas.Templates;

public record PanelObjectTemplateSchema
{
    /// <summary>
    ///     Defaults to null<br />If specified, any on-use effect of this object will use this cooldown
    /// </summary>
    public int? CooldownMs { get; init; }
    /// <summary>
    ///     The base name of the object
    /// </summary>
    [JsonRequired]
    public string Name { get; init; } = null!;
    /// <summary>
    ///     The sprite id used to display the object in it's respective panel, minus the offset
    /// </summary>
    public ushort PanelSprite { get; init; }
    /// <summary>
    ///     A collection of names of scripts to attach to this object by default
    /// </summary>
    public ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    /// <summary>
    ///     A collection of key-value pairs of key-value pairs<br />Each script that has variables needs a scriptName-Value pair, and the value of
    ///     that entry is a dictionary of propertyName-Value pairs
    /// </summary>
    public IDictionary<string, DynamicVars> ScriptVars { get; init; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);
    /// <summary>
    ///     A unique id specific to this template. Best practice is to match the file name
    /// </summary>
    [JsonRequired]
    public string TemplateKey { get; init; } = null!;
}