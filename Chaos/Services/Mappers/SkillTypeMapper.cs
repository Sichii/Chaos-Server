using Chaos.Entities.Networking.Server;
using Chaos.Entities.Schemas.World;
using Chaos.Objects.Panel;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Mappers.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Mappers;

public class SkillTypeMapper : ITypeMapper<Skill, SkillSchema>,
                               ITypeMapper<Skill, SkillInfo>
{
    private readonly ILogger<SkillTypeMapper> Logger;
    private readonly ISimpleCache SimpleCache;
    private readonly ISkillScriptFactory SkillScriptFactory;

    public SkillTypeMapper(
        ISimpleCache skillTemplateCache,
        ISkillScriptFactory skillScriptFactory,
        ILogger<SkillTypeMapper> logger
    )
    {
        SimpleCache = skillTemplateCache;
        SkillScriptFactory = skillScriptFactory;
        Logger = logger;
    }

    public Skill Map(SkillSchema obj)
    {
        var skill = new Skill(obj, SimpleCache, SkillScriptFactory);

        Logger.LogTrace("Deserialized skill - Name: {SkillName}, UniqueId: {UniqueId}", skill.Template.Name, skill.UniqueId);

        return skill;
    }

    public Skill Map(SkillInfo obj) => throw new NotImplementedException();
    SkillInfo ITypeMapper<Skill, SkillInfo>.Map(Skill obj) => new()
    {
        Name = obj.Template.Name,
        Slot = obj.Slot,
        Sprite = obj.Template.PanelSprite
    };

    public SkillSchema Map(Skill obj)
    {
        var ret = new SkillSchema
        {
            UniqueId = obj.UniqueId,
            ElapsedMs = obj.Elapsed.HasValue ? Convert.ToInt32(obj.Elapsed.Value.TotalMilliseconds) : null,
            ScriptKeys = obj.ScriptKeys.Except(obj.Template.ScriptKeys).ToHashSet(StringComparer.OrdinalIgnoreCase),
            TemplateKey = obj.Template.TemplateKey
        };

        Logger.LogTrace("Serialized skill - TemplateKey: {TemplateKey}, UniqueId: {UniqueId}", ret.TemplateKey, ret.UniqueId);

        return ret;
    }
}