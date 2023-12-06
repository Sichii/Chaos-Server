using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Services.Factories;

public sealed class SkillFactory(ISimpleCache simpleCache, IScriptProvider scriptProvider) : ISkillFactory
{
    private readonly IScriptProvider ScriptProvider = scriptProvider;
    private readonly ISimpleCache SimpleCache = simpleCache;

    public Skill Create(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= Array.Empty<string>();
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
}