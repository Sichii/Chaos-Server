using Chaos.Geometry.Abstractions;

namespace Chaos.Client.Data.Models;

public sealed class ControlInfo
{
    public int? ButtonResultValue { get; set; }
    public List<int> ColorIndexes { get; set; } = new();
    public List<(string ImageName, int FrameIndex)> Images { get; set; } = new();
    public string Name { get; set; } = null!;
    public IRectangle Rect { get; set; } = null!;
    public ControlType Type { get; set; }
}

public enum ControlType
{
    Anchor = 0,
    Button = 3,
    ReadonlyTextBox = 7
}