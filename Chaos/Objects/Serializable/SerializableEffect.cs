namespace Chaos.Objects.Serializable;

public record SerializableEffect
{
    public string Name { get; set; } = null!;
    public int? RemainingSecs { get; set; }
}