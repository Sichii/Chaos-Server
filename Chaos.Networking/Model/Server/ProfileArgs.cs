using Chaos.Core.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record ProfileArgs : ISendArgs
{
    public AdvClass? AdvClass { get; set; }
    public BaseClass BaseClass { get; set; }
    public IDictionary<EquipmentSlot, ItemArg?> Equipment { get; set; } = new Dictionary<EquipmentSlot, ItemArg?>();
    public bool GroupOpen { get; set; }
    public string? GuildName { get; set; }
    public string? GuildTitle { get; set; }
    public uint Id { get; set; }
    public ICollection<LegendMarkArg> LegendMarks { get; set; } = new List<LegendMarkArg>();
    public string Name { get; set; } = null!;
    public Nation Nation { get; set; }
    public byte[] Portrait { get; set; } = Array.Empty<byte>();
    public string? ProfileText { get; set; }
    public SocialStatus SocialStatus { get; set; }
    public ICollection<string> Titles { get; set; } = new List<string>();
}