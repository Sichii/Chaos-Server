using Chaos.Caches;
using Chaos.Caches.Interfaces;
using Chaos.Factories.Interfaces;
using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Factories;

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

    public Skill CreateSkill(string templateKey, ICollection<string>? extraScriptKeys = null)
    {
        extraScriptKeys ??= Array.Empty<string>();
        var template = SkillTemplateCache.GetObject(templateKey);
        var skill = new Skill(template, SkillScriptFactory, extraScriptKeys);
        
        Logger.LogDebug("Created skill - Name: {SkillName}, UniqueId: {UniqueId}", skill.Template.Name, skill.UniqueId);

        return skill;
    }

    public Skill DeserializeSkill(SerializableSkill serializableSkill)
    {
        var skill = new Skill(serializableSkill, SkillTemplateCache, SkillScriptFactory);

        Logger.LogDebug("Deserialized skill - Name: {SkillName}, UniqueId: {UniqueId}", skill.Template.Name, skill.UniqueId);

        return skill;
    }

    public Skill CloneSkill(Skill skill)
    {
        var cloned = new Skill(skill.Template, SkillScriptFactory, skill.ScriptKeys)
        {
            Elapsed = skill.Elapsed,
        };

        Logger.LogDebug(
            "Cloned skill - Name: {SkillName}, UniqueId: {UniqueId}, ClonedId: {ClonedId}",
            skill.Template.Name,
            skill.UniqueId,
            cloned.UniqueId);

        return cloned;
    }
}