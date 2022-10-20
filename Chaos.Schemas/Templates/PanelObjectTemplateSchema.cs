using Chaos.Common.Collections;

namespace Chaos.Schemas.Templates;

public record PanelObjectTemplateSchema
{
    /// <summary>
    ///     Defaults to null<br />If specified, any on-use effect of this object will use this cooldown
    /// </summary>
    public required int? CooldownMs { get; init; }
    /// <summary>
    ///     The base name of the object
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    ///     The sprite id used to display the object in it's respective panel, minus the offset
    /// </summary>
    public required ushort PanelSprite { get; init; }
    /// <summary>
    ///     A collection of names of scripts to attach to this object by default
    /// </summary>
    public required ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    /// <summary>
    ///     A collection of key-value pairs of key-value pairs<br />Each script that has variables needs a scriptName-Value pair, and the value of
    ///     that entry is a dictionary of propertyName-Value pairs
    /// </summary>
    public required IDictionary<string, DynamicVars>? ScriptVars { get; init; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);
    /// <summary>
    ///     A unique id specific to this template. Best practice is to match the file name
    /// </summary>
    public required string TemplateKey { get; init; }
}