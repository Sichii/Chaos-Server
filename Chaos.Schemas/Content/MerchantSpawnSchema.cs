using System.Text.Json.Serialization;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Schemas.Content;

public sealed record MerchantSpawnSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Direction Direction { get; init; }
    public ICollection<string> ExtraScriptKeys { get; init; } = Array.Empty<string>();
    [JsonRequired]
    public string MerchantTemplateKey { get; init; } = null!;
    public Point SpawnPoint { get; init; }
}