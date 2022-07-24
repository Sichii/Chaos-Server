using AutoMapper;
using Chaos.Caches.Interfaces;
using Chaos.Effects.Interfaces;
using Chaos.Objects.Serializable;

namespace Chaos.Mappers;

public class EffectMapper : Profile
{

    public EffectMapper()
    {

        CreateMap<IEffect, SerializableEffect>(MemberList.None)
            .ForMember(
                dest => dest.RemainingSecs,
                o =>
                {
                    o.PreCondition(src => src.Remaining.HasValue);
                    o.MapFrom(src => src.Remaining!.Value.TotalSeconds);
                });
        
        CreateMap<IEffectsBar, IEnumerable<SerializableEffect>>(MemberList.None)
            .ConstructUsing(
                (src, rc) =>
                {
                    var ret = new List<SerializableEffect>();

                    foreach (var effect in src)
                        ret.Add(rc.Mapper.Map<SerializableEffect>(effect));

                    return ret;
                });
    }
}