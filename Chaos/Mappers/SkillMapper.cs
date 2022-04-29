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

public class SkillMapper : Profile
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly ICacheManager<string, SkillTemplate> SkillTemplateManager;

    public SkillMapper(ICacheManager<string, SkillTemplate> skillTemplateManager)
    {
        SkillTemplateManager = skillTemplateManager;
        //TODO: script manager
        //TODO: cooldown

        CreateMap<SerializableSkill, Skill>(MemberList.None)
            .ConstructUsing((s, c) => new Skill(SkillTemplateManager.GetObject(s.TemplateKey)));

        CreateMap<Skill, SerializableSkill>(MemberList.None)
            .ForMember(s => s.TemplateKey,
                o => o.MapFrom(s => s.Template.TemplateKey));

        CreateMap<Skill, SkillArg>(MemberList.None)
            .ForMember(a => a.Name,
                o => o.MapFrom(s => s.Template.Name))
            .ForMember(a => a.Sprite,
                o => o.MapFrom(s => s.Template.Sprite));
        
        CreateMap<IEnumerable<SerializableSkill>, SkillBook>(MemberList.None)
            .DisableCtorValidation()
            .IgnoreAllUnmapped()
            .AfterMap((src, dest, rc) =>
            {
                foreach (var sSkill in src)
                {
                    var skill = rc.Mapper.Map<Skill>(sSkill);
                    dest.TryAdd(skill.Slot, skill);
                }
            });

        
        CreateMap<SkillBook, List<SerializableSkill>>(MemberList.None)
            .ConstructUsing(src => new List<SerializableSkill>())
            .IgnoreAllUnmapped()
            .AfterMap((src, dest, rc) =>
            {
                foreach (var skill in src)
                {
                    var sSkill = rc.Mapper.Map<SerializableSkill>(skill);
                    dest.Add(sSkill);
                }
            });
    }
}