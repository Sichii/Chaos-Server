using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Geometry.Definitions;
using Chaos.Scripts.Abstractions;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public record MonsterTemplate : ITemplate, IScripted
{
    public required StatSheet StatSheet { get; init; }
    public required string Name { get; init; }
    public required string TemplateKey { get; init; }
    public required CreatureType Type { get; init; }
    public required Direction Direction { get; init; }
    public required ushort Sprite { get; init; }
    public required int WanderingIntervalMs { get; init; }
    public required int MoveIntervalMs { get; init; }
    public required int AttackIntervalMs { get; init; }
    public required int CastIntervalMs { get; init; }
    public required ISet<string> SpellTemplateKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public required ISet<string> SkillTemplateKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public required ISet<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
}