using System.Collections.Generic;
using AutoMapper;
using Chaos.Caches.Interfaces;
using Chaos.Containers;
using Chaos.Effects.Interfaces;
using Chaos.Managers.Interfaces;
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
            .ConstructUsing(e => EffectCache.GetObject(e.EffectKey));

        CreateMap<IEnumerable<SerializableEffect>, EffectsBar>(MemberList.None)
            .ConstructUsing((e, rc) => new EffectsBar());

        CreateMap<EffectsBar, IEnumerable<SerializableEffect>>(MemberList.None)
            .ConstructUsing((e, rc) => new List<SerializableEffect>());
    }
}