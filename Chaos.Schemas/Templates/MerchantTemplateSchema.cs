using System.Text.Json.Serialization;
using Chaos.Collections.Common;
using Chaos.Schemas.Data;

namespace Chaos.Schemas.Templates;

/// <summary>
///     Represents the serializable schema for a merchant template
/// </summary>
public sealed record MerchantTemplateSchema
{
    /// <summary>
    ///     A collection of items that this merchant will sell. If stock is set to -1, the merchant will have infinite stock of
    ///     it.
    /// </summary>
    public ICollection<ItemDetailsSchema> ItemsForSale { get; set; } = Array.Empty<ItemDetailsSchema>();

    /// <summary>
    ///     A collection of items(Item Template Keys) that this merchant will buy
    /// </summary>
    public ICollection<string> ItemsToBuy { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     The name of this merchant as displayed to aislings
    /// </summary>
    [JsonRequired]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Defaults to 6 hours. If this merchant sells items, this specifies how often the merchant will restock those items
    /// </summary>
    public int RestockIntervalHrs { get; set; } = 6;

    /// <summary>
    ///     Defaults to 100%. If this merchant sells items, this specifies the percentage of items that will be restocked
    /// </summary>
    public int RestockPct { get; set; } = 100;

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
    ///     A collection of skills (Skill Template Keys) that this merchant will teach
    /// </summary>
    public ICollection<string> SkillsToTeach { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     A collection of spells (Spell Template Keys) that this merchant will teach
    /// </summary>
    public ICollection<string> SpellsToTeach { get; set; } = Array.Empty<string>();

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

    /// <summary>
    ///     The number of milliseconds between movements while this merchant is wandering
    /// </summary>
    public int WanderIntervalMs { get; set; }
}