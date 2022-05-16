namespace Chaos.Objects.Serializable;

public record SerializableBankItem
{
    public Attributes Attributes { get; set; } = null!;
    public DisplayColor Color { get; set; }
    public int? CurrentDurability { get; set; }
    public string DisplayName { get; set; } = null!;
    public int? MaximumDurabilityMod { get; set; }
    public int? RemainingCooldownSecs { get; set; }
    public string? ScriptKey { get; set; }
    public string TemplateKey { get; set; } = null!;
    public ulong UniqueId { get; set; }
}