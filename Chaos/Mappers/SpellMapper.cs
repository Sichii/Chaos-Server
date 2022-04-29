using System.Collections.Generic;
using AutoMapper;
using Chaos.Containers;
using Chaos.DataObjects.Serializable;
using Chaos.Extensions;
using Chaos.Managers.Interfaces;
using Chaos.Networking.Model.Server;
using Chaos.PanelObjects;
using Chaos.Templates;

namespace Chaos.Mappers;

public class SpellMapper : Profile
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly ICacheManager<string, SpellTemplate> SpellTemplateManager;

    public SpellMapper(ICacheManager<string, SpellTemplate> spellTemplateManager)
    {
        SpellTemplateManager = spellTemplateManager;
        //TODO: script manager
        //TODO: cooldown

        CreateMap<SerializableSpell, Spell>(MemberList.None)
            .ConstructUsing((s, c) => new Spell(SpellTemplateManager.GetObject(s.TemplateKey)));

        CreateMap<Spell, SerializableSpell>(MemberList.None)
            .ForMember(s => s.TemplateKey,
                o => o.MapFrom(s => s.Template.TemplateKey));

        CreateMap<Spell, SpellArg>(MemberList.None)
            .ForMember(a => a.Name,
                o => o.MapFrom(s => s.Template.Name))
            .ForMember(a => a.Sprite,
                o => o.MapFrom(s => s.Template.Sprite))
            .ForMember(a => a.Prompt,
                o => o.MapFrom(s => s.Template.Prompt))
            .ForMember(a => a.SpellType,
                o => o.MapFrom(s => s.Template.SpellType));
        
        CreateMap<IEnumerable<SerializableSpell>, SpellBook>(MemberList.None)
            .DisableCtorValidation()
            .IgnoreAllUnmapped()
            .AfterMap((src, dest, rc) =>
            {
                foreach (var sSpell in src)
                {
                    var spell = rc.Mapper.Map<Spell>(sSpell);
                    dest.TryAdd(spell.Slot, spell);
                }
            });

        
        CreateMap<SpellBook, List<SerializableSpell>>(MemberList.None)
            .ConstructUsing(src => new List<SerializableSpell>())
            .IgnoreAllUnmapped()
            .AfterMap((src, dest, rc) =>
            {
                foreach (var spell in src)
                {
                    var sSpell = rc.Mapper.Map<SerializableSpell>(spell);
                    dest.Add(sSpell);
                }
            });
    }
}