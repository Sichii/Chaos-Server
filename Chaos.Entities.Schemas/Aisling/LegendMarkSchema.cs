using Chaos.Common.Definitions;

namespace Chaos.Entities.Schemas.Aisling;

public record LegendMarkSchema
{
    public required long Added { get; init; }
    public required MarkColor Color { get; init; }
    public required int Count { get; init; }
    public required MarkIcon Icon { get; init; }
    public required string Key { get; init; } 
    public required string Text { get; init; }
}