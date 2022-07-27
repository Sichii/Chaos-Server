using Chaos.Objects.Panel;

namespace Chaos.Objects.Serializable;

public record SerializableSkill
{
    public int ElapsedMs { get; init; }
    public ICollection<string> ScriptKeys { get; init; }
    public string TemplateKey { get; init; }
    public ulong UniqueId { get; init; }

    #pragma warning disable CS8618
    //json constructor
    public SerializableSkill() { }
    #pragma warning restore CS8618
    
    public SerializableSkill(Skill skill)
    {
        UniqueId = skill.UniqueId;
        ElapsedMs = Convert.ToInt32(skill.Elapsed.TotalMilliseconds);
        ScriptKeys = skill.ScriptKeys.Except(skill.Template.ScriptKeys).ToHashSet(StringComparer.OrdinalIgnoreCase);
        TemplateKey = skill.Template.TemplateKey;
    }
}