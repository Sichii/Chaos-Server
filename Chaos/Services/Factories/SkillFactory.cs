#region
using Chaos.Common.Abstractions;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Services.Factories;

public sealed class SkillFactory(ISimpleCache simpleCache, IScriptProvider scriptProvider) : ISkillFactory
{
    private readonly IScriptProvider ScriptProvider = scriptProvider;
    private readonly ISimpleCache SimpleCache = simpleCache;

    public Skill Create(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= [];
        var template = SimpleCache.Get<SkillTemplate>(templateKey);
        var skill = new Skill(template, ScriptProvider, extraScriptKeys);

        return skill;
    }

    /// <inheritdoc />
    public Skill CreateFaux(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= new List<string>();
        var template = SimpleCache.Get<SkillTemplate>(templateKey);

        //creates an skill with a unique id of 0
        var skill = new Skill(
            template,
            ScriptProvider,
            extraScriptKeys,
            0);

        //no need to log creation of faux skills

        return skill;
    }

    /// <inheritdoc />
    public Skill CreateScriptProxy(ICollection<string>? extraScriptKeys = null)
    {
        var template = new SkillTemplate
        {
            TemplateKey = "scriptProxy",
            Name = "Script Proxy",
            IsAssail = false,
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
            ScriptVars = new Dictionary<string, IScriptVars>(StringComparer.OrdinalIgnoreCase)
        };

        var skill = new Skill(
            template,
            ScriptProvider,
            extraScriptKeys,
            0);

        return skill;
    }
}