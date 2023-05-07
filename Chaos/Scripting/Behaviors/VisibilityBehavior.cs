using System.Collections.Immutable;
using Chaos.Definitions;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.EffectScripts.HideEffects;

namespace Chaos.Scripting.Behaviors;

public class VisibilityBehavior
{
    private readonly ImmutableList<string> SeeHiddenEffects = ImmutableList.Create(
        EffectBase.GetEffectKey(typeof(SeeHideEffect)),
        EffectBase.GetEffectKey(typeof(SeeTrueHideEffect)));

    private readonly ImmutableList<string> SeeTrueHiddenEffects = ImmutableList.Create(EffectBase.GetEffectKey(typeof(SeeTrueHideEffect)));

    public virtual bool CanSee(Creature creature, VisibleEntity entity) =>
        entity.Visibility switch
        {
            VisibilityType.Normal     => true,
            VisibilityType.Hidden     => SeeHiddenEffects.Any(key => creature.Effects.Contains(key)),
            VisibilityType.TrueHidden => SeeTrueHiddenEffects.Any(key => creature.Effects.Contains(key)),
            _                         => false
        };
}