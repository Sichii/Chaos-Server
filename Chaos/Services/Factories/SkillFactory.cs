using Chaos.Objects.Panel;
using Chaos.Services.Caches.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Scripting.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public class SkillFactory : ISkillFactory
{
    private readonly ILogger Logger;
    private readonly ISimpleCache SimpleCache;
    private readonly IScriptProvider ScriptProvider;

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

        Logger.LogDebug("Created skill - Name: {SkillName}, UniqueId: {UniqueId}", skill.Template.Name, skill.UniqueId);

        return skill;
    }
}