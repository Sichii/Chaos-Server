using Chaos.Core.Definitions;

namespace Chaos.DataObjects.Serializable;

public record SerializableItem
{
    public Attributes Attributes { get; set; } = new();
    public DisplayColor Color { get; set; }
    public int? CurrentDurability { get; set; }
    public string DisplayName { get; set; } = null!;
    public int? MaximumDurabilityMod { get; set; }
    public int? RemainingCooldownSecs { get; set; }
    public string? ScriptKey { get; set; }
    public byte Slot { get; set; }
    public string TemplateKey { get; set; } = null!;
    public ulong UniqueId { get; set; }
}