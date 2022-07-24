using Chaos.Objects.Panel;

namespace Chaos.Objects.Serializable;

public record SerializableSkill
{
    public ulong UniqueId { get; }
    public int ElapsedMs { get; }
    public ICollection<string> ScriptKeys { get; } 
    public string TemplateKey { get; }

    public SerializableSkill(Skill skill)
    {
        UniqueId = skill.UniqueId;
        ElapsedMs = Convert.ToInt32(skill.Elapsed.TotalMilliseconds);
        ScriptKeys = skill.ScriptKeys.Except(skill.Template.ScriptKeys).ToHashSet(StringComparer.OrdinalIgnoreCase);
        TemplateKey = skill.Template.TemplateKey;
    }
}