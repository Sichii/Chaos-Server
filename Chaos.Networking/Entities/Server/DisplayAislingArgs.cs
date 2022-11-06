using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record DisplayAislingArgs : ISendArgs
{
    public DisplayColor AccessoryColor1 { get; set; }
    public DisplayColor AccessoryColor2 { get; set; }
    public DisplayColor AccessoryColor3 { get; set; }
    public ushort AccessorySprite1 { get; set; }
    public ushort AccessorySprite2 { get; set; }
    public ushort AccessorySprite3 { get; set; }
    public ushort ArmorSprite1 { get; set; }
    public ushort ArmorSprite2 { get; set; }
    public BodyColor BodyColor { get; set; }
    public BodySprite BodySprite { get; set; }
    public DisplayColor BootsColor { get; set; }
    public byte BootsSprite { get; set; }
    public CreatureType CreatureType { get; set; }
    public Direction Direction { get; set; }
    public EntityType EntityType { get; set; }
    public byte FaceSprite { get; set; }
    public Gender Gender { get; set; }
    public string? GroupBoxText { get; set; }
    public DisplayColor HeadColor { get; set; }
    public ushort HeadSprite { get; set; }
    public uint Id { get; set; }
    public bool IsDead { get; set; }
    public bool IsHidden { get; set; }
    public bool IsMaster { get; set; }
    public LanternSize LanternSize { get; set; }
    public string Name { get; set; } = null!;
    public NameTagStyle NameTagStyle { get; set; }
    public DisplayColor OvercoatColor { get; set; }
    public ushort OvercoatSprite { get; set; }
    public RestPosition RestPosition { get; set; }
    public byte ShieldSprite { get; set; }
    public ushort? Sprite { get; set; }
    public ushort WeaponSprite { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}