using Chaos.Common.Definitions;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Geometry.Definitions;

namespace Chaos.Entities.Schemas.Templates;

public record MonsterTemplateSchema
{
    public required StatSheetSchema StatSheet { get; init; }
    public required CreatureType Type { get; init; }
    public required Direction Direction { get; init; }
    public required string Name { get; init; }
    public required ushort Sprite { get; init; }
    public required int WanderIntervalMs { get; init; }
    public required int MoveIntervalMs { get; init; }
    public required int AttackIntervalMs { get; init; }
    public required int CastIntervalMs { get; init; }
    public required ICollection<string> SpellTemplateKeys { get; init; } = Array.Empty<string>();
    public required ICollection<string> SkillTemplateKeys { get; init; } = Array.Empty<string>();
    public required ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    public required string TemplateKey { get; init; }
}