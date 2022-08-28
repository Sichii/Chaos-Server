using Chaos.Entities.Networking.Server;
using Chaos.Entities.Schemas.World;
using Chaos.Objects.Panel.Abstractions;
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
        ulong? uniqueId = null,
        int? elapsedMs = null
    )
        : base(template, uniqueId, elapsedMs)
    {
        Template = template;
        CastLines = template.CastLines;

        if (extraScriptKeys != null)
            ScriptKeys.AddRange(extraScriptKeys);

        Script = spellScriptFactory.CreateScript(ScriptKeys, this);
    }

    public Spell(
        SpellSchema schema,
        ISimpleCache simpleCache,
        ISpellScriptFactory skillScriptFactory
    )
        : this(
            simpleCache.GetObject<SpellTemplate>(schema.TemplateKey),
            skillScriptFactory,
            schema.ScriptKeys,
            schema.UniqueId,
            schema.ElapsedMs) { }
}