using Chaos.Common.Definitions;

namespace Chaos.Schemas.Aisling;

public sealed record ItemSchema : PanelObjectSchema
{
    public required DisplayColor Color { get; init; }
    public required int Count { get; init; }
    public required int? CurrentDurability { get; init; }
}