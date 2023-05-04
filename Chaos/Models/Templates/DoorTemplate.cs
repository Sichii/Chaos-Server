namespace Chaos.Models.Templates;

public sealed class DoorTemplate
{
    public required bool Closed { get; init; }
    public required bool OpenRight { get; init; }
    public required Point Point { get; init; }
    public required ushort Sprite { get; init; }
}