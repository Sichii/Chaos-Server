using Chaos.Common.Abstractions;
using Chaos.Data;
using Chaos.Networking.Entities.Server;
using Chaos.Objects.Panel;
using Chaos.Schemas.Aisling;
using Chaos.Schemas.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.MapperProfiles;

public sealed class SkillMapperProfile : IMapperProfile<Skill, SkillSchema>,
                                         IMapperProfile<Skill, SkillInfo>,
                                         IMapperProfile<SkillTemplate, SkillTemplateSchema>
{
    private readonly ILogger<SkillMapperProfile> Logger;
    private readonly ITypeMapper Mapper;
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;

    public SkillMapperProfile(
        ISimpleCache simpleCache,
        IScriptProvider scriptProvider,
        ILogger<SkillMapperProfile> logger,
        ITypeMapper mapper
    )
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
        Logger = logger;
        Mapper = mapper;
    }

    public Skill Map(SkillSchema obj)
    {
        var template = SimpleCache.Get<SkillTemplate>(obj.TemplateKey);

        var skill = new Skill(
            template,
            ScriptProvider,
            obj.ScriptKeys,
            obj.UniqueId,
            obj.ElapsedMs)
        {
            Slot = obj.Slot ?? 0
        };

        Logger.LogTrace("Deserialized skill - Name: {SkillName}, UniqueId: {UniqueId}", skill.Template.Name, skill.UniqueId);

        return skill;
    }

    public Skill Map(SkillInfo obj) => throw new NotImplementedException();

    SkillInfo IMapperProfile<Skill, SkillInfo>.Map(Skill obj) => new()
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
            TemplateKey = obj.Template.TemplateKey,
            Slot = obj.Slot
        };

        Logger.LogTrace("Serialized skill - TemplateKey: {TemplateKey}, UniqueId: {UniqueId}", ret.TemplateKey, ret.UniqueId);

        return ret;
    }

    public SkillTemplate Map(SkillTemplateSchema obj) => new()
    {
        TemplateKey = obj.TemplateKey,
        Name = obj.Name,
        IsAssail = obj.IsAssail,
        PanelSprite = obj.PanelSprite,
        ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
        Cooldown = obj.CooldownMs == null ? null : TimeSpan.FromMilliseconds(obj.CooldownMs.Value),
        ScriptVars = new Dictionary<string, IScriptVars>(
            obj.ScriptVars.Select(kvp => new KeyValuePair<string, IScriptVars>(kvp.Key, kvp.Value)),
            StringComparer.OrdinalIgnoreCase),
        Description = obj.Description,
        LearningRequirements = obj.LearningRequirements == null ? null : Mapper.Map<LearningRequirements>(obj.LearningRequirements)
    };

    public SkillTemplateSchema Map(SkillTemplate obj) => throw new NotImplementedException();
}