using Chaos.Caches.Interfaces;
using Chaos.Factories.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

public class SpellFactory : ISpellFactory
{
    private readonly ILogger Logger;
    private readonly ISpellScriptFactory SpellScriptFactory;
    private readonly ISimpleCache<SpellTemplate> SpellTemplateCache;

    public SpellFactory(
        ISimpleCache<SpellTemplate> spellTemplateCache,
        ISpellScriptFactory spellScriptFactory,
        ILogger<SpellFactory> logger
    )
    {
        SpellTemplateCache = spellTemplateCache;
        SpellScriptFactory = spellScriptFactory;
        Logger = logger;
    }

    public Spell CreateSpell(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= Array.Empty<string>();
        var template = SpellTemplateCache.GetObject(templateKey);
        var spell = new Spell(template, SpellScriptFactory, extraScriptKeys);
        
        Logger.LogDebug("Created spell - Name: {SpellName}, UniqueId: {UniqueId}", spell.Template.Name, spell.UniqueId);

        return spell;
    }

    public Spell DeserializeSpell(SerializableSpell serializableSpell)
    {
        var spell = new Spell(serializableSpell, SpellTemplateCache, SpellScriptFactory);

        Logger.LogDebug("Deserialized spell - Name: {SpellName}, UniqueId: {UniqueId}", spell.Template.Name, spell.UniqueId);

        return spell;
    }

    public Spell CloneSpell(Spell spell)
    {
        var cloned = new Spell(spell.Template, SpellScriptFactory, spell.ScriptKeys)
        {
            Elapsed = spell.Elapsed
        };

        Logger.LogDebug(
            "Cloned spell - Name: {SpellName}, UniqueId: {UniqueId}, ClonedId: {ClonedId}",
            spell.Template.Name,
            spell.UniqueId,
            cloned.UniqueId);

        return cloned;
    }
}