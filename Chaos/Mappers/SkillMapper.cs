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

public class SkillMapper : Profile
{
    public SkillMapper()
    {
        CreateMap<Skill, SerializableSkill>(MemberList.None)
            .ForMember(
                dest => dest.ElapsedMs,
                o => o.MapFrom(src => src.Elapsed.TotalMilliseconds))
            .ForMember(
                s => s.TemplateKey,
                o => o.MapFrom(s => s.Template.TemplateKey));

        CreateMap<Skill, SkillInfo>(MemberList.None)
            .ForMember(
                a => a.Name,
                o => o.MapFrom(s => s.Template.Name))
            .ForMember(
                a => a.Sprite,
                o => o.MapFrom(s => s.Template.PanelSprite));
        
        CreateMap<IPanel<Skill>, ICollection<SerializableSkill>>(MemberList.None)
            .ConstructUsing(src => new List<SerializableSkill>())
            .AfterMap(
                (src, dest, rc) =>
                {
                    foreach (var skill in src)
                    {
                        var sSkill = rc.Mapper.Map<SerializableSkill>(skill);
                        dest.Add(sSkill);
                    }
                });
    }
}