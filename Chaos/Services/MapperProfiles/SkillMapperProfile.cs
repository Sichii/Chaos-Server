#region
using Chaos.Common.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Networking.Entities.Server;
using Chaos.Schemas.Aisling;
using Chaos.Schemas.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
#endregion

namespace Chaos.Services.MapperProfiles;

public sealed class SkillMapperProfile(ISimpleCache simpleCache, IScriptProvider scriptProvider, ITypeMapper mapper)
    : IMapperProfile<Skill, SkillSchema>, IMapperProfile<Skill, SkillInfo>, IMapperProfile<SkillTemplate, SkillTemplateSchema>
{
    private readonly ITypeMapper Mapper = mapper;
    private readonly IScriptProvider ScriptProvider = scriptProvider;
    private readonly ISimpleCache SimpleCache = simpleCache;

    public Skill Map(SkillInfo obj) => throw new NotImplementedException();

    SkillInfo IMapperProfile<Skill, SkillInfo>.Map(Skill obj)
        => new()
        {
            Name = obj.Template.Name,
            PanelName = obj.PanelDisplayName,
            Slot = obj.Slot,
            Sprite = obj.Template.PanelSprite
        };

    public Skill Map(SkillSchema obj)
    {
        var template = SimpleCache.Get<SkillTemplate>(obj.TemplateKey);
        var maxLevel = template.LevelsUp ? obj.MaxLevel ?? template.MaxLevel : template.MaxLevel;
        var level = template.LevelsUp ? obj.Level ?? 0 : maxLevel;

        var skill = new Skill(
            template,
            ScriptProvider,
            obj.ScriptKeys,
            obj.UniqueId,
            obj.ElapsedMs)
        {
            Slot = obj.Slot ?? 0,
            MaxLevel = maxLevel,
            Level = level
        };

        return skill;
    }

    public SkillSchema Map(Skill obj)
    {
        var extraScriptKeys = obj.ScriptKeys
                                 .Except(obj.Template.ScriptKeys)
                                 .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var ret = new SkillSchema
        {
            UniqueId = obj.UniqueId,
            ElapsedMs = obj.Elapsed.HasValue ? Convert.ToInt32(obj.Elapsed.Value.TotalMilliseconds) : null,
            ScriptKeys = extraScriptKeys.Count != 0 ? extraScriptKeys : null,
            TemplateKey = obj.Template.TemplateKey,
            Slot = obj.Slot,
            Level = obj.Template.LevelsUp ? obj.Level : null,
            MaxLevel = obj.Template.LevelsUp && (obj.MaxLevel != obj.Template.MaxLevel) ? obj.MaxLevel : null
        };

        return ret;
    }

    public SkillTemplate Map(SkillTemplateSchema obj)
        => new()
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
            LearningRequirements = obj.LearningRequirements == null ? null : Mapper.Map<LearningRequirements>(obj.LearningRequirements),
            Level = obj.Level,
            AbilityLevel = obj.AbilityLevel,
            Class = obj.Class,
            AdvClass = obj.AdvClass,
            RequiresMaster = obj.RequiresMaster,
            LevelsUp = obj.LevelsUp,
            MaxLevel = obj.MaxLevel ?? 100
        };

    public SkillTemplateSchema Map(SkillTemplate obj) => throw new NotImplementedException();
}