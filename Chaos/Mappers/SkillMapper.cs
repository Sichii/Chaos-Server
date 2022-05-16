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
    private readonly ILogger Logger;
    private readonly ISkillScriptFactory SkillScriptFactory;
    private readonly ISimpleCache<string, SkillTemplate> SkillTemplateCache;

    public SkillMapper(
        ISimpleCache<string, SkillTemplate> skillTemplateCache,
        ISkillScriptFactory skillScriptFactory,
        ILogger<SkillMapper> logger
    )
    {
        SkillTemplateCache = skillTemplateCache;
        SkillScriptFactory = skillScriptFactory;
        Logger = logger;

        CreateMap<SerializableSkill, Skill>(MemberList.None)
            .ConstructUsing(s => new Skill(SkillTemplateCache.GetObject(s.TemplateKey)))
            .ForMember(
                dest => dest.Elapsed,
                o => o.MapFrom(src => TimeSpan.FromMilliseconds(src.ElapsedMs)))
            .AfterMap(
                (_, dest) =>
                {
                    var scriptKeys = dest.Template.ScriptKeys
                                         .Concat(dest.ScriptKeys)
                                         .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    dest.Script = SkillScriptFactory.CreateScript(scriptKeys, dest);
                });

        CreateMap<Skill, SerializableSkill>(MemberList.None)
            .ForMember(
                dest => dest.ElapsedMs,
                o => o.MapFrom(src => src.Elapsed.TotalMilliseconds))
            .ForMember(
                s => s.TemplateKey,
                o => o.MapFrom(s => s.Template.TemplateKey));

        CreateMap<Skill, SkillArg>(MemberList.None)
            .ForMember(
                a => a.Name,
                o => o.MapFrom(s => s.Template.Name))
            .ForMember(
                a => a.Sprite,
                o => o.MapFrom(s => s.Template.PanelSprite));

        CreateMap<IEnumerable<SerializableSkill>, IPanel<Skill>>(MemberList.None)
            .DisableCtorValidation()
            .AfterMap(
                (src, dest, rc) =>
                {
                    foreach (var sSkill in src)
                    {
                        var skill = rc.Mapper.Map<Skill>(sSkill);
                        dest.TryAdd(skill.Slot, skill);
                    }
                });

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