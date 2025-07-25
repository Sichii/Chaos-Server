#region
using Chaos.Collections.Common;
using Chaos.Common.Utilities;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Schemas.Aisling;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Storage;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
#endregion

namespace Chaos.Services.MapperProfiles;

public class EffectMapperProfile(IEffectFactory effectFactory) : IMapperProfile<IEffect, EffectSchema>
{
    private readonly IEffectFactory EffectFactory = effectFactory;

    /// <inheritdoc />
    public IEffect Map(EffectSchema obj)
    {
        var effect = EffectFactory.Create(obj.EffectKey);
        effect.Remaining = TimeSpan.FromSeconds(obj.RemainingSecs);
        effect.SnapshotVars = obj.SnapshotVars;

        return effect;
    }

    /// <inheritdoc />
    public EffectSchema Map(IEffect obj)
        => new()
        {
            EffectKey = EffectBase.GetEffectKey(obj.GetType()),
            RemainingSecs = Convert.ToInt32(Math.Ceiling(obj.Remaining.TotalSeconds)),
            SnapshotVars = new StaticVars(obj.SnapshotVars.ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
        };
}