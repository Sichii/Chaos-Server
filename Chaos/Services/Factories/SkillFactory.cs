using Chaos.Objects.Panel;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public sealed class SkillFactory : ISkillFactory
{
    private readonly ILogger<SkillFactory> Logger;
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;

    public SkillFactory(
        ISimpleCache simpleCache,
        IScriptProvider scriptProvider,
        ILogger<SkillFactory> logger
    )
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
        Logger = logger;
    }

    public Skill Create(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= Array.Empty<string>();
        var template = SimpleCache.Get<SkillTemplate>(templateKey);
        var skill = new Skill(template, ScriptProvider, extraScriptKeys);

        Logger.LogTrace("Created skill {Skill}", skill);

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