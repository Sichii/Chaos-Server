using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record ProfileArgs : ISendArgs
{
    public AdvClass? AdvClass { get; set; }
    public BaseClass BaseClass { get; set; }
    public Dictionary<EquipmentSlot, ItemInfo?> Equipment { get; set; } = new();
    public bool GroupOpen { get; set; }
    public string? GuildName { get; set; }
    public string? GuildTitle { get; set; }
    public uint Id { get; set; }
    public ICollection<LegendMarkInfo> LegendMarks { get; set; } = new List<LegendMarkInfo>();
    public string Name { get; set; } = null!;
    public Nation Nation { get; set; }
    public byte[] Portrait { get; set; } = Array.Empty<byte>();
    public string? ProfileText { get; set; }
    public SocialStatus SocialStatus { get; set; }
    public ICollection<string> Titles { get; set; } = new List<string>();
}