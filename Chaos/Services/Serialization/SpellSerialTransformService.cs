using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Serialization.Interfaces;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Serialization;

public class SpellSerialTransformService : ISerialTransformService<Spell, SerializableSpell>
{
    private readonly ILogger<SpellSerialTransformService> Logger;
    private readonly ISpellScriptFactory SpellScriptFactory;
    private readonly ISimpleCache<SpellTemplate> SpellTemplateCache;

    public SpellSerialTransformService(
        ISimpleCache<SpellTemplate> spellTemplateCache,
        ISpellScriptFactory spellScriptFactory,
        ILogger<SpellSerialTransformService> logger
    )
    {
        SpellTemplateCache = spellTemplateCache;
        SpellScriptFactory = spellScriptFactory;
        Logger = logger;
    }

    public Spell Transform(SerializableSpell serialized)
    {
        var spell = new Spell(serialized, SpellTemplateCache, SpellScriptFactory);

        Logger.LogTrace("Deserialized spell - Name: {SpellName}, UniqueId: {UniqueId}", spell.Template.Name, spell.UniqueId);

        return spell;
    }

    public SerializableSpell Transform(Spell entity)
    {
        var ret = new SerializableSpell(entity);

        Logger.LogTrace("Serialized spell - TemplateKey: {TemplateKey}, UniqueId: {UniqueId}", ret.TemplateKey, ret.UniqueId);

        return ret;
    }
}