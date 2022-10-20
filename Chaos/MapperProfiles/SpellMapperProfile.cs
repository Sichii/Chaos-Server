using Chaos.Networking.Entities.Server;
using Chaos.Objects.Panel;
using Chaos.Schemas.Aisling;
using Chaos.Scripting.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.MapperProfiles;

public sealed class SpellMapperProfile : IMapperProfile<Spell, SpellSchema>,
                                         IMapperProfile<Spell, SpellInfo>
{
    private readonly ILogger<SpellMapperProfile> Logger;
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;

    public SpellMapperProfile(
        ISimpleCache simpleCache,
        IScriptProvider scriptProvider,
        ILogger<SpellMapperProfile> logger
    )
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
        Logger = logger;
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

        Logger.LogTrace("Deserialized spell - Name: {SpellName}, UniqueId: {UniqueId}", spell.Template.Name, spell.UniqueId);

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

        Logger.LogTrace("Serialized spell - TemplateKey: {TemplateKey}, UniqueId: {UniqueId}", ret.TemplateKey, ret.UniqueId);

        return ret;
    }
}