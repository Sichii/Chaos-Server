using Chaos.Objects.Panel;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public class SkillFactory : ISkillFactory
{
    private readonly ILogger Logger;
    private readonly ISimpleCache SimpleCache;
    private readonly ISkillScriptFactory SkillScriptFactory;

    public SkillFactory(
        ISimpleCache simpleCache,
        ISkillScriptFactory skillScriptFactory,
        ILogger<SkillFactory> logger
    )
    {
        SimpleCache = simpleCache;
        SkillScriptFactory = skillScriptFactory;
        Logger = logger;
    }

    public Skill Clone(Skill obj)
    {
        var cloned = new Skill(obj.Template, SkillScriptFactory, obj.ScriptKeys)
        {
            Elapsed = obj.Elapsed
        };

        Logger.LogDebug(
            "Cloned skill - Name: {SkillName}, UniqueId: {UniqueId}, ClonedId: {ClonedId}",
            obj.Template.Name,
            obj.UniqueId,
            cloned.UniqueId);

        return cloned;
    }

    public Skill Create(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= Array.Empty<string>();
        var template = SimpleCache.GetObject<SkillTemplate>(templateKey);
        var skill = new Skill(template, SkillScriptFactory, extraScriptKeys);

        Logger.LogDebug("Created skill - Name: {SkillName}, UniqueId: {UniqueId}", skill.Template.Name, skill.UniqueId);

        return skill;
    }
}