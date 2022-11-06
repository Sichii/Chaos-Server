using System.Text.Json.Serialization;
using Chaos.Geometry;

namespace Chaos.Schemas.Data;

public sealed record WorldMapWarpSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Point Source { get; init; }
    [JsonRequired]
    public string WorldMapKey { get; init; } = null!;
}