using System.Text.Json.Serialization;
using Chaos.Geometry;

namespace Chaos.Schemas.Data;

public sealed record WorldMapWarpSchema
{
    [JsonRequired]
    public string WorldMapKey { get; init; } = null!;
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Point Source { get; init; }
}