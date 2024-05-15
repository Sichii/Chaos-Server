using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct GenericAbilityComponent<TEntity> : IConditionalComponent where TEntity: MapEntity
{
    /// <inheritdoc />
    public bool Execute(ActivationContext context, ComponentVars vars)
        => new ComponentExecutor(context, vars).ExecuteAndCheck<ManaCostAbilityComponent>()
                                               ?.Execute<BreaksHideAbilityComponent>()
                                               .ExecuteAndCheck<GetTargetsAbilityComponent<TEntity>>()
                                               ?.Execute<BodyAnimationAbilityComponent>()
                                               .Execute<AnimationAbilityComponent>()
                                               .Execute<SoundAbilityComponent>()
           != null;

    public interface IAbilityComponentOptions : GetTargetsAbilityComponent<TEntity>.IGetTargetsComponentOptions,
                                                SoundAbilityComponent.ISoundComponentOptions,
                                                BodyAnimationAbilityComponent.IBodyAnimationComponentOptions,
                                                AnimationAbilityComponent.IAnimationComponentOptions,
                                                ManaCostAbilityComponent.IManaCostComponentOptions,
                                                BreaksHideAbilityComponent.IBreaksHideComponentOptions { }
}