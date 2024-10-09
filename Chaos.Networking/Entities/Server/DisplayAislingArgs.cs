using Chaos.DarkAges.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.DisplayAisling" /> packet
/// </summary>
public sealed record DisplayAislingArgs : IPacketSerializable
{
    /// <summary>
    ///     The color of the accessory in the first accessory slot
    /// </summary>
    public DisplayColor AccessoryColor1 { get; set; }

    /// <summary>
    ///     The color of the accessory in the second accessory slot
    /// </summary>
    public DisplayColor AccessoryColor2 { get; set; }

    /// <summary>
    ///     The color of the accessory in the third accessory slot
    /// </summary>
    public DisplayColor AccessoryColor3 { get; set; }

    /// <summary>
    ///     The sprite of the accessory in the first accessory slot
    /// </summary>
    public ushort AccessorySprite1 { get; set; }

    /// <summary>
    ///     The sprite of the accessory in the second accessory slot
    /// </summary>
    public ushort AccessorySprite2 { get; set; }

    /// <summary>
    ///     The sprite of the accessory in the third accessory slot
    /// </summary>
    public ushort AccessorySprite3 { get; set; }

    /// <summary>
    ///     The sprite of the armor in the armor slot
    /// </summary>
    public ushort ArmorSprite1 { get; set; }

    /// <summary>
    ///     Also.. the sprite of the armor in the armor slot (not sure what the difference is atm)
    /// </summary>
    public ushort ArmorSprite2 { get; set; }

    /// <summary>
    ///     The color of the aisling's body
    /// </summary>
    public BodyColor BodyColor { get; set; }

    /// <summary>
    ///     The sprite used for the aisling's body
    /// </summary>
    public BodySprite BodySprite { get; set; }

    /// <summary>
    ///     The color of the boots in the boots slot
    /// </summary>
    public DisplayColor BootsColor { get; set; }

    /// <summary>
    ///     The sprite of the boots in the boots slot
    /// </summary>
    public byte BootsSprite { get; set; }

    /// <summary>
    ///     The direction the aisling is facing
    /// </summary>
    public Direction Direction { get; set; }

    /// <summary>
    ///     The sprite used for the aisling's face
    /// </summary>
    public byte FaceSprite { get; set; }

    /// <summary>
    ///     The aisling's gender
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    ///     If the aisling has a group box, this is the text displayed in it
    /// </summary>
    public string? GroupBoxText { get; set; }

    /// <summary>
    ///     The color of the aisling's helmet in the overhelm slot, or the color of the helmet in the helmet slot, or the color
    ///     of the aisling's hair
    /// </summary>
    public DisplayColor HeadColor { get; set; }

    /// <summary>
    ///     The sprite of the aisling's helmet in the overhelm slot, or the sprite of the helmet in the helmet slot, or the
    ///     sprite of the aisling's hair
    /// </summary>
    public ushort HeadSprite { get; set; }

    /// <summary>
    ///     The id of the aisling
    /// </summary>
    public uint Id { get; set; }

    /// <summary>
    ///     Whether or not the aisling is dead. If true, the aisling will show up as a ghost
    /// </summary>
    public bool IsDead { get; set; }

    /// <summary>
    ///     Whether or not the aisling is hidden. If true, the aisling will be fully invisible
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    ///     Whether or not the aisling is hidden. If true, the aisling will be semitransparent
    /// </summary>
    public bool IsTransparent { get; set; }

    /// <summary>
    ///     If the aisling is on a dark map and has a light source, this is the size of the visible area around the aisling
    /// </summary>
    public LanternSize LanternSize { get; set; }

    /// <summary>
    ///     The name of the aisling
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     The style of the aisling's nametag
    /// </summary>
    public NameTagStyle NameTagStyle { get; set; }

    /// <summary>
    ///     The color of the aisling's armor in the overcoat slot
    /// </summary>
    public DisplayColor OvercoatColor { get; set; }

    /// <summary>
    ///     The sprite of the aisling's armor in the overcoat slot
    /// </summary>
    public ushort OvercoatSprite { get; set; }

    /// <summary>
    ///     The color of the aisling's pants, if they have any
    /// </summary>
    public DisplayColor? PantsColor { get; set; }

    /// <summary>
    ///     If the aisling is sitting in a resting position, this is the identifier for that position
    /// </summary>
    public RestPosition RestPosition { get; set; }

    /// <summary>
    ///     The color of the aisling's shield in the shield slot
    /// </summary>
    public byte ShieldSprite { get; set; }

    /// <summary>
    ///     If the aisling is in a form that has a sprite that should override the aisling's equipment, this is that sprite id
    /// </summary>
    public ushort? Sprite { get; set; }

    /// <summary>
    ///     The color of the aisling's weapon in the weapon slot
    /// </summary>
    public ushort WeaponSprite { get; set; }

    /// <summary>
    ///     The x coordinate of the aisling
    /// </summary>
    public int X { get; set; }

    /// <summary>
    ///     The y coordinate of the aisling
    /// </summary>
    public int Y { get; set; }
}