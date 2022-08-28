using Chaos.Common.Definitions;

namespace Chaos.Entities.Schemas.World;

public record LegendMarkSchema
{
    public long Added { get; init; }
    public MarkColor Color { get; init; }
    public int Count { get; init; }
    public MarkIcon Icon { get; init; }
    public string Key { get; init; } = null!;
    public string Text { get; init; } = null!;
}