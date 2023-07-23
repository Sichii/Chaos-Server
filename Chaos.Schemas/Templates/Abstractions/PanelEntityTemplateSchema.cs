using System.Text.Json.Serialization;
using Chaos.Collections.Common;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.Templates.Abstractions;

/// <summary>
///     Represents the serializable schema for a panel object template
/// </summary>
public abstract record PanelEntityTemplateSchema
{
    /// <summary>
    ///     Defaults to null<br />
    ///     If specified, this is the advanced class required to use this entity
    /// </summary>
    public AdvClass? AdvClass { get; set; }

    /// <summary>
    ///     Defaults to null<br />
    ///     If specified, this is the base class required to use this entity
    /// </summary>
    public BaseClass? Class { get; set; }
    /// <summary>
    ///     Defaults to null<br />
    ///     If specified, any on-use effect of this object will use this cooldown
    /// </summary>
    public int? CooldownMs { get; set; }

    /// <summary>
    ///     Defaults to null<br />
    ///     A brief description of this entity
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     The level required to use this entity
    /// </summary>
    public int Level { get; set; }
    /// <summary>
    ///     The base name of the entity
    /// </summary>
    [JsonRequired]
    public string Name { get; set; } = null!;
    /// <summary>
    ///     The sprite id used to display the entity in it's respective panel (no offset)
    /// </summary>
    public ushort PanelSprite { get; set; }

    /// <summary>
    ///     Whether or not this entity requires you to be a master
    /// </summary>
    public bool RequiresMaster { get; set; }

    /// <summary>
    ///     A collection of names of scripts to attach to this object by default. The name of a script is just the name of the
    ///     class without "Script" at the end.
    /// </summary>
    public ICollection<string> ScriptKeys { get; set; } = Array.Empty<string>();
    /// <summary>
    ///     A collection of key-value pairs of key-value pairs<br />
    ///     Each script that has variables needs a scriptName-Value pair, and the value of that entry is a dictionary of
    ///     propertyName-Value pairs
    /// </summary>
    public IDictionary<string, DynamicVars> ScriptVars { get; set; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);
    /// <summary>
    ///     A unique id specific to this template. This must match the file name
    /// </summary>
    [JsonRequired]
    public string TemplateKey { get; set; } = null!;
}