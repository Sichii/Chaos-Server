using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.Aisling;

public sealed record LegendMarkSchema
{
    public long Added { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public MarkColor Color { get; init; }
    public int Count { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public MarkIcon Icon { get; init; }
    [JsonRequired]
    public string Key { get; init; } = null!;
    [JsonRequired]
    public string Text { get; init; } = null!;
}