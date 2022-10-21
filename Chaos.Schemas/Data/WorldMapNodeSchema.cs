using System.Text.Json.Serialization;
using Chaos.Geometry;

namespace Chaos.Schemas.Data;

public sealed record WorldMapNodeSchema
{
    [JsonRequired]
    public string NodeKey { get; init; } = null!;
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Location Destination { get; init; }
    [JsonRequired]
    public string Text { get; init; } = null!;
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Point ScreenPosition { get; init; }
}