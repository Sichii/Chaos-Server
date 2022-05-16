using Chaos.Caches.Interfaces;
using Chaos.Core.Identity;
using Chaos.Core.Utilities;
using Chaos.Factories.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

public class SpellFactory : ISpellFactory
{
    private readonly ILogger Logger;
    private readonly ISpellScriptFactory SpellScriptFactory;
    private readonly ISimpleCache<string, SpellTemplate> SpellTemplateCache;

    public SpellFactory(
        ISimpleCache<string, SpellTemplate> spellTemplateCache,
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
        extraScriptKeys ??= new List<string>();
        var template = SpellTemplateCache.GetObject(templateKey);
        var spell = new Spell(template);

        var scriptKeys = template.ScriptKeys
                                 .Concat(extraScriptKeys)
                                 .ToHashSet(StringComparer.OrdinalIgnoreCase);

        spell.Script = SpellScriptFactory.CreateScript(scriptKeys, spell);
        spell.UniqueId = ServerId.NextId;
        Logger.LogDebug("Created spell \"{SpellName}\" with unique id \"{UniqueId}\"", spell.Template.Name, spell.UniqueId);

        return spell;
    }
}