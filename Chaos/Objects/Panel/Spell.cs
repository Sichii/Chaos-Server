using Chaos.Caches.Interfaces;
using Chaos.Factories.Interfaces;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Objects.Serializable;
using Chaos.Scripts.Interfaces;
using Chaos.Templates;

namespace Chaos.Objects.Panel;

/// <summary>
///     Represents an object that exists within the spell panel.
/// </summary>
public class Spell : PanelObjectBase, IScriptedSpell
{
    public byte CastLines { get; set; }
    public ISpellScript Script { get; }
    public override SpellTemplate Template { get; }

    public Spell(
        SpellTemplate template,
        ISpellScriptFactory spellScriptFactory,
        ICollection<string>? extraScriptKeys = null,
        ulong? uniqueId = null
    )
        : base(template, uniqueId)
    {
        Template = template;
        CastLines = template.CastLines;
        
        if(extraScriptKeys != null)
            ScriptKeys.AddRange(extraScriptKeys);

        Script = spellScriptFactory.CreateScript(ScriptKeys, this);
    }
    
    public Spell(
        SerializableSpell serializableSpell,
        ISimpleCache<SpellTemplate> skillTemplateCache,
        ISpellScriptFactory skillScriptFactory
    )
        : this(
            skillTemplateCache.GetObject(serializableSpell.TemplateKey),
            skillScriptFactory,
            serializableSpell.ScriptKeys,
            serializableSpell.UniqueId) =>
        Elapsed = TimeSpan.FromMilliseconds(serializableSpell.ElapsedMs);
}