using AutoMapper;
using Chaos.Caches.Interfaces;
using Chaos.Containers.Interfaces;
using Chaos.Factories.Interfaces;
using Chaos.Networking.Model.Server;
using Chaos.Objects.Panel;
using Chaos.Objects.Serializable;
using Chaos.Templates;
using Microsoft.Extensions.Logging;

namespace Chaos.Mappers;

public class SpellMapper : Profile
{
    private readonly ILogger Logger;
    private readonly ISpellScriptFactory SpellScriptFactory;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly ISimpleCache<string, SpellTemplate> SpellTemplateCache;

    public SpellMapper(
        ISimpleCache<string, SpellTemplate> spellTemplateCache,
        ISpellScriptFactory spellScriptFactory,
        ILogger<SpellMapper> logger
    )
    {
        SpellTemplateCache = spellTemplateCache;
        SpellScriptFactory = spellScriptFactory;
        Logger = logger;

        CreateMap<SerializableSpell, Spell>(MemberList.None)
            .ConstructUsing(s => new Spell(SpellTemplateCache.GetObject(s.TemplateKey)))
            .ForMember(
                dest => dest.Elapsed,
                o => o.MapFrom(src => TimeSpan.FromMilliseconds(src.ElapsedMs)))
            .AfterMap(
                (_, dest) =>
                {
                    var scriptKeys = dest.Template.ScriptKeys
                                         .Concat(dest.ScriptKeys)
                                         .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    dest.Script = SpellScriptFactory.CreateScript(scriptKeys, dest);
                });

        CreateMap<Spell, SerializableSpell>(MemberList.None)
            .ForMember(
                dest => dest.ElapsedMs,
                o => o.MapFrom(src => src.Elapsed.TotalMilliseconds))
            .ForMember(
                s => s.TemplateKey,
                o => o.MapFrom(s => s.Template.TemplateKey));

        CreateMap<Spell, SpellArg>(MemberList.None)
            .ForMember(
                a => a.Name,
                o => o.MapFrom(s => s.Template.Name))
            .ForMember(
                a => a.Sprite,
                o => o.MapFrom(s => s.Template.PanelSprite))
            .ForMember(
                a => a.Prompt,
                o => o.MapFrom(s => s.Template.Prompt))
            .ForMember(
                a => a.SpellType,
                o => o.MapFrom(s => s.Template.SpellType));

        CreateMap<IEnumerable<SerializableSpell>, IPanel<Spell>>(MemberList.None)
            .DisableCtorValidation()
            .AfterMap(
                (src, dest, rc) =>
                {
                    foreach (var sSpell in src)
                    {
                        var spell = rc.Mapper.Map<Spell>(sSpell);
                        dest.TryAdd(spell.Slot, spell);
                    }
                });

        CreateMap<IPanel<Spell>, ICollection<SerializableSpell>>(MemberList.None)
            .ConstructUsing(src => new List<SerializableSpell>())
            .AfterMap(
                (src, dest, rc) =>
                {
                    foreach (var spell in src)
                    {
                        var sSpell = rc.Mapper.Map<SerializableSpell>(spell);
                        dest.Add(sSpell);
                    }
                });
    }
}