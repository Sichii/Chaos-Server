using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record AttributesArgs : ISendArgs
{
    public byte Ability { get; set; }
    public sbyte Ac { get; set; }
    public bool Blind { get; set; }
    public byte Con { get; set; }
    public uint CurrentHp { get; set; }
    public uint CurrentMp { get; set; }
    public short CurrentWeight { get; set; }
    public Element DefenseElement { get; set; }
    public byte Dex { get; set; }
    public byte Dmg { get; set; }
    public uint GamePoints { get; set; }
    public uint Gold { get; set; }
    public byte Hit { get; set; }
    public byte Int { get; set; }
    public bool IsAdmin { get; set; }
    public byte Level { get; set; }
    public byte MagicResistance { get; set; }
    public MailFlag MailFlags { get; set; }
    public uint MaximumHp { get; set; }
    public uint MaximumMp { get; set; }
    public short MaxWeight { get; set; }
    public Element OffenseElement { get; set; }
    public StatUpdateType StatUpdateType { get; set; }
    public byte Str { get; set; }
    public uint ToNextAbility { get; set; }
    public uint ToNextLevel { get; set; }
    public uint TotalAbility { get; set; }
    public uint TotalExp { get; set; }
    public byte UnspentPoints { get; set; }
    public byte Wis { get; set; }
    public bool HasUnspentPoints => UnspentPoints != 0;
}