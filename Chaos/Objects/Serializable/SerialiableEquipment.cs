using System.Collections.Generic;
using Chaos.Core.Definitions;

namespace Chaos.Objects.Serializable;

public record SerialiableEquipment
{
    public DisplayColor Color { get; set; }
    public int ElapsedMs { get; set; }
    public int CurrentDurability { get; set; }
    public ICollection<string>? ScriptKeys { get; set; }
    public EquipmentSlot Slot { get; set; }
    public string TemplateKey { get; set; } = null!;
    public ulong UniqueId { get; set; }
}