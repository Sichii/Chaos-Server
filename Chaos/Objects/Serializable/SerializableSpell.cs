using Chaos.Objects.Panel;

namespace Chaos.Objects.Serializable;

public record SerializableSpell
{
    public ulong UniqueId { get; }
    public int ElapsedMs { get; }
    public ICollection<string> ScriptKeys { get; set; }
    public string TemplateKey { get; set; }

    public SerializableSpell(Spell spell)
    {
        UniqueId = spell.UniqueId;
        ElapsedMs = Convert.ToInt32(spell.Elapsed.TotalMilliseconds);
        ScriptKeys = spell.ScriptKeys.Except(spell.Template.ScriptKeys).ToHashSet(StringComparer.OrdinalIgnoreCase);
        TemplateKey = spell.Template.TemplateKey;
    }
}