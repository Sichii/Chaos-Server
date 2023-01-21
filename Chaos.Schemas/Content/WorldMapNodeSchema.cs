using System.Text.Json.Serialization;
using Chaos.Geometry;

namespace Chaos.Schemas.Content;

public sealed record WorldMapNodeSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Location Destination { get; set; }
    [JsonRequired]
    public string NodeKey { get; set; } = null!;
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Point ScreenPosition { get; set; }
    [JsonRequired]
    public string Text { get; set; } = null!;
}