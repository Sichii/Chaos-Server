using Chaos.Objects.Panel;

namespace Chaos.Objects.Serializable;

public record SerializableItem
{
    public DisplayColor Color { get; }
    public int Count { get; }
    public int? CurrentDurability { get; }
    public int ElapsedMs { get; }
    public ICollection<string>? ScriptKeys { get; }
    public byte? Slot { get; }
    public string TemplateKey { get; }
    public ulong UniqueId { get; }

    public SerializableItem(Item item, bool serializeSlot = true)
    {
        UniqueId = item.UniqueId;
        ElapsedMs = Convert.ToInt32(item.Elapsed.TotalMilliseconds);
        ScriptKeys = item.ScriptKeys.Except(item.Template.ScriptKeys).ToHashSet(StringComparer.OrdinalIgnoreCase);
        TemplateKey = item.Template.TemplateKey;
        Color = item.Color;
        Count = item.Count;
        CurrentDurability = item.CurrentDurability;
        Slot = serializeSlot ? item.Slot : null;
    }
}