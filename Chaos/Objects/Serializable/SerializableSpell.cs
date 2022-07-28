using Chaos.Objects.Panel;

namespace Chaos.Objects.Serializable;

public record SerializableSpell
{
    public int ElapsedMs { get; init; }
    public ICollection<string> ScriptKeys { get; init; }
    public string TemplateKey { get; init; }
    public ulong UniqueId { get; init; }

    #pragma warning disable CS8618
    //json constructor
    public SerializableSpell() { }
    #pragma warning restore CS8618

    public SerializableSpell(Spell spell)
    {
        UniqueId = spell.UniqueId;
        ElapsedMs = Convert.ToInt32(spell.Elapsed.TotalMilliseconds);
        ScriptKeys = spell.ScriptKeys.Except(spell.Template.ScriptKeys).ToHashSet(StringComparer.OrdinalIgnoreCase);
        TemplateKey = spell.Template.TemplateKey;
    }
}