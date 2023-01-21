using System.Text.Json.Serialization;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Schemas.Content;

public sealed record MerchantSpawnSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Direction Direction { get; set; }
    public ICollection<string> ExtraScriptKeys { get; set; } = Array.Empty<string>();
    [JsonRequired]
    public string MerchantTemplateKey { get; set; } = null!;
    public Point SpawnPoint { get; set; }
}