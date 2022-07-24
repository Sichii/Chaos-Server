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
    public SpellMapper()
    {
        CreateMap<Spell, SerializableSpell>(MemberList.None)
            .ForMember(
                dest => dest.ElapsedMs,
                o => o.MapFrom(src => src.Elapsed.TotalMilliseconds))
            .ForMember(
                s => s.TemplateKey,
                o => o.MapFrom(s => s.Template.TemplateKey));

        CreateMap<Spell, SpellInfo>(MemberList.None)
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