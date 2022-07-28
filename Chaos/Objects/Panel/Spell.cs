using Chaos.Networking.Model.Server;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Objects.Serializable;
using Chaos.Scripts.Interfaces;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
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

        if (extraScriptKeys != null)
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

    public SpellInfo ToSpellInfo() => new()
    {
        CastLines = CastLines,
        Name = Template.Name,
        Prompt = Template.Prompt ?? string.Empty,
        Slot = Slot,
        SpellType = Template.SpellType,
        Sprite = Template.PanelSprite
    };
}