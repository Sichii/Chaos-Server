using Chaos.Schemas.Aisling;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class EffectMapperProfile : IMapperProfile<IEffect, EffectSchema>
{
    private readonly IEffectFactory EffectFactory;

    public EffectMapperProfile(IEffectFactory effectFactory) => EffectFactory = effectFactory;

    /// <inheritdoc />
    public IEffect Map(EffectSchema obj)
    {
        var effect = EffectFactory.Create(obj.EffectKey);
        effect.Remaining = TimeSpan.FromSeconds(obj.RemainingSecs);

        return effect;
    }

    /// <inheritdoc />
    public EffectSchema Map(IEffect obj)
        => new()
        {
            EffectKey = EffectBase.GetEffectKey(obj.GetType()),
            RemainingSecs = Convert.ToInt32(Math.Ceiling(obj.Remaining.TotalSeconds))
        };
}