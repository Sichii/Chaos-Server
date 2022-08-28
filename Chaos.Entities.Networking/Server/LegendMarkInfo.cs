using Chaos.Common.Definitions;

namespace Chaos.Entities.Networking.Server;

public record LegendMarkInfo
{
    public MarkColor Color { get; set; }
    public MarkIcon Icon { get; set; }
    public string Key { get; set; } = null!;
    public string Text { get; set; } = null!;
}