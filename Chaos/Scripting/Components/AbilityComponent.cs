using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;

namespace Chaos.Scripting.Components;

public class AbilityComponent<TEntity> : IConditionalComponent where TEntity: MapEntity
{
    /// <inheritdoc />
    public virtual bool Execute(ActivationContext context, ComponentVars vars) =>
        new ComponentExecutor(context, vars)
            .ExecuteAndCheck<ManaCostComponent>()
            ?
            .Execute<BreaksHideComponent>()
            .ExecuteAndCheck<GetTargetsComponent<TEntity>>()
            ?
            .Execute<BodyAnimationComponent>()
            .Execute<AnimationComponent>()
            .Execute<SoundComponent>()
        != null;

    public interface IAbilityComponentOptions : GetTargetsComponent<TEntity>.IGetTargetsComponentOptions,
                                                SoundComponent.ISoundComponentOptions,
                                                BodyAnimationComponent.IBodyAnimationComponentOptions,
                                                AnimationComponent.IAnimationComponentOptions,
                                                ManaCostComponent.IManaCostComponentOptions,
                                                BreaksHideComponent.IBreaksHideComponentOptions { }
}