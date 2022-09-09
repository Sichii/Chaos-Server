using Chaos.Entities.Networking.Server;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Objects.Panel;
using Chaos.Services.Caches.Abstractions;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Services.Scripting.Abstractions;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Mappers;

public class SpellMapperProfile : IMapperProfile<Spell, SpellSchema>,
                               IMapperProfile<Spell, SpellInfo>
{
    private readonly ILogger<SpellMapperProfile> Logger;
    private readonly ISimpleCache SimpleCache;
    private readonly IScriptProvider ScriptProvider;

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
        var template = SimpleCache.GetObject<SpellTemplate>(obj.TemplateKey);

        var spell = new Spell(
            template,
            ScriptProvider,
            obj.ScriptKeys,
            obj.UniqueId,
            obj.ElapsedMs);

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