namespace Chaos.DataObjects.Serializable;

public record SerializableSpell
{
    public int RemainingCooldownSecs { get; set; }
    public string TemplateKey { get; set; } = null!;
}