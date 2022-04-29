using Chaos.Core.Definitions;

namespace Chaos.Networking.Model.Server;

public record LegendMarkArg
{
    public MarkColor Color { get; set; }
    public MarkIcon Icon { get; set; }
    public string Key { get; set; } = null!;
    public string Text { get; set; } = null!;
}