using Chaos.Common.Definitions;

namespace Chaos.Entities.Networking.Server;

public record ItemInfo
{
    public BaseClass Class { get; init; }
    public DisplayColor Color { get; init; }
    public int? Cost { get; init; }
    public uint? Count { get; init; }
    public int CurrentDurability { get; init; }
    public GameObjectType GameObjectType { get; init; }
    public int MaxDurability { get; init; }
    public string Name { get; init; } = null!;
    public byte Slot { get; init; }
    public ushort Sprite { get; init; }
    public bool Stackable { get; init; }
}