using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Geometry.Definitions;
using Chaos.Scripts.Abstractions;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public record MonsterTemplate : ITemplate, IScripted
{
    /// <summary>
    /// The number of milliseconds between usages of assails
    /// </summary>
    public required int AssailIntervalMs { get; init; }
    /// <summary>
    /// The initial direction of the monster when spawned
    /// </summary>
    public required Direction Direction { get; init; }
    /// <summary>
    /// The number of milliseconds between movements while this monster is targeting an enemy
    /// </summary>
    public required int MoveIntervalMs { get; init; }
    /// <summary>
    /// The name of the monster when double clicked
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// A collection of names of monsters scripts to attach to this monster<br/>TODO: scripts section 
    /// </summary>
    public required ISet<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    /// <summary>
    /// The number of milliseconds between usages of non-assail skills
    /// </summary>
    public required int SkillIntervalMs { get; init; }
    /// <summary>
    /// A collection of template keys of skills this monster will use
    /// </summary>
    public required ISet<string> SkillTemplateKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    /// <summary>
    /// The number of milliseconds between usages of spells
    /// </summary>
    public required int SpellIntervalMs { get; init; }
    /// <summary>
    /// A collection of template keys of spells this monster will cast
    /// </summary>
    public required ISet<string> SpellTemplateKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    /// <summary>
    /// The sprite id of the monster minus the offset
    /// </summary>
    public required ushort Sprite { get; init; }
    /// <summary>
    /// The base stats of this monster
    /// </summary>
    public required StatSheet StatSheet { get; init; }
    /// <summary>
    /// A unique id specific to this monster template<br/>Best practice is to match the name of the file
    /// </summary>
    public required string TemplateKey { get; init; }
    /// <summary>
    /// The monster's type<br/>WhiteSquare has no additional functionality, it just appears as a white square on the tab map
    /// </summary>
    public required CreatureType Type { get; init; }
    /// <summary>
    /// The number of milliseconds between movements while this monster is wandering when it has no target
    /// </summary>
    public required int WanderIntervalMs { get; init; }
}