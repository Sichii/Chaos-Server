using Chaos.Data;

namespace Chaos.Objects.Serializable;

public record SerializableLegendMark
{
    public long Added { get; init; }
    public MarkColor Color { get; init; }
    public int Count { get; init; }
    public MarkIcon Icon { get; init; }
    public string Key { get; init; }
    public string Text { get; init; }

    #pragma warning disable CS8618
    //json constructor
    public SerializableLegendMark() { }
    #pragma warning restore CS8618
    
    public SerializableLegendMark(LegendMark legendMark)
    {
        Added = legendMark.Added.Ticks;
        Color = legendMark.Color;
        Count = legendMark.Count;
        Icon = legendMark.Icon;
        Key = legendMark.Key;
        Text = legendMark.Text;
    }
}