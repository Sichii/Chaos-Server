using System.Text.Json.Serialization;
using Chaos.Geometry;

namespace Chaos.Schemas.Content;

public sealed record WorldMapNodeSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Location Destination { get; init; }
    [JsonRequired]
    public string NodeKey { get; init; } = null!;
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Point ScreenPosition { get; init; }
    [JsonRequired]
    public string Text { get; init; } = null!;
}