using System.Collections.Generic;
using AutoMapper;
using Chaos.Containers;
using Chaos.DataObjects.Serializable;
using Chaos.Effects.Interfaces;
using Chaos.Extensions;
using Chaos.Managers.Interfaces;

namespace Chaos.Mappers;

public class EffectMapper : Profile
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly ICacheManager<string, IEffect> EffectManager;

    public EffectMapper(ICacheManager<string, IEffect> effectManager)
    {
        EffectManager = effectManager;

        CreateMap<SerializableEffect, IEffect>(MemberList.None)
            .ConstructUsing(e => EffectManager.GetObject(e.EffectKey));

        CreateMap<IEnumerable<SerializableEffect>, EffectsBar>(MemberList.None)
            .ConstructUsing((e, rc) => new EffectsBar());

        CreateMap<EffectsBar, IEnumerable<SerializableEffect>>(MemberList.None)
            .ConstructUsing((e, rc) => new List<SerializableEffect>());
    }
}