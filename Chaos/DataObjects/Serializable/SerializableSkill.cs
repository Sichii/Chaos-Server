namespace Chaos.DataObjects.Serializable;

public record SerializableSkill
{
    public int RemainingCooldownSecs { get; set; }
    public string TemplateKey { get; set; } = null!;
}