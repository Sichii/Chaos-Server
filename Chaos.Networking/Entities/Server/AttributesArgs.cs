using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.Attributes" /> packet
/// </summary>
public sealed record AttributesArgs : IPacketSerializable
{
    /// <summary>
    ///     The ability level of the character
    /// </summary>
    public byte Ability { get; set; }

    /// <summary>
    ///     The ac of the character
    /// </summary>
    public sbyte Ac { get; set; }

    /// <summary>
    ///     Whether or not the character is blind
    /// </summary>
    public bool Blind { get; set; }

    /// <summary>
    ///     The constitution stat of the character
    /// </summary>
    public byte Con { get; set; }

    /// <summary>
    ///     The current hp of the character
    /// </summary>
    public uint CurrentHp { get; set; }

    /// <summary>
    ///     The current mp of the character
    /// </summary>
    public uint CurrentMp { get; set; }

    /// <summary>
    ///     The current weight of the character
    /// </summary>
    public short CurrentWeight { get; set; }

    /// <summary>
    ///     The defense element of the character
    /// </summary>
    public Element DefenseElement { get; set; }

    /// <summary>
    ///     The dexterity stat of the character
    /// </summary>
    public byte Dex { get; set; }

    /// <summary>
    ///     The damage stat of the character
    /// </summary>
    public byte Dmg { get; set; }

    /// <summary>
    ///     The amount of game points the character has
    /// </summary>
    public uint GamePoints { get; set; }

    /// <summary>
    ///     The amount of gold the character has
    /// </summary>
    public uint Gold { get; set; }

    /// <summary>
    ///     Whether or not the mail icon should blink
    /// </summary>
    public bool HasUnreadMail { get; set; }

    /// <summary>
    ///     The hit stat of the character
    /// </summary>
    public byte Hit { get; set; }

    /// <summary>
    ///     The intelligence stat of the character
    /// </summary>
    public byte Int { get; set; }

    /// <summary>
    ///     Whether or not the character is an admin
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    ///     Whether or not the character is allowed to swim on water tiles
    /// </summary>
    public bool IsSwimming { get; set; }

    /// <summary>
    ///     The level of the character
    /// </summary>
    public byte Level { get; set; }

    /// <summary>
    ///     The magic resistance of the character
    /// </summary>
    public byte MagicResistance { get; set; }

    /// <summary>
    ///     The maximum hp of the character
    /// </summary>
    public uint MaximumHp { get; set; }

    /// <summary>
    ///     The maximum mp of the character
    /// </summary>
    public uint MaximumMp { get; set; }

    /// <summary>
    ///     The maximum weight of the character
    /// </summary>
    public short MaxWeight { get; set; }

    /// <summary>
    ///     The offense element of the character
    /// </summary>
    public Element OffenseElement { get; set; }

    /// <summary>
    ///     The type of stat update to send to the client
    /// </summary>
    public StatUpdateType StatUpdateType { get; set; }

    /// <summary>
    ///     The strength stat of the character
    /// </summary>
    public byte Str { get; set; }

    /// <summary>
    ///     The amount of ability points needed for the character to increase their ability level
    /// </summary>
    public uint ToNextAbility { get; set; }

    /// <summary>
    ///     The amount of experience points needed for the character to increase their level
    /// </summary>
    public uint ToNextLevel { get; set; }

    /// <summary>
    ///     The total amount of ability points the character has
    /// </summary>
    public uint TotalAbility { get; set; }

    /// <summary>
    ///     The total amount of experience points the character has
    /// </summary>
    public uint TotalExp { get; set; }

    /// <summary>
    ///     The amount of unspent stat points the character has
    /// </summary>
    public byte UnspentPoints { get; set; }

    /// <summary>
    ///     The wisdom stat of the character
    /// </summary>
    public byte Wis { get; set; }

    /// <summary>
    ///     Whether or not the character has unspent stat points
    /// </summary>
    public bool HasUnspentPoints => UnspentPoints != 0;
}