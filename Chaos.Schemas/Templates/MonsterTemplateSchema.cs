#region
using System.Text.Json.Serialization;
using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Schemas.Data;
#endregion

namespace Chaos.Schemas.Templates;

/// <summary>
///     Represents the serializable schema for a monster template
/// </summary>
public sealed record MonsterTemplateSchema
{
    /// <summary>
    ///     The amount of ability this monster will reward when killed
    /// </summary>
    public int AbilityReward { get; set; }

    /// <summary>
    ///     Defaults to 0
    ///     <br />
    ///     If specified, this will be aggressive and attack enemies if they come within the specified distance
    /// </summary>
    public int AggroRange { get; set; } = -1;

    /// <summary>
    ///     The number of milliseconds between usages of assails
    /// </summary>
    public int AssailIntervalMs { get; set; }

    /// <summary>
    ///     The amount of exp this monster will reward when killed
    /// </summary>
    public int ExpReward { get; set; }

    /// <summary>
    ///     A collection of loot table keys that this monster can drop from
    /// </summary>
    public ICollection<string> LootTableKeys { get; set; } = [];

    /// <summary>
    ///     Maximum amount of gold for this monster to drop
    /// </summary>
    public int MaxGoldDrop { get; set; }

    /// <summary>
    ///     Minimum amount of gold for this monster to drop
    /// </summary>
    public int MinGoldDrop { get; set; }

    /// <summary>
    ///     The number of milliseconds between movements while this monster is targeting an enemy
    /// </summary>
    public int MoveIntervalMs { get; set; }

    /// <summary>
    ///     The name of the monster when double clicked
    /// </summary>
    [JsonRequired]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     A collection of names of monsters scripts to attach to this monster
    ///     <br />
    ///     TODO: scripts section
    /// </summary>
    public ICollection<string> ScriptKeys { get; set; } = [];

    /// <summary>
    ///     A collection of key-value pairs of key-value pairs
    ///     <br />
    ///     Each script that has variables needs a scriptName-Value pair, and the value of that entry is a dictionary of
    ///     propertyName-Value pairs
    /// </summary>
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public IDictionary<string, DynamicVars> ScriptVars { get; set; }
        = new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     The number of milliseconds between usages of non-assail skills
    /// </summary>
    public int SkillIntervalMs { get; set; }

    /// <summary>
    ///     A collection of template keys of skills this monster will use
    /// </summary>
    public ICollection<string> SkillTemplateKeys { get; set; } = [];

    /// <summary>
    ///     The number of milliseconds between usages of spells
    /// </summary>
    public int SpellIntervalMs { get; set; }

    /// <summary>
    ///     A collection of template keys of spells this monster will cast
    /// </summary>
    public ICollection<string> SpellTemplateKeys { get; set; } = [];

    /// <summary>
    ///     The sprite id of the monster minus the offset
    /// </summary>
    public ushort Sprite { get; set; }

    /// <summary>
    ///     The base stats of this monster
    /// </summary>
    [JsonRequired]
    public StatSheetSchema StatSheet { get; set; } = null!;

    /// <summary>
    ///     A unique id specific to this monster template
    ///     <br />
    ///     Best practice is to match the name of the file
    /// </summary>
    [JsonRequired]
    public string TemplateKey { get; set; } = null!;

    /// <summary>
    ///     The monster's type
    ///     <br />
    ///     WhiteSquare has no additional functionality, it just appears as a white square on the tab map
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public CreatureType Type { get; set; }

    /// <summary>
    ///     The number of milliseconds between movements while this monster is wandering when it has no target
    /// </summary>
    public int WanderIntervalMs { get; set; }
}