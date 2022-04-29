namespace Chaos.DataObjects.Serializable;

public record SerializableEffect
{
    public string EffectKey { get; set; } = null!;
    public int RemainingSecs { get; set; }
}