using AutoMapper;
using Chaos.Caches.Interfaces;
using Chaos.Effects.Interfaces;
using Chaos.Objects.Serializable;

namespace Chaos.Mappers;

public class EffectMapper : Profile
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly ISimpleCache<string, IEffect> EffectCache;

    public EffectMapper(ISimpleCache<string, IEffect> effectCache)
    {
        EffectCache = effectCache;

        CreateMap<SerializableEffect, IEffect>(MemberList.None)
            .ConstructUsing(e => EffectCache.GetObject(e.Name))
            .ForMember(
                dest => dest.Remaining,
                o =>
                {
                    o.PreCondition(src => src.RemainingSecs.HasValue);
                    o.MapFrom(src => TimeSpan.FromSeconds(src.RemainingSecs!.Value));
                });

        CreateMap<IEffect, SerializableEffect>(MemberList.None)
            .ForMember(
                dest => dest.RemainingSecs,
                o =>
                {
                    o.PreCondition(src => src.Remaining.HasValue);
                    o.MapFrom(src => src.Remaining!.Value.TotalSeconds);
                });

        CreateMap<IEnumerable<SerializableEffect>, IEffectsBar>(MemberList.None)
            .DisableCtorValidation()
            .AfterMap(
                (src, dest, rc) =>
                {
                    foreach (var serializableEffect in src)
                        dest.Add(rc.Mapper.Map<IEffect>(serializableEffect));
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