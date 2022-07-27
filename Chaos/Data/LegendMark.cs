using Chaos.Networking.Model.Server;
using Chaos.Objects.Serializable;

namespace Chaos.Data;

public record LegendMark(
    string Text,
    string Key,
    MarkIcon Icon,
    MarkColor Color,
    int Count,
    GameTime Added
)
{
    public GameTime Added { get; set; } = Added;
    public int Count { get; set; } = Count;

    public LegendMark(SerializableLegendMark serializableLegendMark)
        : this(
            serializableLegendMark.Text,
            serializableLegendMark.Key,
            serializableLegendMark.Icon,
            serializableLegendMark.Color,
            serializableLegendMark.Count,
            new GameTime(serializableLegendMark.Added)) { }

    public virtual bool Equals(LegendMark? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return string.Equals(Text, other.Text, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Text);

    public LegendMarkInfo ToLegendMarkInfo() => new()
    {
        Color = Color,
        Icon = Icon,
        Key = Key,
        Text = ToString()
    };
    
    public override string ToString() => Count > 1 ? $@"{Text} ({Count}) - {Added.ToString()}" : $@"{Text} - {Added.ToString()}";
}