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

public sealed class SpellMapperProfile : IMapperProfile<Spell, SpellSchema>,
                                         IMapperProfile<Spell, SpellInfo>,
                                         IMapperProfile<SpellTemplate, SpellTemplateSchema>
{
    private readonly ILogger<SpellMapperProfile> Logger;
    private readonly ITypeMapper Mapper;
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;

    public SpellMapperProfile(
        ISimpleCache simpleCache,
        IScriptProvider scriptProvider,
        ILogger<SpellMapperProfile> logger,
        ITypeMapper mapper
    )
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
        Logger = logger;
        Mapper = mapper;
    }

    public Spell Map(SpellSchema obj)
    {
        var template = SimpleCache.Get<SpellTemplate>(obj.TemplateKey);

        var spell = new Spell(
            template,
            ScriptProvider,
            obj.ScriptKeys,
            obj.UniqueId,
            obj.ElapsedMs)
        {
            Slot = obj.Slot ?? 0
        };

        return spell;
    }

    public Spell Map(SpellInfo obj) => throw new NotImplementedException();

    SpellInfo IMapperProfile<Spell, SpellInfo>.Map(Spell obj) => new()
    {
        CastLines = obj.CastLines,
        Name = obj.Template.Name,
        Prompt = obj.Template.Prompt ?? string.Empty,
        Slot = obj.Slot,
        SpellType = obj.Template.SpellType,
        Sprite = obj.Template.PanelSprite
    };

    public SpellSchema Map(Spell obj)
    {
        var ret = new SpellSchema
        {
            UniqueId = obj.UniqueId,
            ElapsedMs = obj.Elapsed.HasValue ? Convert.ToInt32(obj.Elapsed.Value.TotalMilliseconds) : null,
            ScriptKeys = obj.ScriptKeys.Except(obj.Template.ScriptKeys).ToHashSet(StringComparer.OrdinalIgnoreCase),
            TemplateKey = obj.Template.TemplateKey,
            Slot = obj.Slot
        };

        return ret;
    }

    public SpellTemplate Map(SpellTemplateSchema obj) => new()
    {
        TemplateKey = obj.TemplateKey,
        Name = obj.Name,
        ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
        CastLines = obj.CastLines,
        Prompt = obj.Prompt,
        SpellType = obj.SpellType,
        Cooldown = obj.CooldownMs == null ? null : TimeSpan.FromMilliseconds(obj.CooldownMs.Value),
        PanelSprite = obj.PanelSprite,
        ScriptVars = new Dictionary<string, IScriptVars>(
            obj.ScriptVars.Select(kvp => new KeyValuePair<string, IScriptVars>(kvp.Key, kvp.Value)),
            StringComparer.OrdinalIgnoreCase),
        Description = obj.Description,
        LearningRequirements = obj.LearningRequirements == null ? null : Mapper.Map<LearningRequirements>(obj.LearningRequirements),
        Level = obj.Level,
        Class = obj.Class,
        AdvClass = obj.AdvClass,
        RequiresMaster = obj.RequiresMaster
    };

    public SpellTemplateSchema Map(SpellTemplate obj) => throw new NotImplementedException();
}