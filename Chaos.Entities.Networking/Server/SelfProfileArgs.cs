using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Server;

public record SelfProfileArgs : ISendArgs
{
    public AdvClass? AdvClass { get; set; }
    public BaseClass BaseClass { get; set; }
    public IDictionary<EquipmentSlot, ItemInfo> Equipment { get; set; } = new Dictionary<EquipmentSlot, ItemInfo>();
    public bool GroupOpen { get; set; }
    public string? GroupString { get; set; }
    public string? GuildName { get; set; }
    public string? GuildTitle { get; set; }
    public bool IsMaster { get; set; }
    public ICollection<LegendMarkInfo> LegendMarks { get; set; } = new List<LegendMarkInfo>();
    public string Name { get; set; } = null!;
    public Nation Nation { get; set; }
    public byte[] Portrait { get; set; } = Array.Empty<byte>();
    public string ProfileText { get; set; } = null!;
    public SocialStatus SocialStatus { get; set; }
    public string? SpouseName { get; set; }
    public ICollection<string> Titles { get; set; } = new List<string>();
}