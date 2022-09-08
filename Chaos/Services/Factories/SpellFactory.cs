using Chaos.Objects.Panel;
using Chaos.Services.Caches.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Scripting.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public class SpellFactory : ISpellFactory
{
    private readonly ILogger Logger;
    private readonly ISimpleCache SimpleCache;
    private readonly IScriptProvider ScriptProvider;

    public SpellFactory(
        ISimpleCache simpleCache,
        IScriptProvider scriptProvider,
        ILogger<SpellFactory> logger
    )
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
        Logger = logger;
    }

    public Spell Create(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= Array.Empty<string>();
        var template = SimpleCache.GetObject<SpellTemplate>(templateKey);
        var spell = new Spell(template, ScriptProvider, extraScriptKeys);

        Logger.LogDebug("Created spell - Name: {SpellName}, UniqueId: {UniqueId}", spell.Template.Name, spell.UniqueId);

        return spell;
    }
}