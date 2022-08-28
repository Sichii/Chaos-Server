using Chaos.Entities.Schemas.World;
using Chaos.Objects.Panel;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public class SpellFactory : ISpellFactory
{
    private readonly ILogger Logger;
    private readonly ISimpleCache SimpleCache;
    private readonly ISpellScriptFactory SpellScriptFactory;

    public SpellFactory(
        ISimpleCache simpleCache,
        ISpellScriptFactory spellScriptFactory,
        ILogger<SpellFactory> logger
    )
    {
        SimpleCache = simpleCache;
        SpellScriptFactory = spellScriptFactory;
        Logger = logger;
    }

    public Spell Clone(Spell obj)
    {
        var cloned = new Spell(obj.Template, SpellScriptFactory, obj.ScriptKeys)
        {
            Elapsed = obj.Elapsed
        };

        Logger.LogDebug(
            "Cloned spell - Name: {SpellName}, UniqueId: {UniqueId}, ClonedId: {ClonedId}",
            obj.Template.Name,
            obj.UniqueId,
            cloned.UniqueId);

        return cloned;
    }

    public Spell Create(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= Array.Empty<string>();
        var template = SimpleCache.GetObject<SpellTemplate>(templateKey);
        var spell = new Spell(template, SpellScriptFactory, extraScriptKeys);

        Logger.LogDebug("Created spell - Name: {SpellName}, UniqueId: {UniqueId}", spell.Template.Name, spell.UniqueId);

        return spell;
    }

    public Spell Deserialize(SpellSchema schema)
    {
        var spell = new Spell(schema, SimpleCache, SpellScriptFactory);

        Logger.LogDebug("Deserialized spell - Name: {SpellName}, UniqueId: {UniqueId}", spell.Template.Name, spell.UniqueId);

        return spell;
    }
}