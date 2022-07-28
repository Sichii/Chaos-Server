using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Serialization.Interfaces;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Serialization;

public class SkillSerialTransformService : ISerialTransformService<Skill, SerializableSkill>
{
    private readonly ILogger<SkillSerialTransformService> Logger;
    private readonly ISkillScriptFactory SkillScriptFactory;
    private readonly ISimpleCache<SkillTemplate> SkillTemplateCache;

    public SkillSerialTransformService(
        ISimpleCache<SkillTemplate> skillTemplateCache,
        ISkillScriptFactory skillScriptFactory,
        ILogger<SkillSerialTransformService> logger
    )
    {
        SkillTemplateCache = skillTemplateCache;
        SkillScriptFactory = skillScriptFactory;
        Logger = logger;
    }

    public Skill Transform(SerializableSkill serialized)
    {
        var skill = new Skill(serialized, SkillTemplateCache, SkillScriptFactory);

        Logger.LogTrace("Deserialized skill - Name: {SkillName}, UniqueId: {UniqueId}", skill.Template.Name, skill.UniqueId);

        return skill;
    }

    public SerializableSkill Transform(Skill entity)
    {
        var ret = new SerializableSkill(entity);

        Logger.LogTrace("Serialized skill - TemplateKey: {TemplateKey}, UniqueId: {UniqueId}", ret.TemplateKey, ret.UniqueId);

        return ret;
    }
}