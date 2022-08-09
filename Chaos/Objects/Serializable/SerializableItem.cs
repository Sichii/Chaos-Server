using Chaos.Networking.Definitions;
using Chaos.Objects.Panel;

namespace Chaos.Objects.Serializable;

public record SerializableItem
{
    public DisplayColor Color { get; init; }
    public int Count { get; init; }
    public int? CurrentDurability { get; init; }
    public int ElapsedMs { get; init; }
    public ICollection<string>? ScriptKeys { get; init; }
    public byte? Slot { get; init; }
    public string TemplateKey { get; init; }
    public ulong UniqueId { get; init; }

    #pragma warning disable CS8618
    //json constructor
    public SerializableItem() { }
    #pragma warning restore CS8618

    public SerializableItem(Item item, bool shouldSerializeSlot = true)
    {
        UniqueId = item.UniqueId;
        ElapsedMs = Convert.ToInt32(item.Elapsed.TotalMilliseconds);
        ScriptKeys = item.ScriptKeys.Except(item.Template.ScriptKeys).ToHashSet(StringComparer.OrdinalIgnoreCase);
        TemplateKey = item.Template.TemplateKey;
        Color = item.Color;
        Count = item.Count;
        CurrentDurability = item.CurrentDurability;
        Slot = shouldSerializeSlot ? item.Slot : null;
    }
}