namespace Chaos.Objects.Serializable;

public record SerializableBankItem
{
    public DisplayColor Color { get; set; }
    public int? CurrentDurability { get; set; }
    public int RemainingMs { get; set; }
    public ICollection<string>? ScriptKeys { get; set; }
    public string TemplateKey { get; set; } = null!;
    public ulong UniqueId { get; set; }
}