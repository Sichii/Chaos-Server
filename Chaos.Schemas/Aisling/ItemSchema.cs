using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.Aisling;

public sealed record ItemSchema : PanelObjectSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public DisplayColor Color { get; init; }
    [JsonRequired]
    public int Count { get; init; }
    public int? CurrentDurability { get; init; }
}