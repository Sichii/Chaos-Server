using System.Text.Json.Serialization;
using Chaos.Common.Definitions;
using Chaos.Schemas.Aisling.Abstractions;

namespace Chaos.Schemas.Aisling;

public sealed record ItemSchema : PanelObjectSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public DisplayColor Color { get; set; }
    [JsonRequired]
    public int Count { get; set; }
    public int? CurrentDurability { get; set; }
    public string? DisplayName { get; set; }
}