namespace Chaos.Templates;

public sealed class DoorTemplate
{
    public bool Closed { get; init; }
    public bool OpenRight { get; init; }
    public Point Point { get; init; }
    public ushort Sprite { get; init; }
}