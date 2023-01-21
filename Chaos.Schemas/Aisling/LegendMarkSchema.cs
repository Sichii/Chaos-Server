using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.Aisling;

public sealed record LegendMarkSchema
{
    public long Added { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public MarkColor Color { get; set; }
    public int Count { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public MarkIcon Icon { get; set; }
    [JsonRequired]
    public string Key { get; set; } = null!;
    [JsonRequired]
    public string Text { get; set; } = null!;
}