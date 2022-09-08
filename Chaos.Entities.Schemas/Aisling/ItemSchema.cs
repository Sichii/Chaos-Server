using Chaos.Common.Definitions;

namespace Chaos.Entities.Schemas.Aisling;

public record ItemSchema : PanelObjectSchema
{
    public required DisplayColor Color { get; init; }
    public required int Count { get; init; }
    public required int? CurrentDurability { get; init; }
}