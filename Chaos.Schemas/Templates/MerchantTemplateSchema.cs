using System.Text.Json.Serialization;
using Chaos.Common.Collections;

namespace Chaos.Schemas.Templates;

public sealed record MerchantTemplateSchema
{
    /// <summary>
    ///     The unique id of a dialog that should be shown when this merchant is activated
    /// </summary>
    public string? DialogKey { get; set; }
    /// <summary>
    ///     The name of this merchant as displayed to aislings
    /// </summary>
    [JsonRequired]
    public string Name { get; set; } = null!;
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
    ///     The sprite id of the merchant minus the offset
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public ushort Sprite { get; set; }

    /// <summary>
    ///     A unique id specific to this template. Best practice is to match the file name
    /// </summary>
    [JsonRequired]
    public string TemplateKey { get; set; } = null!;
}