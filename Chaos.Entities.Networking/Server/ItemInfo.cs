using Chaos.Common.Definitions;

namespace Chaos.Entities.Networking.Server;

public record ItemInfo
{
    public required DisplayColor Color { get; init; }
    public required int? Cost { get; init; }
    public required uint? Count { get; init; }
    public required int CurrentDurability { get; init; }
    public required GameObjectType GameObjectType { get; init; }
    public required int MaxDurability { get; init; }
    public required string Name { get; init; } = null!;
    public required byte Slot { get; init; }
    public required ushort Sprite { get; init; }
    public required bool Stackable { get; init; }
}