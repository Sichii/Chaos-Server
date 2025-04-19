#region
using Chaos.Collections.Common;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
#endregion

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

    /// <inheritdoc />
    public Spell CreateScriptProxy(ICollection<string>? extraScriptKeys = null)
    {
        var template = new SpellTemplate
        {
            TemplateKey = "scriptProxy",
            Name = "Script Proxy",
            LearningRequirements = null,
            LevelsUp = false,
            MaxLevel = 0,
            AbilityLevel = 0,
            AdvClass = null,
            Class = null,
            Cooldown = null,
            Description = null,
            Level = 0,
            PanelSprite = 0,
            RequiresMaster = false,
            ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase),
            CastLines = 0,
            Prompt = null,
            SpellType = SpellType.NoTarget
        };

        var spell = new Spell(
            template,
            ScriptProvider,
            extraScriptKeys,
            0);

        return spell;
    }
}