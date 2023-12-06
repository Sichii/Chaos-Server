using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Services.Factories;

public sealed class SpellFactory(ISimpleCache simpleCache, IScriptProvider scriptProvider) : ISpellFactory
{
    private readonly IScriptProvider ScriptProvider = scriptProvider;
    private readonly ISimpleCache SimpleCache = simpleCache;

    public Spell Create(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= Array.Empty<string>();
        var template = SimpleCache.Get<SpellTemplate>(templateKey);
        var spell = new Spell(template, ScriptProvider, extraScriptKeys);

        return spell;
    }

    /// <inheritdoc />
    public Spell CreateFaux(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= new List<string>();
        var template = SimpleCache.Get<SpellTemplate>(templateKey);

        //creates an spell with a unique id of 0
        var spell = new Spell(
            template,
            ScriptProvider,
            extraScriptKeys,
            0);

        //no need to log creation of faux spells

        return spell;
    }
}