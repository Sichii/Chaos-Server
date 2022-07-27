using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Factories;

public class SkillFactory : ISkillFactory
{
    private readonly ILogger Logger;
    private readonly ISkillScriptFactory SkillScriptFactory;
    private readonly ISimpleCache<SkillTemplate> SkillTemplateCache;

    public SkillFactory(
        ISimpleCache<SkillTemplate> skillTemplateCache,
        ISkillScriptFactory skillScriptFactory,
        ILogger<SkillFactory> logger
    )
    {
        SkillTemplateCache = skillTemplateCache;
        SkillScriptFactory = skillScriptFactory;
        Logger = logger;
    }

    public Skill Create(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= Array.Empty<string>();
        var template = SkillTemplateCache.GetObject(templateKey);
        var skill = new Skill(template, SkillScriptFactory, extraScriptKeys);
        
        Logger.LogDebug("Created skill - Name: {SkillName}, UniqueId: {UniqueId}", skill.Template.Name, skill.UniqueId);

        return skill;
    }

    public Skill Clone(Skill obj)
    {
        var cloned = new Skill(obj.Template, SkillScriptFactory, obj.ScriptKeys)
        {
            Elapsed = obj.Elapsed,
        };

        Logger.LogDebug(
            "Cloned skill - Name: {SkillName}, UniqueId: {UniqueId}, ClonedId: {ClonedId}",
            obj.Template.Name,
            obj.UniqueId,
            cloned.UniqueId);

        return cloned;
    }
}