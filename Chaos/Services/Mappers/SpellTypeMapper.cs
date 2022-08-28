using Chaos.Entities.Networking.Server;
using Chaos.Entities.Schemas.World;
using Chaos.Objects.Panel;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Mappers.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chaos.Services.Mappers;

public class SpellTypeMapper : ITypeMapper<Spell, SpellSchema>,
                               ITypeMapper<Spell, SpellInfo>
{
    private readonly ILogger<SpellTypeMapper> Logger;
    private readonly ISimpleCache SimpleCache;
    private readonly ISpellScriptFactory SpellScriptFactory;

    public SpellTypeMapper(
        ISimpleCache spellTemplateCache,
        ISpellScriptFactory spellScriptFactory,
        ILogger<SpellTypeMapper> logger
    )
    {
        SimpleCache = spellTemplateCache;
        SpellScriptFactory = spellScriptFactory;
        Logger = logger;
    }

    public Spell Map(SpellSchema obj)
    {
        var spell = new Spell(obj, SimpleCache, SpellScriptFactory);

        Logger.LogTrace("Deserialized spell - Name: {SpellName}, UniqueId: {UniqueId}", spell.Template.Name, spell.UniqueId);

        return spell;
    }

    public Spell Map(SpellInfo obj) => throw new NotImplementedException();
    SpellInfo ITypeMapper<Spell, SpellInfo>.Map(Spell obj) => new()
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
            TemplateKey = obj.Template.TemplateKey
        };

        Logger.LogTrace("Serialized spell - TemplateKey: {TemplateKey}, UniqueId: {UniqueId}", ret.TemplateKey, ret.UniqueId);

        return ret;
    }
}