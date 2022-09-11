using Chaos.Common.Definitions;
using Chaos.Entities.Schemas.Templates;
using Chaos.Geometry;

namespace Chaos.Entities.Schemas.Content;

public record MonsterSpawnSchema
{
    public required string? LootTableKey { get; init; }
    public required int IntervalSecs { get; init; }
    public required int? IntervalVariancePct { get; init; }
    public required int MaxPerSpawn { get; init; }
    public required int MaxAmount { get; init; }
    public required int? AggroRange { get; init; }
    public required int MinGoldDrop { get; init; }
    public required int MaxGoldDrop { get; init; }
    public required int ExpReward { get; init; }
    public required ICollection<string> ExtraScriptKeys { get; init; } = Array.Empty<string>();
    public required string MonsterTemplateKey { get; init; }
    public required Rectangle? SpawnArea { get; init; }
}