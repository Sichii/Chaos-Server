using Chaos.Objects.Panel;
using Chaos.Scripting.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public class SkillFactory : ISkillFactory
{
    private readonly ILogger Logger;
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
        var template = SimpleCache.GetObject<SkillTemplate>(templateKey);
        var skill = new Skill(template, ScriptProvider, extraScriptKeys);

        Logger.LogTrace("Created skill - Name: {SkillName}, UniqueId: {UniqueId}", skill.Template.Name, skill.UniqueId);

        return skill;
    }
}