namespace Chaos.Networking.Model.Server;

public record ItemInfo
{
    public BaseClass Class { get; set; }
    public DisplayColor Color { get; set; }
    public int? Cost { get; set; }
    public uint? Count { get; set; }
    public int CurrentDurability { get; set; }
    public GameObjectType GameObjectType { get; set; }
    public int MaxDurability { get; set; }
    public string Name { get; set; } = null!;
    public byte Slot { get; set; }
    public ushort Sprite { get; set; }
    public bool Stackable { get; set; }
}