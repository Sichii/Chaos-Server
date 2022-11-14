using System.Text.Json.Serialization;
using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Schemas.Aisling;

namespace Chaos.Schemas.Templates;

public sealed record MonsterTemplateSchema
{
    /// <summary>
    ///     The number of milliseconds between usages of assails
    /// </summary>
    public int AssailIntervalMs { get; init; }
    /// <summary>
    ///     The initial direction of the monster when spawned
    /// </summary>
    public Direction Direction { get; init; } = (Direction)Random.Shared.Next(4);
    /// <summary>
    ///     The number of milliseconds between movements while this monster is targeting an enemy
    /// </summary>
    public int MoveIntervalMs { get; init; }
    /// <summary>
    ///     The name of the monster when double clicked
    /// </summary>
    [JsonRequired]
    public string Name { get; init; } = null!;
    /// <summary>
    ///     A collection of names of monsters scripts to attach to this monster<br />TODO: scripts section
    /// </summary>
    public ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    public IDictionary<string, DynamicVars> ScriptVars { get; init; } =
        new Dictionary<string, DynamicVars>(StringComparer.OrdinalIgnoreCase);
    /// <summary>
    ///     The number of milliseconds between usages of non-assail skills
    /// </summary>
    public int SkillIntervalMs { get; init; }
    /// <summary>
    ///     A collection of template keys of skills this monster will use
    /// </summary>
    public ICollection<string> SkillTemplateKeys { get; init; } = Array.Empty<string>();
    /// <summary>
    ///     The number of milliseconds between usages of spells
    /// </summary>
    public int SpellIntervalMs { get; init; }
    /// <summary>
    ///     A collection of template keys of spells this monster will cast
    /// </summary>
    public ICollection<string> SpellTemplateKeys { get; init; } = Array.Empty<string>();
    /// <summary>
    ///     The sprite id of the monster minus the offset
    /// </summary>
    public ushort Sprite { get; init; }
    /// <summary>
    ///     The base stats of this monster
    /// </summary>
    [JsonRequired]
    public StatSheetSchema StatSheet { get; init; } = null!;
    /// <summary>
    ///     A unique id specific to this monster template<br />Best practice is to match the name of the file
    /// </summary>
    [JsonRequired]
    public string TemplateKey { get; init; } = null!;
    /// <summary>
    ///     The monster's type<br />WhiteSquare has no additional functionality, it just appears as a white square on the tab map
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public CreatureType Type { get; init; }
    /// <summary>
    ///     The number of milliseconds between movements while this monster is wandering when it has no target
    /// </summary>
    public int WanderIntervalMs { get; init; }
}