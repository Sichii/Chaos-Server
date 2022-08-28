using Chaos.Common.Definitions;

namespace Chaos.Entities.Schemas.World;

public record ItemSchema : PanelObjectSchema
{
    public DisplayColor Color { get; init; }
    public int Count { get; init; }
    public int? CurrentDurability { get; init; }
}