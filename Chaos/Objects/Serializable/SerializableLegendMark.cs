namespace Chaos.Objects.Serializable;

public record SerializableLegendMark
{
    public long Added { get; set; }
    public MarkColor Color { get; set; }
    public int Count { get; set; }
    public MarkIcon Icon { get; set; }
    public string Key { get; set; } = null!;
    public string Text { get; set; } = null!;
}