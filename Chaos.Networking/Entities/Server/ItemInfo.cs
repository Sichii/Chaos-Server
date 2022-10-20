using Chaos.Common.Definitions;

namespace Chaos.Networking.Entities.Server;

public sealed record ItemInfo
{
    public required DisplayColor Color { get; init; }
    public required int? Cost { get; init; }
    public required uint? Count { get; init; }
    public required int CurrentDurability { get; init; }
    public required EntityType EntityType { get; init; }
    public required int MaxDurability { get; init; }
    public required string Name { get; init; } = null!;
    public required byte Slot { get; init; }
    public required ushort Sprite { get; init; }
    public required bool Stackable { get; init; }
}